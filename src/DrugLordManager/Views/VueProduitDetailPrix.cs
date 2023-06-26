using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueProduitDetailPrix
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("prixunitaire")] public decimal PrixUnitaire { get; set; }
        [PostgreColumn("prix_moyen")] public decimal PrixMoyen { get; set; }
    }
}
