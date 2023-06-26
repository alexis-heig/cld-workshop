using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStatsAnnéeMoisMontant
    {
        [PostgreColumn("annee")] public decimal Année { get; set; }
        [PostgreColumn("mois")] public decimal Mois { get; set; }
        [PostgreColumn("total_montant_paye")] public decimal Montant { get; set; }
    }
}
