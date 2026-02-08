namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class InvalidAnneeFormationException : Exception
{
    public InvalidAnneeFormationException()
    {
    }

    public InvalidAnneeFormationException(string message) : base(message)
    {
    }

    public InvalidAnneeFormationException(string message, Exception inner) : base(message, inner)
    {
    }
}