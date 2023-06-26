using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueNiveauxFiabilite
    {
        [PostgreColumn("niveaux_fiabilites")] public string NiveauFiabilite { get; set; }
    }
}
