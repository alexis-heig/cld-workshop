using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurLocalité
    {
        [PostgreColumn("nomville")] public string NomVille { get; set; }
        [PostgreColumn("nomquartier")] public string NomQuartier { get; set; }
        [PostgreColumn("nb_vendeurs")] public long NbVendeurs { get; set; }
    }
}
