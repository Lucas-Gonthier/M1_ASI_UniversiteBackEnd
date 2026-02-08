namespace UniversiteDomain.Exceptions.EtudiantExceptions;

[Serializable]
public class InvalidPrenomEtudiantException : Exception
{
    public InvalidPrenomEtudiantException()
    {
    }

    public InvalidPrenomEtudiantException(string message) : base(message)
    {
    }

    public InvalidPrenomEtudiantException(string message, Exception inner) : base(message, inner)
    {
    }
}
