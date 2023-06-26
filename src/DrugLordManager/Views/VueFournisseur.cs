using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueFournisseur
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
    }
}
