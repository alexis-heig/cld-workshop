using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueLieu
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("nomLieu")] public string NomLieu { get; set; }
        [PostgreColumn("nomVille")] public string NomVille { get; set; }
        [PostgreColumn("nomQuartier")] public string NomQuartier { get; set; }
        [PostgreColumn("niveaudesûreté")] public string NiveauSureté { get; set; }
    }
}
