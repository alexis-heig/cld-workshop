using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueTransaction
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("idclient")] public int IdClient { get; set; }
        [PostgreColumn("idvendeur")] public int IdVendeur { get; set; }
        [PostgreColumn("idlieu")] public int IdLieu { get; set; }
        [PostgreColumn("date")] public DateTime Date { get; set; }
        [PostgreColumn("qualitédéroulement")] public string QualitéDéroulement { get; set; }
        [PostgreColumn("remarque")] public string? Remarque { get; set; }
        [PostgreColumn("prix")] public decimal Prix { get; set; }
        [PostgreColumn("montantpayé")] public decimal MontantPayé { get; set; }
        [PostgreColumn("statut")] public string Statut { get; set; }
        [PostgreColumn("nomvendeur")] public string NomVendeur { get; set; }
        [PostgreColumn("nomclient")] public string NomClient { get; set; }
        [PostgreColumn("nomlieu")] public string NomLieu { get; set; }
        [PostgreColumn("nomville")] public string NomVille { get; set; }
    }
}
