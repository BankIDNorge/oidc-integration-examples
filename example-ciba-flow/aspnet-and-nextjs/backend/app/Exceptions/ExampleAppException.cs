using ExampleAspnet.Models;

namespace ExampleAspnet.Exceptions;

public abstract class ExampleAppException : Exception
{
    public ErrorResponse ErrorResponse { get; init; }

    protected ExampleAppException(string message, ErrorResponse errorResponse) : base(message)
    {
        ErrorResponse = errorResponse;
    }

    protected ExampleAppException(ErrorResponse errorResponse) : base(errorResponse.ErrorDescription)
    {
        ErrorResponse = errorResponse;
    }
}