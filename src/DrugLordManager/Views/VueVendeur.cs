using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{

    public class VueVendeur
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("statut")] public string Statut { get; set; }
        [PostgreColumn("niveaufiabilite")] public string NiveauFiabilité { get; set; }
        [PostgreColumn("nomquartier")] public string NomQuartier { get; set; }
        [PostgreColumn("nomville")] public string NomVille { get; set; }
    }
}
