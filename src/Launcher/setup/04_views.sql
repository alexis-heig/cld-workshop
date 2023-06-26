CREATE OR REPLACE VIEW vClient AS
    SELECT Personne.id, Personne.pseudonyme, PersonnePhysique.niveauFiabilite
    FROM Client
    INNER JOIN Personne ON Client.idPersonnePhysique = Personne.id
    INNER JOIN PersonnePhysique ON PersonnePhysique.idPersonne = Personne.id;

CREATE OR REPLACE VIEW vGenrePersonne AS
    SELECT pseudonyme,
    CASE
        WHEN Vendeur.idPersonnePhysique IS NOT NULL THEN 'Vendeur'
        WHEN Client.idPersonnePhysique IS NOT NULL THEN 'Client'
        WHEN Fournisseur.idPersonne IS NOT NULL THEN 'Fournisseur'
        ELSE '' -- problèmes si on en a 
    END AS genre
    FROM Personne
    LEFT JOIN Vendeur ON Vendeur.idPersonnePhysique = Personne.id
    LEFT JOIN Client ON Client.idPersonnePhysique = Personne.id
    LEFT JOIN Fournisseur ON Fournisseur.idPersonne = Personne.id;

CREATE OR REPLACE VIEW vTransaction AS
    SELECT T.*, V.pseudonyme as nomVendeur, C.pseudonyme as nomClient,
    Lieu.nom as nomLieu, Ville.nom as nomVille
    FROM Transaction T
    INNER JOIN (
        SELECT P.pseudonyme, V.idPersonnePhysique FROM Personne P 
            JOIN Vendeur V ON P.id = V.idPersonnePhysique) as V 
            ON T.idVendeur = V.idPersonnePhysique
    INNER JOIN (
        SELECT P.pseudonyme, C.idPersonnePhysique FROM Personne P 
        JOIN Client C ON P.id = C.idPersonnePhysique) as C 
        ON T.idClient = C.idPersonnePhysique
    INNER JOIN Lieu ON Lieu.id = T.idLieu
    INNER JOIN Ville ON Ville.id = Lieu.idVille;
    
CREATE OR REPLACE VIEW vVendeur AS
    SELECT Personne.id, Personne.pseudonyme, 
    Vendeur.statut, PersonnePhysique.niveauFiabilite,
    Quartier.nom as nomQuartier, Ville.nom as nomVille
    FROM Vendeur
    INNER JOIN Personne ON Vendeur.idPersonnePhysique = Personne.id
    INNER JOIN PersonnePhysique ON PersonnePhysique.idPersonne = Personne.id
    INNER JOIN Quartier ON Vendeur.nomQuartier = Quartier.nom
    INNER JOIN Ville ON Quartier.idVille = Ville.id;