using DrugLordManager.Data;
using DrugLordManager.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrugLordManager.Pages.Lieux
{
    public class IndexModel : PageModel
    {
        public List<VueLieu> Lieux { get; set; }

        private static Dictionary<string, Func<List<VueLieu>>> s_tris = new Dictionary<string, Func<List<VueLieu>>>
        {
            { "default", AccèsDonnées.Lieux},
            { "surete", AccèsDonnées.LieuxTriNiveauSureté},
        };

        public void OnGet(string tri)
        {
            if (!string.IsNullOrEmpty(tri) && s_tris.ContainsKey(tri))
            {
                Lieux = s_tris[tri]();
                return;
            }

            Lieux = s_tris["default"]();
        }
    }
}
