namespace UniversiteDomain.Exceptions.ParcoursExceptions;

[Serializable]
public class DuplicateParcoursIdException : Exception
{
    public DuplicateParcoursIdException()
    {
    }

    public DuplicateParcoursIdException(string message) : base(message)
    {
    }

    public DuplicateParcoursIdException(string message, Exception inner) : base(message, inner)
    {
    }
}