using System.Diagnostics;
using System.Drawing;
using VideoCollabServer.Dtos;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;
using VideoCollabServer.Models;
using VideoCollabServer.Utils;
using File = System.IO.File;


namespace VideoCollabServer.Services;

public class HlsService : IHlsService
{
    private static readonly string RawUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "raw");

    private static readonly string HlsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "hls");
    
    private static readonly string FramesPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "frames");
    
    private static readonly List<string> SupportedVideoExtensions = ["mp4", "mov"];
    
    private static readonly Dictionary<string, string?> ContentType = new()
    {
        { ".ts", "video/MP2T" },
        { ".m3u8", "application/x-mpegURL" }
    };

    private static readonly Dictionary<int, string> Renditions = new()
    {
        {427, "427x240_576k_64k"},
        {640, "640x360_704k_64k"},
        {852, "852x480_896k_64k"},
        {1280, "1280x720_1856k_128k"},
        {1920,  "1920x1080_5000k_192k"}
    };
    
    private static string _ffmpegExecutable = null!;

    private readonly Queue<int> _transcodingQueue;

    private readonly IServiceScopeFactory _serviceScope;

    private bool _transcoderBusy;
    
    public HlsService(IConfiguration configuration, IServiceScopeFactory serviceScope)
    {
        _ffmpegExecutable = configuration.GetSection("Executables")["ffmpeg"]!;
        _transcodingQueue = new Queue<int>();
        _serviceScope = serviceScope;
    }

    private async void RunTranscoding()
    {
        _transcoderBusy = true;
        while (_transcodingQueue.Count > 0)
        {
            var movieId = _transcodingQueue.Dequeue();
            
            var rawDirectoryInfo = new DirectoryInfo(RawUploadsPath);
            var movieFileInfo = rawDirectoryInfo.GetFiles($"mov_{movieId}.*").FirstOrDefault();
            if (movieFileInfo is null || !movieFileInfo.Exists)
            {
                await ChangeMovieStatusAsync(movieId, Movie.Statuses.StartTranscodingError);
                continue;
            }
            var moviePath = movieFileInfo.FullName;
            var size = await GetVideoResolution(moviePath, movieId);
            if (!size.Succeeded)
            {
                await ChangeMovieStatusAsync(movieId, Movie.Statuses.StartTranscodingError);
                continue;
            }
            
            var process = new Process();
            CreateTranscodeMovieProcess(process, movieId, size.Value, moviePath);
            await ChangeMovieStatusAsync(movieId, Movie.Statuses.Transcoding);

            process.Start();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                await ChangeMovieStatusAsync(movieId, Movie.Statuses.TranscodingError);
                continue;
            }

            await ChangeMovieStatusAsync(movieId, Movie.Statuses.ReadyToView);
            // TODO: Add Transcoding progress
        }
        _transcoderBusy = false;
    }

    private async Task ChangeMovieStatusAsync(int movieId, Movie.Statuses status)
    {
        using var myScope = _serviceScope.CreateScope();
        var myService = myScope.ServiceProvider.GetRequiredService<ITranscodingMovieRepository>();
        await myService.ChangeMovieStatusAsync(movieId, status);
    }

    private void CreateTranscodeMovieProcess(Process process, int movieId, Size rawMovieSize, string moviePath)
    {
        var directoryOutput = Path.Combine(HlsPath, $"mov_{movieId}");
        const string shellFile = "Utils/ffmpeg.sh";
        
        var renditions = Renditions
            .Where(r => r.Key <= rawMovieSize.Width)
            .Aggregate("", (current, r) => current + $"{r.Value} ");

        process.StartInfo = new ProcessStartInfo
        {
            FileName = $"{shellFile}",
            Arguments = $"{moviePath} {directoryOutput} {_ffmpegExecutable} \"{renditions}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
    }

    private async Task<Result<Size>> GetVideoResolution(string path, int movieId)
    {
        var frameOutput = Path.Combine(FramesPath, $"frame_{movieId}.jpg");
        var proc = new Process();
        proc.StartInfo = new ProcessStartInfo
        {
            FileName = "Utils/get_frame.sh",
            Arguments = $"{path} {frameOutput}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        proc.Start();
        await proc.WaitForExitAsync();
        try
        {
            var size = ImageDimensions.GetDimensions(frameOutput);
            return Result<Size>.Ok(size);
        }
        catch (Exception)
        {
            return Result<Size>.Error("Possibly file is not exist");
        }
    }

    public Result<HlsFileDto> GetHlsFile(int movieId, string quality, string file)
    {
        var fileInfo = new FileInfo(Path.Combine(HlsPath, $"mov_{movieId}", quality, file));

        if (!fileInfo.Exists || !ContentType.TryGetValue(fileInfo.Extension, out var contentType))
            return Result<HlsFileDto>.Error("Internal error");
        
        return Result<HlsFileDto>.Ok(
            new HlsFileDto
            {
                Stream = fileInfo.OpenRead(),
                ContentType = contentType!
            });
    }
    
    public async Task<Result> UploadMovieAsync(int movieId, IFormFile file, IMovieRepository movieRepository)
    {
        var extension = file.FileName.Split(".").Last();

        if (!SupportedVideoExtensions.Contains(extension))
            return new Result
            {
                Errors = ["Unsupported video file extension"],
                Succeeded = false
            };

        if (!await movieRepository.ContainsMovieAsync(movieId))
            return new Result
            {
                Errors = ["Movie doesn't exist"],
                Succeeded = false
            };

        var fileName = $"mov_{movieId}.{extension}";

        await using (var fs = File.Create(Path.Combine(RawUploadsPath, fileName)))
            await file.CopyToAsync(fs);

        _transcodingQueue.Enqueue(movieId);
        if (!_transcoderBusy)
            _ = Task.Run(RunTranscoding);

        return Result.Ok();
    }

    public void DeleteMovie(int movieId)
    {
        var directory = new DirectoryInfo(Path.Combine(HlsPath, $"mov_{movieId}"));
        var rawDirectoryInfo = new DirectoryInfo(RawUploadsPath);
        var rawFileInfo = rawDirectoryInfo.GetFiles($"mov_{movieId}.*").FirstOrDefault();
        if (rawFileInfo is { Exists: true })
            rawFileInfo.Delete();

        if (directory.Exists)
            directory.Delete(true);
    }

    public Result<FileStream> GetPlaylistByMovieId(int movieId)
    {
        var playlistInfo = new FileInfo(Path.Combine(HlsPath, $"mov_{movieId}", "master.m3u8"));
        if (playlistInfo.Exists)
            return Result<FileStream>.Ok(playlistInfo.OpenRead());

        var directory = new DirectoryInfo(RawUploadsPath);
        var files = directory.GetFiles($"mov_{movieId}.*");

        return Result<FileStream>.Error(
            files.Length > 0 ? "Video haven't transcoded to HLS yet" : "Video doesn't exist");
    }
}