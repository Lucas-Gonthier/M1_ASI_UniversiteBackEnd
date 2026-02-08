namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class InvalidIntituleUeException : Exception
{
    public InvalidIntituleUeException()
    {
    }

    public InvalidIntituleUeException(string message) : base(message)
    {
    }

    public InvalidIntituleUeException(string message, Exception inner) : base(message, inner)
    {
    }
}
