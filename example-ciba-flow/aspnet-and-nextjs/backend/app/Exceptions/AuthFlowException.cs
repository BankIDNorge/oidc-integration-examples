using ExampleAspnet.Models;

namespace ExampleAspnet.Exceptions;

[Serializable]
public class AuthFlowException : ExampleAppException
{
    public AuthFlowException() : base(ErrorResponse.AuthFlowError)
    {
    }
}