using ExampleAspnet.Models;

namespace ExampleAspnet.Exceptions;

[Serializable]
public class GrantTimeoutException : ExampleAppException
{
    public GrantTimeoutException() : base(ErrorResponse.GrantTimeout)
    {
    }
}