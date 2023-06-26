using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueLieuVille
    {
        [PostgreColumn("nom_ville")] public string NomVille { get; set; }
        [PostgreColumn("nb_lieux")] public long NbLieux { get; set; }
    }
}
