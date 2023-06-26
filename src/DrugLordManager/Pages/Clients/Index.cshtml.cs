using DrugLordManager.Data;
using DrugLordManager.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrugLordManager.Pages.Clients
{
    public class IndexModel : PageModel
    {
        public List<VueClient> Clients { get; set; }
        public void OnGet()
        {
            Clients = AccèsDonnées.Clients();
        }
    }
}
