using System.Diagnostics;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoCollab.Core.Domain;
using VideoCollab.Core.Domain.Abstractions;
using VideoCollab.Core.Domain.Models;
using VideoCollab.Core.Utils;
using File = System.IO.File;


namespace VideoCollab.Core.Services;

public class HlsService : IHlsService
{
    private readonly string _uploadsPath;
    
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
        _uploadsPath = configuration.GetSection("MediaStorage").Value!;
        _transcodingQueue = new Queue<int>();
        _serviceScope = serviceScope;
    }

    private async void RunTranscoding()
    {
        _transcoderBusy = true;
        while (_transcodingQueue.Count > 0)
        {
            var movieId = _transcodingQueue.Dequeue();
            
            var rawDirectoryInfo = new DirectoryInfo(Path.Combine(_uploadsPath, "raw"));
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
        }
        _transcoderBusy = false;
    }

    private async Task ChangeMovieStatusAsync(int movieId, Movie.Statuses status)
    {
        using var myScope = _serviceScope.CreateScope();
        var myService = myScope.ServiceProvider.GetRequiredService<IMovieService>();
        await myService.ChangeMovieStatusAsync(movieId, status);
    }

    private void CreateTranscodeMovieProcess(Process process, int movieId, Size rawMovieSize, string moviePath)
    {
        var directoryOutput = Path.Combine(Path.Combine(_uploadsPath, "hls"), $"mov_{movieId}");
        const string shellFile = "../VideoCollab.Core/Utils/ffmpeg.sh";
        
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
        var frameOutput = Path.Combine(Path.Combine(_uploadsPath, "frames"), $"frame_{movieId}.jpg");
        var proc = new Process();
        proc.StartInfo = new ProcessStartInfo
        {
            FileName = "../VideoCollab.Core/Utils/get_frame.sh",
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

    public Result<(string, FileStream)> GetHlsFile(int movieId, string quality, string file)
    {
        var fileInfo = new FileInfo(Path.Combine(Path.Combine(_uploadsPath, "hls"), $"mov_{movieId}", quality, file));

        if (!fileInfo.Exists || !ContentType.TryGetValue(fileInfo.Extension, out var contentType))
            return Result<(string, FileStream)>.Error("Internal error");

        return Result<(string, FileStream)>.Ok(new ValueTuple<string, FileStream>(contentType!, fileInfo.OpenRead()));
    }
    
    public async Task<Result> UploadMovieAsync(int movieId, Stream stream, string fileName, IMovieService movieService)
    {
        var extension = fileName.Split(".").Last();

        if (!SupportedVideoExtensions.Contains(extension))
            return new Result
            {
                Errors = ["Unsupported video file extension"],
                Succeeded = false
            };

        if (!await movieService.ContainsMovieAsync(movieId))
            return new Result
            {
                Errors = ["Movie doesn't exist"],
                Succeeded = false
            };

        fileName = $"mov_{movieId}.{extension}";

        await using (var fs = File.Create(Path.Combine(Path.Combine(_uploadsPath, "raw"), fileName)))
            await stream.CopyToAsync(fs);

        _transcodingQueue.Enqueue(movieId);
        if (!_transcoderBusy)
            _ = Task.Run(RunTranscoding);

        return Result.Ok();
    }

    public void DeleteMovie(int movieId)
    {
        var directory = new DirectoryInfo(Path.Combine(Path.Combine(_uploadsPath, "hls"), $"mov_{movieId}"));
        var rawDirectoryInfo = new DirectoryInfo(Path.Combine(_uploadsPath, "raw"));
        var rawFileInfo = rawDirectoryInfo.GetFiles($"mov_{movieId}.*").FirstOrDefault();
        if (rawFileInfo is { Exists: true })
            rawFileInfo.Delete();

        if (directory.Exists)
            directory.Delete(true);
    }

    public Result<FileStream> GetPlaylistByMovieId(int movieId)
    {
        var playlistInfo = new FileInfo(Path.Combine(Path.Combine(_uploadsPath, "hls"), $"mov_{movieId}", "master.m3u8"));
        if (playlistInfo.Exists)
            return Result<FileStream>.Ok(playlistInfo.OpenRead());

        var directory = new DirectoryInfo(Path.Combine(_uploadsPath, "raw"));
        var files = directory.GetFiles($"mov_{movieId}.*");

        return Result<FileStream>.Error(
            files.Length > 0 ? "Video haven't transcoded to HLS yet" : "Video doesn't exist");
    }
}