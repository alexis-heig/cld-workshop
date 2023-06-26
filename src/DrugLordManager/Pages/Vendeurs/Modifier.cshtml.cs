using DrugLordManager.Data;
using DrugLordManager.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrugLordManager.Pages.Vendeurs
{
    public class ModifierModel : PageModel
    {
        public VueVendeurUnique Vendeur { get; set; }
        public List<SelectListItem> NiveauxFiabilite { get; set; }

        public void OnGet(int idVendeur)
        {
            Vendeur = AccèsDonnées.Vendeur(idVendeur).First();
            NiveauxFiabilite = AccèsDonnées.NiveauxFiabilite()
                .Select(n => new SelectListItem(n.NiveauFiabilite, n.NiveauFiabilite))
                .ToList();
        }

        public IActionResult OnPost(int idVendeur, string niveauFiabilite)
        {
            AccèsDonnées.VendeurModifierNiveauFiabilité(idVendeur, niveauFiabilite);
            return Redirect("/Vendeurs");
        }
    }
}
