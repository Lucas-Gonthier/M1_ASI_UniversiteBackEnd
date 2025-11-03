using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class EtudiantDto(long id, string numEtud, string nom, string prenom, string email)
{
    public long Id { get; set; } = id;
    public string NumEtud { get; set; } = numEtud;
    public string Nom { get; set; } = nom;
    public string Prenom { get; set; } = prenom;
    public string Email { get; set; } = email;

    public static EtudiantDto ToDto(Etudiant etudiant)
    {
        return new EtudiantDto(etudiant.EtudiantId, etudiant.NumEtud, etudiant.Nom, etudiant.Prenom, etudiant.Email);
    }

    public Etudiant ToEntity()
    {
        return new Etudiant { EtudiantId = Id, NumEtud = NumEtud, Nom = Nom, Prenom = Prenom, Email = Email };
    }
}