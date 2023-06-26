using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueTransactionTypeProduit
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("nb_transactions")] public long NbTransactions { get; set; }
    }
}
