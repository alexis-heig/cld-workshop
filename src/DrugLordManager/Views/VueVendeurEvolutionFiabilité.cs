using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurEvolutionFiabilité
    {
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("niveaufiabilité")] public string NiveauFiabilité { get; set; }
        [PostgreColumn("datedebut")] public DateTime DateDebut { get; set; }
    }
}
