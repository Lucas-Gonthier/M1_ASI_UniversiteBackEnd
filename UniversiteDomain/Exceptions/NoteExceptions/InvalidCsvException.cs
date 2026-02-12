namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidCsvException : Exception
{
    public List<string> Erreurs { get; }

    public InvalidCsvException(List<string> erreurs)
        : base("Le fichier CSV contient des erreurs de validation")
    {
        Erreurs = erreurs;
    }
}
