using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurVille
    {
        [PostgreColumn("nomville")] public string NomVille { get; set; }
        [PostgreColumn("nb_vendeurs")] public long NbVendeurs { get; set; }
    }
}
