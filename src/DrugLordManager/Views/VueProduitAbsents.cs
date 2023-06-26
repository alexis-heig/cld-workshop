﻿using DrugLordManager.Attributes;

namespace DrugLordManager.Views
{
    public class VueProduitAbsents
    {
        [PostgreColumn("id")] public int Id { get; set; }
        [PostgreColumn("nom")] public string Nom { get; set; }
        [PostgreColumn("prixunitaire")] public decimal PrixUnitaire { get; set; }
    }
}
