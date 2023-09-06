using ExampleAspnet.Models;

namespace ExampleAspnet.Exceptions;

[Serializable]
public class InvalidSessionException : ExampleAppException
{
    public InvalidSessionException() : base(ErrorResponse.InvalidSession)
    {
    }
}