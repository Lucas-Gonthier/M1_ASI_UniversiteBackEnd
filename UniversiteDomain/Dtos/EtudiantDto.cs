using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class EtudiantDto(long id, string numEtud, string nom, string prenom, string email)
{
    public long Id { get; } = id;
    public string NumEtud { get; } = numEtud;
    public string Nom { get; } = nom;
    public string Prenom { get; } = prenom;
    public string Email { get; } = email;

    public static EtudiantDto ToDto(Etudiant etudiant)
    {
        return new EtudiantDto(etudiant.EtudiantId, etudiant.NumEtud, etudiant.Nom, etudiant.Prenom, etudiant.Email);
    }

    public Etudiant ToEntity()
    {
        return new Etudiant { EtudiantId = Id, NumEtud = NumEtud, Nom = Nom, Prenom = Prenom, Email = Email };
    }
}