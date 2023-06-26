using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueTransactionVilleQuartier
    {
        [PostgreColumn("nom_ville")] public string NomVille { get; set; }
        [PostgreColumn("nom_quartier")] public string NomQuartier { get; set; }
        [PostgreColumn("nbTransactions")] public long NbTransactions { get; set; }
    }
}
