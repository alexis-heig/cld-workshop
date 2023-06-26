using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{

    public class VueClient
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
    }
}
