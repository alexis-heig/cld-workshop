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
            { "default", Acc�sDonn�es.Transactions},
            { "statut", Acc�sDonn�es.TransactionsTriStatut },
            { "date", Acc�sDonn�es.TransactionsTriDate },
            { "lieuxville", Acc�sDonn�es.TransactionsTriLieuxEtVille },
            { "vendeur", Acc�sDonn�es.TransactionsTriVendeur },
            { "client", Acc�sDonn�es.TransactionsTriClient },
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
