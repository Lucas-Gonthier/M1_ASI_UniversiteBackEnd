namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class DuplicateNumeroUeException : Exception
{
    public DuplicateNumeroUeException()
    {
    }

    public DuplicateNumeroUeException(string message) : base(message)
    {
    }

    public DuplicateNumeroUeException(string message, Exception inner) : base(message, inner)
    {
    }
}
