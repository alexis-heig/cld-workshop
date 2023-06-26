using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueTransactionVille
    {
        [PostgreColumn("nomville")] public string NomVille { get; set; }
        [PostgreColumn("nb_transactions")] public long NbTransactions { get; set; }
    }
}
