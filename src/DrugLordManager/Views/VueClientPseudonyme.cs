using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueClientPseudonyme
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
    }
}
