namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class UeNotFoundException : Exception
{
    public UeNotFoundException()
    {
    }

    public UeNotFoundException(string message) : base(message)
    {
    }

    public UeNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}