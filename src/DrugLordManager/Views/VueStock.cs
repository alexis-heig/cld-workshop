using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStock
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("prixunitaire")] public decimal PrixUnitaire { get; set; }
        [PostgreColumn("quantité")] public decimal Quantité { get; set; }
        [PostgreColumn("qualité")] public string Qualité { get; set; }
        [PostgreColumn("datearrivée")] public DateTime DateArrivée { get; set; }
        [PostgreColumn("pseudo_fournisseur")] public string PseudoFournisseur { get; set; }
    }
}
