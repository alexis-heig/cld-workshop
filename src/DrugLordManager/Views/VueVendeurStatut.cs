using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueVendeurStatut
    {
        [PostgreColumn("statut")] public string Statut { get; set; }
        [PostgreColumn("nb_vendeurs")] public long NbVendeurs { get; set; }
    }
}
