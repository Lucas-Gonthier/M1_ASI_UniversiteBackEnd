namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class NoteOnUeNotInParcoursException : Exception
{
    public NoteOnUeNotInParcoursException()
    {
    }

    public NoteOnUeNotInParcoursException(string message) : base(message)
    {
    }

    public NoteOnUeNotInParcoursException(string message, Exception inner) : base(message, inner)
    {
    }
}