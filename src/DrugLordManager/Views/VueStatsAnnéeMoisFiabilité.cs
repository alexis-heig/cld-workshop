using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueStatsAnnéeMoisFiabilité
    {
        [PostgreColumn("annee")] public decimal Année { get; set; }
        [PostgreColumn("mois")] public decimal Mois { get; set; }
        [PostgreColumn("faible")] public long Faible { get; set; }
        [PostgreColumn("moyen")] public long Moyen { get; set; }
        [PostgreColumn("fort")] public long Fort { get; set; }
    }
}
