using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueFournisseurProduitStock
    {
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("nb_produits_en_stock")] public long NbProduitsEnStock { get; set; }
    }
}
