using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueProduitFuturs
    {
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("quantité")] public decimal Quantité { get; set; }
        [PostgreColumn("datearrivée")] public DateTime DateArrivée { get; set; }
    }
}
