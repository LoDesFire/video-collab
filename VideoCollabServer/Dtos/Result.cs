namespace VideoCollabServer.Dtos;

public class Result<T>
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
    public T? Value { get; set; }
    
    public static Result<T> Ok(T value)
    {
        return new Result<T>
        {
            Succeeded = true,
            Value = value
        };
    }
    
    public static Result<T> Error(string error)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = [error]
        };
    }
    
    public static Result<T> Error(IEnumerable<string> errors)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = errors
        };
    }
}

public class Result
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];

    public static Result Ok()
    {
        return new Result
        {
            Succeeded = true
        };
    }
    
    public static Result Error(string error)
    {
        return new Result
        {
            Succeeded = false,
            Errors = [error]
        };
    }
    
    public static Result Error(IEnumerable<string> errors)
    {
        return new Result
        {
            Succeeded = false,
            Errors = errors
        };
    }
}