using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStatsAnnéeMoisPrix
    {
        [PostgreColumn("annee")] public decimal Année { get; set; }
        [PostgreColumn("mois")] public decimal Mois { get; set; }
        [PostgreColumn("total_prix")] public decimal Prix { get; set; }
    }
}
