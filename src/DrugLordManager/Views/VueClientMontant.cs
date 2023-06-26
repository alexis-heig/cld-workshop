using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueClientMontant
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("montant_moyen")] public decimal MontantMoyen { get; set; }
    }
}
