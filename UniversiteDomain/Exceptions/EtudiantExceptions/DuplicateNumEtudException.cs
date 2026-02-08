namespace UniversiteDomain.Exceptions.EtudiantExceptions;

[Serializable]
public class DuplicateNumEtudException : Exception
{
    public DuplicateNumEtudException()
    {
    }

    public DuplicateNumEtudException(string message) : base(message)
    {
    }

    public DuplicateNumEtudException(string message, Exception inner) : base(message, inner)
    {
    }
}