using DrugLordManager.Data;
using DrugLordManager.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrugLordManager.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        public List<VueTransaction> Transactions { get; set; }

        private static Dictionary<string, Func<List<VueTransaction>>> s_tris = new Dictionary<string, Func<List<VueTransaction>>>
        {
            { "default", AccèsDonnées.Transactions},
            { "statut", AccèsDonnées.TransactionsTriStatut },
            { "date", AccèsDonnées.TransactionsTriDate },
            { "lieuxville", AccèsDonnées.TransactionsTriLieuxEtVille },
            { "vendeur", AccèsDonnées.TransactionsTriVendeur },
            { "client", AccèsDonnées.TransactionsTriClient },
        };

        public void OnGet(string tri)
        {

            if (!string.IsNullOrEmpty(tri) && s_tris.ContainsKey(tri))
            {
                Transactions = s_tris[tri]();
                return;
            }

            Transactions = s_tris["default"]();
        }
    }
}
