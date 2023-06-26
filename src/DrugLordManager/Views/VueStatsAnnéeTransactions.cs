using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStatsAnnéeTransactions
    {
        [PostgreColumn("annee")] public decimal Année { get; set; }
        [PostgreColumn("nombre_transactions")] public long NbTransactions { get; set; }
    }
}
