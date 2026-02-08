namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class DuplicateUeDansParcoursException : Exception
{
    public DuplicateUeDansParcoursException()
    {
    }

    public DuplicateUeDansParcoursException(string message) : base(message)
    {
    }

    public DuplicateUeDansParcoursException(string message, Exception inner) : base(message, inner)
    {
    }
}