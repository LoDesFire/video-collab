using System.Diagnostics;
using VideoCollabServer.Dtos.Movie;
using VideoCollabServer.Interfaces;

namespace VideoCollabServer.Services;

public class HlsService : IHlsService
{
    private readonly Dictionary<string, string?> _contentType = new()
    {
        { ".ts", "video/MP2T" },
        { ".m3u8", "application/x-mpegURL" }
    };
    
    private readonly List<string> _supportedVideoExtensions = new()
    {
        "mp4", "mov"
    };
    
    public async  Task<UploadMovieDto> UploadMovieAsync(int movieId, IFormFile file, IMovieRepository movieRepository)
    {
        var extension = file.FileName.Split(".").Last();

        if (!_supportedVideoExtensions.Contains(extension))
            return new UploadMovieDto
            {
                Error = "Unsupported video file extension",
                Succeeded = false
            };

        if (!await movieRepository.ContainsMovieAsync(movieId))
            return new UploadMovieDto
            {
                Error = "Movie doesn't exist",
                Succeeded = false
            };

        var fileName = $"mov_{movieId}.{extension}";

        await using (var fs = File.Create($"uploads/raw/{fileName}"))
        {
            await file.CopyToAsync(fs);
        }

        TranscodeMovie($"uploads/raw/{fileName}"); // TODO: Add transcoding to queue

        return new UploadMovieDto { Succeeded = true };
    }

    public void DeleteMovie(int movieId)
    {
        var directory = new DirectoryInfo($"uploads/hls/mov_{movieId}");
        var rawDirectoryInfo = new DirectoryInfo("uploads/raw/");
        var rawFileInfo = rawDirectoryInfo.GetFiles($"mov_{movieId}.*").First();
        if (rawFileInfo.Exists) 
            rawFileInfo.Delete();
        
        if (directory.Exists)
            directory.Delete(true);
    }

    public PlaylistDto GetPlaylistByMovieId(int movieId)
    {
        var playlistInfo = new FileInfo($"uploads/hls/mov_{movieId}/master.m3u8");
        if (playlistInfo.Exists)
            return new PlaylistDto
            {
                Exists = true,
                Stream = playlistInfo.OpenRead()
            };

        var directory = new DirectoryInfo("uploads/raw");
        var files = directory.GetFiles($"mov_{movieId}.*");

        return new PlaylistDto
        {
            Exists = false,
            Error = files.Length > 0 ? "Video haven't transcoded to HLS yet" : "Video doesn't exist"
        };
    }

    public void TranscodeMovie(string path)
    {
        // TODO: Add supporting of different 'ffmpeg' paths. 
        // TODO: Add resolutions mutable parameter
        var fileInfo = new FileInfo(path);
        var directoryOutput = $"uploads/hls/{fileInfo.Name.Split('.')[0]}";
        const string shellFile = "ShellScripts/ffmpeg.sh";

        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = $"{shellFile}",
            Arguments = $"{path} {directoryOutput}",
            RedirectStandardOutput = true,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        process.Start();

        // process.OutputDataReceived += (sender, args) =>
        // {
        //     Debug.WriteLine(args.Data);
        // };
        // TODO: Add Transcoding progress
    }

    public HlsFileDto GetHlsFile(int movieId, string quality, string file)
    {
        var fileInfo = new FileInfo($"uploads/hls/mov_{movieId}/{quality}/{file}");

        if (!fileInfo.Exists || !_contentType.TryGetValue(fileInfo.Extension, out var contentType))
            return new HlsFileDto
            {
                Succeeded = false
            };

        return new HlsFileDto
        {
            Succeeded = true,
            Stream = fileInfo.OpenRead(),
            ContentType = contentType
        };
    }
}