namespace UniversiteDomain.Exceptions.NoteExceptions;

// NoteOnUeNotInParcoursException

public class InvalidNoteValeurException : Exception
{
    public InvalidNoteValeurException() : base()
    {
    }
    
    public InvalidNoteValeurException(string message) : base(message)
    {
    }

    public InvalidNoteValeurException(string message, Exception inner) : base(message, inner)
    {
    }
}