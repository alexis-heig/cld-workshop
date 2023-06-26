using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStatsAnnéeMoisTransactions
    {
        [PostgreColumn("annee")] public decimal Année { get; set; }
        [PostgreColumn("mois")] public decimal Mois { get; set; }
        [PostgreColumn("nombre_transactions")] public long NbTransactions { get; set; }
    }
}
