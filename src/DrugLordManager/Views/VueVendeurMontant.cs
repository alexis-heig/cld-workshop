using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurMontant
    {
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("montant_payé_moyen")] public decimal MontantMoyenPayé { get; set; }
    }
}
