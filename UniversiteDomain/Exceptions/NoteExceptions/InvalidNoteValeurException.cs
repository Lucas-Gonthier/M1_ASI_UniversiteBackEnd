namespace UniversiteDomain.Exceptions.NoteExceptions;

[Serializable]
public class InvalidNoteValeurException : Exception
{
    public InvalidNoteValeurException(string message) : base(message)
    {
    }

    public InvalidNoteValeurException(string message, Exception inner) : base(message, inner)
    {
    }
}