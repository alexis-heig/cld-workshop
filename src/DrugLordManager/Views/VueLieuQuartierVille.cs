using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueLieuQuartierVille
    {
        [PostgreColumn("nom_ville")] public string NomVille { get; set; }
        [PostgreColumn("nom_quartier")] public string NomQuartier { get; set; }
        [PostgreColumn("nb_lieux")] public long NbLieux { get; set; }
    }
}
