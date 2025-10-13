namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class DuplicateNoteOnUeException : Exception
{
    public DuplicateNoteOnUeException() : base()
    {
    }

    public DuplicateNoteOnUeException(string message) : base(message)
    {
    }

    public DuplicateNoteOnUeException(string message, Exception inner) : base(message, inner)
    {
    }
}