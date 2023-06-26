using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurUnique
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("niveaufiabilite")] public string NiveauFiabilité { get; set; }
    }
}
