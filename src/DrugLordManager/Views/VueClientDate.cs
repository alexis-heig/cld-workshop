using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueClientDate
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("pseudonyme")] public string Pseudonyme { get; set; }
        [PostgreColumn("date")] public DateTime Date { get; set; }
    }
}
