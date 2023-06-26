using DrugLordManager.Data;
using DrugLordManager.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrugLordManager.Pages.Vendeurs
{
    public class IndexModel : PageModel
    {
        public List<VueVendeur> Vendeurs { get; set; }

        private static Dictionary<string, Func<List<VueVendeur>>> s_tris = new Dictionary<string, Func<List<VueVendeur>>>
        {
            { "default", Acc�sDonn�es.Vendeurs},
            { "pseudo", Acc�sDonn�es.VendeursTriPseudonyme },
            { "quartierville", Acc�sDonn�es.VendeursTriQuartierEtVille },
        };

        public void OnGet(string tri)
        {

            if (!string.IsNullOrEmpty(tri) && s_tris.ContainsKey(tri))
            {
                Vendeurs = s_tris[tri]();
                return;
            }

            Vendeurs = s_tris["default"]();
        }
    }
}
