using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueClientTransactions
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("nb_transactions")] public long NbTransactions { get; set; }
    }
}
