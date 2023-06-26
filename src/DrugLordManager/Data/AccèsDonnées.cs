using DrugLordManager.Attributes;
using DrugLordManager.Views;
using Npgsql;
using System.Diagnostics;
using System.Reflection;

namespace DrugLordManager.Data
{
    /// <summary>
    /// Cette classe permet l'accès aux données SQL via une connexion avec le SGBD PostgreSQL.
    /// Elle est utilisée conjointement avec toutes les classes définies dans le dossier Views qui
    /// sont des structures de données représentant les résultats des différentes requêtes SQL.
    /// Attention: le terme 'Vue' utilisé dans ce document ne correspond pas au terme de vue SQL.
    /// </summary>
    public class AccèsDonnées
    {
        public static string? SqlConnectionString;

        // Autre
        public static List<VueNiveauxFiabilite> NiveauxFiabilite()
        {
            string requete =
                "SELECT unnest(enum_range(NULL::NIVEAU_FIABILITE))::text " +
                "AS niveaux_fiabilites;";
            return EnvoyerRequeteSelect<VueNiveauxFiabilite>(requete);
        }

        // Vendeurs
        public static List<VueVendeur> Vendeurs() // V1
        {
            string requete = 
                "SELECT vVendeur.* " +
                "FROM vVendeur;";
            return EnvoyerRequeteSelect<VueVendeur>(requete);
        }

        public static List<VueVendeur> VendeursTriPseudonyme() // V2
        {
            string requete = 
                "SELECT vVendeur.* " +
                "FROM vVendeur " +
                "ORDER BY vVendeur.pseudonyme;";
            return EnvoyerRequeteSelect<VueVendeur>(requete);
        }

        public static List<VueVendeur> VendeursTriQuartierEtVille() // V3
        {
            string requete = 
                "SELECT vVendeur.* " +
                "FROM vVendeur " +
                "ORDER BY vVendeur.nomVille, vVendeur.nomQuartier;";
            return EnvoyerRequeteSelect<VueVendeur>(requete);
        }

        public static List<VueVendeurLocalité> VendeursNombreParVilleEtQuartier() // V4
        {
            string requete = 
                "SELECT Ville.nom as nomVille, Quartier.nom as nomQuartier, " +
                "COUNT(*) as nb_vendeurs " +
                "FROM Vendeur " +
                "INNER JOIN Quartier ON Vendeur.nomQuartier = Quartier.nom " +
                "INNER JOIN Ville ON Vendeur.idVille = Ville.id " +
                "GROUP BY Ville.nom, Quartier.nom " +
                "ORDER BY nb_vendeurs DESC;";
            return EnvoyerRequeteSelect<VueVendeurLocalité>(requete);
        }

        public static List<VueVendeurVille> VendeursNombreParVille() // V5
        {
            string requete = 
                "SELECT Ville.nom as nomVille, COUNT(*) as nb_vendeurs " +
                "FROM Vendeur " +
                "INNER JOIN Ville ON Vendeur.idVille = Ville.id " +
                "GROUP BY Ville.nom " +
                "ORDER BY nb_vendeurs DESC;";
            return EnvoyerRequeteSelect<VueVendeurVille>(requete);
        }

        public static List<VueVendeur> VendeursDisponiblesAujourdhui() // V6
        {
            string requete =
                "SELECT DISTINCT vVendeur.* " +
                "FROM vVendeur " +
                "INNER JOIN Transaction ON Transaction.idVendeur = vVendeur.id " +
                "WHERE Transaction.date <> NOW() AND vVendeur.statut = 'en vie';";
            return EnvoyerRequeteSelect<VueVendeur>(requete);
        }

        public static List<VueVendeurStatut> VendeursNombreParStatut() // V7
        {
            string requete = 
                "SELECT Vendeur.statut, COUNT(Vendeur.idPersonnePhysique) as nb_vendeurs " +
                "FROM Vendeur " +
                "GROUP BY Vendeur.statut;";
            return EnvoyerRequeteSelect<VueVendeurStatut>(requete);
        }

        public static List<VueVendeurMontant> VendeurMontantMoyenTransactions(int idVendeur) // V_U1
        {
            string requete =
                "SELECT Personne.pseudonyme, coalesce(ROUND(AVG(Transaction.montantPayé), 2), 0) AS montant_payé_moyen " +
                "FROM Transaction " +
                "RIGHT JOIN Personne ON Personne.id = Transaction.idVendeur " +
                $"WHERE Personne.id = {idVendeur} " +
                "GROUP BY Personne.pseudonyme;";
            return EnvoyerRequeteSelect<VueVendeurMontant>(requete);
        }

        public static List<VueVendeurEvolutionFiabilité> VendeurEvolutionNiveauFiabilité(int idVendeur) // V_U2
        {
            string requete = 
                "SELECT Personne.pseudonyme, HistoriqueNiveauFiabilite.niveauFiabilité, HistoriqueNiveauFiabilite.dateDebut " +
                "FROM HistoriqueNiveauFiabilite " +
                "INNER JOIN Personne ON Personne.id = HistoriqueNiveauFiabilite.idPersonnePhysique " +
                $"WHERE Personne.id = {idVendeur} " +
                "ORDER BY HistoriqueNiveauFiabilite.dateDebut;";
            return EnvoyerRequeteSelect<VueVendeurEvolutionFiabilité>(requete);
        }

        public static List<VueVendeurUnique> Vendeur(int idVendeur)
        {
            string requete =
                "SELECT vVendeur.id, vVendeur.pseudonyme, vVendeur.niveaufiabilite " +
                "FROM vVendeur " +
                $"WHERE vVendeur.id = {idVendeur};";
            return EnvoyerRequeteSelect<VueVendeurUnique>(requete);
        }

        public static void VendeurModifierNiveauFiabilité(int idVendeur, string niveauFiabilité)
        {
            string requete =
                "UPDATE PersonnePhysique " +
                $"SET niveauFiabilite = '{niveauFiabilité}' " +
                $"WHERE idPersonne = {idVendeur}";
            EnvoyerRequeteUpdate(requete);
        }

        // Fournisseurs & stocks
        public static List<VueFournisseur> Fournisseurs() // FS_1
        {
            string requete =
                "SELECT Fournisseur.idPersonne AS id, Personne.pseudonyme " +
                "FROM Fournisseur " +
                "INNER JOIN Personne ON Fournisseur.idPersonne = Personne.id;";
            return EnvoyerRequeteSelect<VueFournisseur>(requete);
        }

        public static List<VueStock> Stocks() // FS_2
        {
            string requete =
                "SELECT Stock.id, produit.nom, Produit.prixUnitaire, " +
                "Stock.quantité, Stock.qualité, Stock.dateArrivée, " +
                "Personne.pseudonyme as pseudo_fournisseur " +
                "FROM Stock " +
                "INNER JOIN Produit ON Stock.idProduit = Produit.id " +
                "INNER JOIN Personne" +
                "    ON Stock.idFournisseur = Personne.id;";
            return EnvoyerRequeteSelect<VueStock>(requete);
        }

        public static List<VueTransactionTypeProduit> TransactionsNombreParTypesDeProduit() // FS_3
        {
            string requete =
                "SELECT Produit.id, Produit.nom, COUNT(*) as nb_transactions " +
                "FROM Transaction_stock " +
                "INNER JOIN Stock ON Transaction_stock.idStock = Stock.id " +
                "INNER JOIN Produit ON Stock.idProduit = Produit.id " +
                "GROUP BY Produit.id, Produit.nom " +
                "ORDER BY nb_transactions DESC;";
            return EnvoyerRequeteSelect<VueTransactionTypeProduit>(requete);
        }

        public static List<VueProduitFuturs> StockProduitsFuturs() // FS_4
        {
            string requete =
                "SELECT Produit.nom, Stock.quantité, Stock.dateArrivée " +
                "FROM Stock " +
                "INNER JOIN Produit ON Produit.id = Stock.idProduit " +
                "WHERE Stock.dateArrivée > NOW();";
            return EnvoyerRequeteSelect<VueProduitFuturs>(requete);
        }

        public static List<VueFournisseurProduitStock> StockMeilleursFournisseursSelonProduitsPrésent() // FS_5
        {
            string requete =
                "SELECT Personne.pseudonyme, COUNT(*) AS nb_produits_en_stock " +
                "FROM Stock " +
                "INNER JOIN Personne ON Personne.id = Stock.idFournisseur " +
                "GROUP BY Personne.pseudonyme " +
                "HAVING COUNT(*) >= ALL(" +
                "   SELECT COUNT(*) FROM Stock " +
                "   INNER JOIN Personne ON Personne.id = Stock.idFournisseur " +
                "   GROUP BY Personne.pseudonyme);";
            return EnvoyerRequeteSelect<VueFournisseurProduitStock>(requete);
        }

        public static List<VueProduitAbsents> StockProduitsAbsents() // FS_6
        {
            string requete =
                "SELECT * FROM Produit " +
                "WHERE id NOT IN (SELECT idProduit FROM Stock);";
            return EnvoyerRequeteSelect<VueProduitAbsents>(requete);
        }

        // Produits
        public static List<VueProduit> Produits() // P_1
        {
            string requete =
                "SELECT * " +
                "FROM Produit;";
            return EnvoyerRequeteSelect<VueProduit>(requete);
        }

        public static List<VueProduit> ProduitsPrixInferieurAuPrixMoyen() // P_2
        {
            string requete =
                "SELECT * " +
                "FROM Produit " +
                "WHERE prixUnitaire < " +
                "(SELECT AVG(prixUnitaire) FROM Produit);";
            return EnvoyerRequeteSelect<VueProduit>(requete);
        }

        public static List<VueProduit> ProduitsPrixSuperieurAuPrixMoyen() // P_3
        {
            string requete =
                "SELECT * " +
                "FROM Produit " +
                "WHERE prixUnitaire > " +
                "(SELECT AVG(prixUnitaire) FROM Produit);";
            return EnvoyerRequeteSelect<VueProduit>(requete);
        }

        public static List<VueProduitNomPrix> ProduitsJamaisVendus() // P_4
        {
            string requete =
                "SELECT * " +
                "FROM Produit " +
                "WHERE NOT EXISTS(" +
                "    SELECT * FROM Transaction_stock" +
                "    INNER JOIN Stock ON Transaction_stock.idStock = Stock.id" +
                "    INNER JOIN Produit AS P2 ON Stock.idProduit = P2.id AND" +
                "    P2.id = Produit.id);";
            return EnvoyerRequeteSelect<VueProduitNomPrix>(requete);
        }

        public static List<VueProduitDetailPrix> ProduitsSelonPrixMoyen() // P_5
        {
            string requete =
                "WITH PrixMoyenParProduit AS (" +
                "SELECT round(AVG(prixUnitaire), 2) AS prix_moyen " +
                "FROM Produit " +
                ") " +
                "SELECT Produit.id, Produit.nom, Produit.prixUnitaire, " +
                "PrixMoyenParProduit.prix_moyen " +
                "FROM Produit " +
                "CROSS JOIN PrixMoyenParProduit " +
                "ORDER BY Produit.prixUnitaire;";
            return EnvoyerRequeteSelect<VueProduitDetailPrix>(requete);
        }

        // Clients
        public static List<VueClient> Clients() // C_1
        {
            string requete =
                "SELECT * " +
                "FROM vClient;";
            return EnvoyerRequeteSelect<VueClient>(requete);
        }

        public static List<VueClientPseudonyme> ClientsFiabiliteFaible() // C_2
        {
            string requete =
                "SELECT vClient.id, Personne.pseudonyme " +
                "FROM vClient " +
                "INNER JOIN PersonnePhysique ON vClient.id = PersonnePhysique.idPersonne " +
                "INNER JOIN Personne ON vClient.id = Personne.id " +
                "WHERE PersonnePhysique.niveauFiabilite = 'faible';";
            return EnvoyerRequeteSelect<VueClientPseudonyme>(requete);
        }

        public static List<VueClientTransactions> ClientsMeilleurs() // C_3
        {
            string requete =
                "SELECT vClient.*, COUNT(*) AS nb_transactions " +
                "FROM vClient " +
                "INNER JOIN Transaction ON vClient.id = Transaction.idClient  " +
                "GROUP BY vClient.id, vClient.pseudonyme, vClient.niveaufiabilite " +
                "HAVING COUNT(*) >= ALL(" +
                "   SELECT COUNT(*) FROM vClient " +
                "   INNER JOIN Transaction ON vClient.id = Transaction.idClient " +
                "   GROUP BY vClient.id, vClient.pseudonyme);";
            return EnvoyerRequeteSelect<VueClientTransactions>(requete);
        }

        public static List<VueClientTransactions> ClientsPires() // C_4
        {
            string requete =
                "SELECT vClient.*, COUNT(*) AS nb_transactions " +
                "FROM vClient " +
                "INNER JOIN Transaction ON vClient.id = Transaction.idClient " +
                "GROUP BY vClient.id, vClient.pseudonyme, vClient.niveaufiabilite " +
                "HAVING COUNT(*) <= ALL(" +
                "   SELECT COUNT(*) FROM vClient " +
                "   INNER JOIN Transaction ON vClient.id = Transaction.idClient " +
                "   GROUP BY vClient.id, vClient.pseudonyme);";
            return EnvoyerRequeteSelect<VueClientTransactions>(requete);
        }

        public static List<VueClientDate> ClientsFideles() // C_5
        {
            string requete =
                "SELECT vClient.*, Transaction.date " +
                "FROM vClient " +
                "INNER JOIN Transaction ON vClient.id = Transaction.idClient " +
                "WHERE Transaction.date = " +
                "(SELECT MIN(date) FROM Transaction);";
            return EnvoyerRequeteSelect<VueClientDate>(requete);
        }

        public static List<VueClientPseudonyme> ClientsNouveaux() // C_6
        {
            string requete =
                "SELECT Client.idPersonnePhysique as id, Personne.pseudonyme " +
                "FROM Client " +
                "INNER JOIN Personne ON Client.idPersonnePhysique = Personne.id " +
                "WHERE Client.idPersonnePhysique NOT IN(" +
                "    SELECT Transaction.idClient" +
                "    FROM Transaction " +
                "    INNER JOIN Personne ON Transaction.idClient = Personne.id" +
                "    WHERE EXTRACT(month FROM Transaction.date) <> EXTRACT(month FROM Now()) " +
                "      OR EXTRACT(year FROM Transaction.date) <> EXTRACT(year FROM Now())" +
                ");";
            return EnvoyerRequeteSelect<VueClientPseudonyme>(requete);
        }

        public static List<VueClientMontant> ClientsSelonMontantTransactions() // C_7
        {
            string requete =
                "SELECT vClient.*, COALESCE(ROUND(AVG(Transaction.montantPayé),2),0) as montant_moyen " +
                "FROM vClient " +
                "LEFT JOIN Transaction ON vClient.id = Transaction.idClient " +
                "GROUP BY vClient.id, vClient.pseudonyme, vClient.niveaufiabilite " +
                "ORDER BY montant_moyen DESC;";
            return EnvoyerRequeteSelect<VueClientMontant>(requete);
        }

        public static List<VueClientTransactions> ClientsSelonNombresTransactions() // C_8
        {
            string requete =
                "SELECT vClient.*, COUNT(Transaction.id) as nb_transactions " +
                "FROM vClient " +
                "LEFT JOIN Transaction ON vClient.id = Transaction.idClient " +
                "GROUP BY vClient.id, vClient.pseudonyme, vClient.niveaufiabilite " +
                "ORDER BY nb_transactions DESC;";
            return EnvoyerRequeteSelect<VueClientTransactions>(requete);
        }

        public static List<VueClientDate> ClientsMauvaisesTransactionsLorsDuMois() // C_9
        {
            string requete =
                "SELECT vClient.*, Transaction.date " +
                "FROM vClient " +
                "INNER JOIN Transaction ON Transaction.idClient = vClient.id " +
                "WHERE Transaction.qualitéDéroulement='très mauvais' AND " +
                "EXTRACT(month FROM Transaction.date) = EXTRACT(month FROM Now()) AND " +
                "EXTRACT(year FROM Transaction.date) = EXTRACT(year FROM Now());";
            return EnvoyerRequeteSelect<VueClientDate>(requete);
        }

        // Transactions
        public static List<VueTransaction> Transactions() // T_1
        {
            string requete =
                "SELECT * " +
                "FROM vTransaction;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsTriStatut() // T_2
        {
            string requete =
                "SELECT * FROM vTransaction " +
                "ORDER BY vTransaction.statut DESC;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsTriDate() // T_3
        {
            string requete =
                "SELECT * FROM vTransaction " +
                "ORDER BY vTransaction.date;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsTriLieuxEtVille() // T_4
        {
            string requete =
                "SELECT * FROM vTransaction " +
                "ORDER BY vTransaction.nomVille, vTransaction.nomLieu;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsTriVendeur() // T_5
        {
            string requete =
                "SELECT * FROM vTransaction " +
                "ORDER BY vTransaction.nomVendeur;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsTriClient() // T_6
        {
            string requete =
                "SELECT * FROM vTransaction " +
                "ORDER BY vTransaction.nomClient;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransactionVille> TransactionsParVille() // T_7
        {
            string requete =
                "SELECT vTransaction.nomVille, COUNT(*) as nb_transactions " +
                "FROM vTransaction " +
                "GROUP BY vTransaction.nomVille " +
                "ORDER BY nb_transactions DESC;";
            return EnvoyerRequeteSelect<VueTransactionVille>(requete);
        }

        public static List<VueTransactionVilleQuartier> TransactionsParQuartierEtVille() // T_8
        {
            string requete =
                "SELECT Ville.nom as nom_ville, Quartier.nom as nom_quartier, COUNT(Transaction.idLieu) as NbTransactions " +
                "FROM Transaction " +
                "INNER JOIN Lieu ON Lieu.id = Transaction.idLieu " +
                "INNER JOIN Quartier ON Lieu.nomQuartier = Quartier.nom " +
                "INNER JOIN Ville ON Lieu.idVille = Ville.id " +
                "GROUP BY nom_ville, nom_quartier " + 
                "ORDER BY NbTransactions DESC;";
            return EnvoyerRequeteSelect<VueTransactionVilleQuartier>(requete);
        }

        public static List<VueTransaction> TransactionsCetteAnnée() // T_9
        {
            string requete =
                "SELECT vTransaction.* " +
                "FROM vTransaction " +
                "WHERE vTransaction.date BETWEEN " +
                "date_trunc('year', now()) AND CURRENT_TIMESTAMP;";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsParLieu(int idLieu) // T_U1
        {
            string requete =
                "SELECT * FROM vTransaction " +
                $"WHERE vTransaction.idLieu = {idLieu};";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsParClient(int idClient) // T_U2
        {
            string requete =
                "SELECT * FROM vTransaction " +
                $"WHERE vTransaction.idClient = {idClient};";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        public static List<VueTransaction> TransactionsParVendeur(int idVendeur) // T_U3
        {
            string requete =
                "SELECT * FROM vTransaction " +
                $"WHERE vTransaction.idVendeur = {idVendeur};";
            return EnvoyerRequeteSelect<VueTransaction>(requete);
        }

        // Lieux
        public static List<VueLieu> Lieux() // L_1
        {
            string requete =
                "SELECT Lieu.id, Lieu.nom AS nomLieu, Quartier.nom AS nomQuartier, Ville.nom AS nomVille, Lieu.niveauDeSûreté " +
                "FROM Lieu " +
                "INNER JOIN Quartier ON Lieu.nomQuartier = Quartier.nom " +
                "INNER JOIN Ville ON Quartier.idVille = Ville.id;";
            return EnvoyerRequeteSelect<VueLieu>(requete);
        }

        public static List<VueLieu> LieuxTriNiveauSureté() // L_2
        {
            string requete =
                "SELECT Lieu.id, Lieu.nom AS nomLieu, Quartier.nom AS nomQuartier, Ville.nom AS nomVille, Lieu.niveauDeSûreté " +
                "FROM Lieu " +
                "INNER JOIN Quartier ON Lieu.nomQuartier = Quartier.nom " +
                "INNER JOIN Ville ON Quartier.idVille = Ville.id " +
                "ORDER BY Lieu.niveauDeSûreté DESC;";
            return EnvoyerRequeteSelect<VueLieu>(requete);
        }

        public static List<VueLieuTransaction> LieuxMinimumTransactions() // L_3
        {
            string requete =
                "SELECT Lieu.id, Lieu.nom, COUNT(*) as nb_transactions " +
                "FROM Lieu " +
                "INNER JOIN Transaction ON lieu.id = Transaction.idlieu " +
                "GROUP BY Lieu.id " +
                "HAVING COUNT(*) <= ALL(" +
                "   SELECT COUNT(*) FROM Lieu " +
                "   INNER JOIN Transaction ON lieu.id = Transaction.idlieu " +
                "   GROUP BY Lieu.id);";
            return EnvoyerRequeteSelect<VueLieuTransaction>(requete);
        }

        public static List<VueLieuTransaction> LieuxMaximumTransactions() // L_4
        {
            string requete =
                "SELECT Lieu.id, Lieu.nom, COUNT(*) as nb_transactions " +
                "FROM Lieu " +
                "INNER JOIN Transaction ON lieu.id = Transaction.idlieu " +
                "GROUP BY Lieu.id " +
                "HAVING COUNT(*) >= ALL(" +
                "   SELECT COUNT(*) FROM Lieu " +
                "   INNER JOIN Transaction ON lieu.id = Transaction.idlieu " +
                "   GROUP BY Lieu.id);";
            return EnvoyerRequeteSelect<VueLieuTransaction>(requete);
        }

        public static List<VueLieuVille> LieuxNombreParVille() // L_5
        {
            string requete =
                "SELECT Ville.nom as nom_ville, COUNT(Lieu.id) as nb_lieux " +
                "FROM Ville " +
                "JOIN Quartier ON Ville.id = Quartier.idVille " +
                "JOIN Lieu ON Quartier.nom = Lieu.nomQuartier AND Quartier.idVille = Lieu.idVille " +
                "GROUP BY nom_ville " +
                "ORDER BY nb_lieux DESC;";
            return EnvoyerRequeteSelect<VueLieuVille>(requete);
        }

        public static List<VueLieuQuartierVille> LieuxNombreParQuartierEtVille() // L_6
        {
            string requete =
                "SELECT Ville.nom as nom_ville, Quartier.nom as nom_quartier, COUNT(Lieu.id) as nb_lieux " +
                "FROM Ville " +
                "JOIN Quartier ON Ville.id = Quartier.idVille " +
                "JOIN Lieu ON Quartier.nom = Lieu.nomQuartier AND Quartier.idVille = Lieu.idVille " +
                "GROUP BY nom_ville, nom_quartier " +
                "ORDER BY nb_lieux DESC;";
            return EnvoyerRequeteSelect<VueLieuQuartierVille>(requete);
        }

        public static List<VueLieuNiveauSureté> LieuxNombreParNiveauSureté() // L_7
        {
            string requete =
                "SELECT Lieu.niveauDeSûreté, COUNT(Lieu.id) as nb_lieux " +
                "FROM Lieu " +
                "INNER JOIN Quartier ON Lieu.nomQuartier = Quartier.nom " +
                "INNER JOIN Ville ON Quartier.idVille = Ville.id " +
                "GROUP BY Lieu.niveauDeSûreté " +
                "ORDER BY nb_lieux DESC;";
            return EnvoyerRequeteSelect<VueLieuNiveauSureté>(requete);
        }

        // Statistiques
        public static List<VueStatsAnnéeTransactions> StatsTransactionsParAnnée() // S_1
        {
            string requete =
                "SELECT EXTRACT(year FROM date) AS annee, COUNT(*) as nombre_transactions " +
                "FROM Transaction " +
                "WHERE Transaction.statut = 'terminée' " +
                "GROUP BY annee " +
                "ORDER BY annee;";
            return EnvoyerRequeteSelect<VueStatsAnnéeTransactions>(requete);
        }

        public static List<VueStatsAnnéeMoisTransactions> StatsTransactionsParMois() // S_2
        {
            string requete =
                "SELECT EXTRACT(year FROM date) AS annee, EXTRACT(month FROM date) AS mois, COUNT(*) as nombre_transactions " +
                "FROM Transaction " +
                "WHERE Transaction.statut = 'terminée' " +
                "GROUP BY annee,mois " +
                "ORDER BY annee,mois;";
            return EnvoyerRequeteSelect<VueStatsAnnéeMoisTransactions>(requete);
        }

        public static List<VueStatsAnnéeMoisMontant> StatsMontantTotalTransactionsMois() // S_3
        {
            string requete =
                "SELECT EXTRACT(year FROM date) AS annee, EXTRACT(month FROM date) AS mois, SUM(montantPayé) AS total_montant_paye " +
                "FROM Transaction " +
                "WHERE Transaction.statut = 'terminée' " +
                "GROUP BY annee,mois " +
                "ORDER BY annee,mois;";
            return EnvoyerRequeteSelect<VueStatsAnnéeMoisMontant>(requete);
        }

        public static List<VueStatsAnnéeMoisTransactions> StatsNbBonnesTransactionsParMois() // S_4
        {
            string requete =
                "SELECT EXTRACT(year FROM date) AS annee, EXTRACT(month FROM date) AS mois, COUNT(*) as nombre_transactions " +
                "FROM Transaction " +
                "WHERE Transaction.statut = 'terminée' AND " +
                "(Transaction.qualitéDéroulement = 'bon' OR Transaction.qualitéDéroulement = 'très bon') " +
                "GROUP BY annee,mois " +
                "ORDER BY annee,mois;";
            return EnvoyerRequeteSelect<VueStatsAnnéeMoisTransactions>(requete);
        }

        public static List<VueStatsAnnéeMoisFiabilité> StatsFiabilitéVendeurs() // S_5
        {
            string requete =
                "SELECT EXTRACT(year FROM HistoriqueNiveauFiabilite.dateDebut) AS annee, " +
                "EXTRACT(month FROM HistoriqueNiveauFiabilite.dateDebut) AS mois, " +
                "SUM(CASE WHEN HistoriqueNiveauFiabilite.niveauFiabilité = 'faible' THEN 1 ELSE 0 END) AS \"faible\", " +
                "SUM(CASE WHEN HistoriqueNiveauFiabilite.niveauFiabilité <> 'moyen' THEN 1 ELSE 0 END) AS \"moyen\", " +
                "SUM(CASE WHEN HistoriqueNiveauFiabilite.niveauFiabilité <> 'fort' THEN 1 ELSE 0 END) AS \"fort\"" +
                "    FROM HistoriqueNiveauFiabilite" +
                "    INNER JOIN Vendeur ON Vendeur.idPersonnePhysique = HistoriqueNiveauFiabilite.idPersonnePhysique" +
                "    GROUP BY annee, mois;";
            return EnvoyerRequeteSelect<VueStatsAnnéeMoisFiabilité>(requete);
        }

        public static List<VueStatsAnnéeMoisPrix> StatsPrixTransactionsMois() // S_6
        {
            string requete =
                "SELECT EXTRACT(year FROM date) AS annee, EXTRACT(month FROM date) AS mois, SUM(prix) AS total_prix " +
                "FROM Transaction " +
                "WHERE Transaction.statut = 'terminée' " +
                "GROUP BY annee,mois " +
                "ORDER BY annee,mois;";
            return EnvoyerRequeteSelect<VueStatsAnnéeMoisPrix>(requete);
        }

        /// <summary>
        /// Envoie une requête de type SELECT et retourne le résultat sous forme d'une liste générique dont le type
        /// est spécifié et correspond à une vue de la requête SQL.
        /// </summary>
        /// <typeparam name="T">Vue représentant le résultat attendu.</typeparam>
        /// <param name="requete">Requête SQL</param>
        /// <returns>Une liste de tuples dont chaque élément est une vue. Si une erreur se produit durant l'exécution du script SQL, le résultat est une liste vide.</returns>
        /// <exception cref="InvalidOperationException">Si la chaîne de connexion de la classe AccèsDonnées n'est pas définie.</exception>
        private static List<T> EnvoyerRequeteSelect<T>(string requete) where T : new()
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new InvalidOperationException("La chaîne de connexion SQL n'est pas définie.");
            }

            var resultat = new List<T>();

            try
            {
                // On utilise la réflexion pour récupérer toutes les propriétés de la vue retournée.
                var proprietes = typeof(T).GetProperties();

                using var connection = new NpgsqlConnection(SqlConnectionString);
                connection.Open();

                using var cmd = new NpgsqlCommand(requete, connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Pour chaque tuple du résultat de la requête SQL, on rempli les valeurs de ses propriétés en
                    // lisant la colonne dont le nom est récupéré grâce à l'attribut 'PostgreColumnAttribute'.
                    T element = new T();
                    foreach (var propriete in proprietes)
                    {
                        var attribut = propriete.GetCustomAttribute<PostgreColumnAttribute>();
                        if (attribut == null)
                        {
                            continue;
                        }

                        var value = reader[attribut.ColumnName];
                        if (value is DBNull)
                        {
                            continue;
                        }

                        propriete.SetValue(element, value);
                    }
                    resultat.Add(element);
                }
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return resultat;
        }

        /// <summary>
        /// Envoie une requête de type UPDATE.
        /// </summary>
        /// <param name="requete">Requête SQL.</param>
        /// <exception cref="InvalidOperationException">Si la chaîne de connexion de la classe AccèsDonnées n'est pas définie.</exception>
        private static void EnvoyerRequeteUpdate(string requete)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new InvalidOperationException("Connection string not set.");
            }

            try 
            {
                using var connection = new NpgsqlConnection(SqlConnectionString);
                connection.Open();

                using var cmd = new NpgsqlCommand(requete, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
