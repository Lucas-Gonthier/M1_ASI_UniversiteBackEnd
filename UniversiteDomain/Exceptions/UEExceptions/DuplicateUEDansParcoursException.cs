namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class DuplicateUEDansParcoursException : Exception
{
    public DuplicateUEDansParcoursException() : base() { }
    public DuplicateUEDansParcoursException(string message) : base(message) { }
    public DuplicateUEDansParcoursException(string message, Exception inner) : base(message, inner) { }
}