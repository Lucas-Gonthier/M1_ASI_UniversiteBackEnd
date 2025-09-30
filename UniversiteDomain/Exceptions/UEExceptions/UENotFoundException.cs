namespace UniversiteDomain.Exceptions.UEExceptions;

[Serializable]
public class UENotFoundException : Exception
{
    public UENotFoundException() : base() { }
    public UENotFoundException(string message) : base(message) { }
    public UENotFoundException(string message, Exception inner) : base(message, inner) { }
}