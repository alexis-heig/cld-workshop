using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueProduitNomPrix
    {
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("prixunitaire")] public decimal PrixUnitaire { get; set; }
    }
}
