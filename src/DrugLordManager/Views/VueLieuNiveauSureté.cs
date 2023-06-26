using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueLieuNiveauSureté
    {
        [PostgreColumn("niveaudesûreté")] public string NiveauSureté { get; set; }
        [PostgreColumn("nb_lieux")] public long NbLieux { get; set; }
    }
}
