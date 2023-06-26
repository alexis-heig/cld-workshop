SET client_encoding TO 'UTF8';

/***** Enums *****/

DROP TYPE IF EXISTS NIVEAU_FIABILITE CASCADE;
CREATE TYPE NIVEAU_FIABILITE AS ENUM ('faible', 'moyen', 'fort');

DROP TYPE IF EXISTS NIVEAU_SURETE CASCADE;
CREATE TYPE NIVEAU_SURETE AS ENUM ('critique', 'dangereux', 'normal', 'paisible', 'très sûr');

DROP TYPE IF EXISTS STATUT_EMPLOYE CASCADE;
CREATE TYPE STATUT_EMPLOYE AS ENUM ('en vie', 'mort', 'au trou', 'rangé');

DROP TYPE IF EXISTS STATUT_TRANSACTION CASCADE;
CREATE TYPE STATUT_TRANSACTION AS ENUM ('en cours', 'terminée', 'annulée');

DROP TYPE IF EXISTS QUALITE CASCADE;
CREATE TYPE QUALITE AS ENUM ('très mauvais', 'mauvais', 'moyen', 'bon', 'très bon');


/***** Création des tables *****/

DROP TABLE IF EXISTS Personne CASCADE;
CREATE TABLE Personne (
  id SMALLSERIAL,
  pseudonyme VARCHAR(30) NOT NULL,
  CONSTRAINT PK_Personne PRIMARY KEY (id)
);

DROP TABLE IF EXISTS PersonnePhysique CASCADE;
CREATE TABLE PersonnePhysique (
  idPersonne SMALLINT,
  niveauFiabilite NIVEAU_FIABILITE NOT NULL,
  CONSTRAINT PK_PersonnePhysique PRIMARY KEY (idPersonne)
);
  
DROP TABLE IF EXISTS Vendeur CASCADE;      
CREATE TABLE Vendeur (
  idPersonnePhysique SMALLINT,
  statut STATUT_EMPLOYE NOT NULL,
  nomQuartier VARCHAR(30) NOT NULL,
  idVille SMALLINT NOT NULL,
  CONSTRAINT PK_Vendeur PRIMARY KEY (idPersonnePhysique)
);

DROP TABLE IF EXISTS Client CASCADE;
CREATE TABLE Client (
  idPersonnePhysique SMALLINT,
  CONSTRAINT PK_Client PRIMARY KEY (idPersonnePhysique)
);

DROP TABLE IF EXISTS Fournisseur CASCADE;
CREATE TABLE Fournisseur (
  idPersonne SMALLINT,
  CONSTRAINT PK_Fournisseur PRIMARY KEY (idPersonne)
);

DROP TABLE IF EXISTS Ville CASCADE;
CREATE TABLE Ville (
  id SMALLSERIAL,
  nom VARCHAR(30) NOT NULL,
  CONSTRAINT PK_Ville PRIMARY KEY (id)
);

DROP TABLE IF EXISTS Quartier CASCADE;

CREATE TABLE Quartier (
  nom VARCHAR(30) NOT NULL,
  idVille SMALLINT NOT NULL,
  CONSTRAINT PK_Quartier PRIMARY KEY (nom, idVille)
);

DROP TABLE IF EXISTS Lieu CASCADE;
CREATE TABLE Lieu (
  id SMALLSERIAL,
  nom VARCHAR(30) NOT NULL,
  niveauDeSûreté NIVEAU_SURETE NOT NULL,
  nomQuartier VARCHAR(30) NOT NULL,
  idVille SMALLINT NOT NULL,
  CONSTRAINT PK_Lieu PRIMARY KEY (id)
);

DROP TABLE IF EXISTS Transaction CASCADE;
CREATE TABLE Transaction (
  id SMALLSERIAL,
  idClient SMALLINT NOT NULL,
  idVendeur SMALLINT NOT NULL,
  idLieu SMALLINT NOT NULL,
  date TIMESTAMP NOT NULL,
  qualitéDéroulement QUALITE NOT NULL,
  remarque VARCHAR(300),
  prix DECIMAL(10,2) NOT NULL,
  montantPayé DECIMAL(10,2) NOT NULL,
  statut STATUT_TRANSACTION NOT NULL,
  CONSTRAINT PK_Transaction PRIMARY KEY (id)
);

DROP TABLE IF EXISTS Transaction_Stock CASCADE;
CREATE TABLE Transaction_Stock (
  idTransaction SMALLINT,
  idStock SMALLINT NOT NULL,
  quantité DECIMAL(10,2) NOT NULL,
  CONSTRAINT PK_Transaction_Stock PRIMARY KEY (idTransaction, idStock)
);

DROP TABLE IF EXISTS Stock CASCADE;
CREATE TABLE Stock (
  id SMALLSERIAL,
  idProduit SMALLINT NOT NULL,
  idFournisseur SMALLINT NOT NULL,
  quantité DECIMAL(10,2) NOT NULL,
  qualité QUALITE NOT NULL,
  dateArrivée TIMESTAMP NOT NULL,
  CONSTRAINT PK_Stock PRIMARY KEY (id)
);

DROP TABLE IF EXISTS Produit CASCADE;
CREATE TABLE Produit (
  id SMALLSERIAL,
  nom VARCHAR(30) NOT NULL,
  prixUnitaire DECIMAL(10, 2) NOT NULL,
  CONSTRAINT PK_Produit PRIMARY KEY (id)
);

DROP TABLE IF EXISTS HistoriqueNiveauFiabilite;
CREATE TABLE HistoriqueNiveauFiabilite(
  idPersonnePhysique SMALLINT NOT NULL,
  dateDebut TIMESTAMP NOT NULL,
  niveauFiabilité NIVEAU_FIABILITE NOT NULL,
  CONSTRAINT PK_HistoriqueNiveauFiabilite PRIMARY KEY (idPersonnePhysique, dateDebut)
);

DROP TABLE IF EXISTS HistoriqueStatut;
CREATE TABLE HistoriqueStatut(
  idVendeur SMALLINT NOT NULL,
  dateDebut TIMESTAMP NOT NULL,
  statut STATUT_EMPLOYE NOT NULL,
  CONSTRAINT PK_HistoriqueStatut PRIMARY KEY (idVendeur, dateDebut)
);


/***** Index *****/

DROP INDEX IF EXISTS IDX_FK_Vendeur_idVille;
CREATE INDEX IDX_FK_Vendeur_idVille ON Vendeur(idVille);

DROP INDEX IF EXISTS IDX_FK_Quartier_idVille;
CREATE INDEX IDX_FK_Quartier_idVille ON Quartier(idVille);

DROP INDEX IF EXISTS IDX_FK_Lieu_idVille;
CREATE INDEX IDX_FK_Lieu_idVille ON Lieu(idVille);

DROP INDEX IF EXISTS IDX_FK_Transaction_idClient;
CREATE INDEX IDX_FK_Transaction_idClient ON Transaction(idClient);

DROP INDEX IF EXISTS IDX_FK_Transaction_idVendeur;
CREATE INDEX IDX_FK_Transaction_idVendeur ON Transaction(idVendeur);

DROP INDEX IF EXISTS IDX_FK_Transaction_idLieu;
CREATE INDEX IDX_FK_Transaction_idLieu ON Transaction(idLieu);

DROP INDEX IF EXISTS IDX_FK_Transaction_Stock_idLieu;
CREATE INDEX IDX_FK_Transaction_Stock_idLieu ON Transaction_Stock(idTransaction);

DROP INDEX IF EXISTS IDX_FK_Transaction_Stock_idStock;
CREATE INDEX IDX_FK_Transaction_Stock_idStock ON Transaction_Stock(idStock);

DROP INDEX IF EXISTS IDX_FK_Stock_idProduit;
CREATE INDEX IDX_FK_Stock_idProduit ON Stock(idProduit);

DROP INDEX IF EXISTS IDX_FK_Stock_idFournisseur;
CREATE INDEX IDX_FK_Stock_idFournisseur ON Stock(idFournisseur);

DROP INDEX IF EXISTS IDX_FK_HistoriqueNiveauFiabilite_idPersonnePhysique;
CREATE INDEX IDX_FK_HistoriqueNiveauFiabilite_idPersonne ON HistoriqueNiveauFiabilite(idPersonnePhysique);

DROP INDEX IF EXISTS IDX_FK_HistoriqueStatut_idVendeur;
CREATE INDEX IDX_FK_HistoriqueStatut_idVendeur ON HistoriqueStatut(idVendeur);


/***** Gestion des contraintes *****/

/* Gestion des acteurs */

ALTER TABLE PersonnePhysique
    ADD CONSTRAINT FK_PersonnePhysique_idPersonne
        FOREIGN KEY (idPersonne)
            REFERENCES Personne (id)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;

ALTER TABLE Vendeur
    ADD CONSTRAINT FK_Vendeur_idPersonnePhysique
        FOREIGN KEY (idPersonnePhysique)
            REFERENCES PersonnePhysique (idPersonne)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;

ALTER TABLE Vendeur
    ADD CONSTRAINT FK_Vendeur_idQuartier_idVille
        FOREIGN KEY (nomQuartier, idVille)
            REFERENCES Quartier (nom, idVille)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;

ALTER TABLE Client
    ADD CONSTRAINT FK_Client_idPersonnePhysique
        FOREIGN KEY (idPersonnePhysique)
            REFERENCES PersonnePhysique (idPersonne)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;

ALTER TABLE Fournisseur
    ADD CONSTRAINT FK_Fournisseur_idPersonne
        FOREIGN KEY (idPersonne)
            REFERENCES Personne (id)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;      

ALTER TABLE HistoriqueNiveauFiabilite
  ADD CONSTRAINT FK_HistoriqueNiveauFiabilite_idPersonnePhysique
    FOREIGN KEY (idPersonnePhysique)
      REFERENCES PersonnePhysique(idPersonne)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

ALTER TABLE HistoriqueStatut
  ADD CONSTRAINT FK_HistoriqueStatut_idVendeur
    FOREIGN KEY (idVendeur)
      REFERENCES Vendeur(idPersonnePhysique)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

/* Gestion des emplacements */
ALTER TABLE Quartier
  ADD CONSTRAINT FK_Quartier_idVille
    FOREIGN KEY (idVille)
      REFERENCES Ville (id)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;

ALTER TABLE Lieu
  ADD CONSTRAINT FK_Lieu_idQuartier_idVille
    FOREIGN KEY (nomQuartier, idVille)
      REFERENCES Quartier (nom, idVille)
  ON UPDATE NO ACTION
  ON DELETE CASCADE;

/* Gestion de la transaction */

ALTER TABLE Transaction
  ADD CONSTRAINT FK_Transaction_idClient
    FOREIGN KEY (idClient)
      REFERENCES Client (idPersonnePhysique);

ALTER TABLE Transaction
  ADD CONSTRAINT FK_Transaction_idVendeur
    FOREIGN KEY (idVendeur)
      REFERENCES Vendeur (idPersonnePhysique);

ALTER TABLE Transaction
  ADD CONSTRAINT FK_Transaction_idLieu
    FOREIGN KEY (idLieu)
      REFERENCES Lieu (id);

ALTER TABLE Transaction_Stock
  ADD CONSTRAINT FK_Transaction_Stock_idTransaction
    FOREIGN KEY (idTransaction)
      REFERENCES Transaction (id)
  ON DELETE CASCADE;

ALTER TABLE Transaction_Stock
  ADD CONSTRAINT FK_Transaction_Stock_idStock
    FOREIGN KEY (idStock)
      REFERENCES Stock (id);

/* Gestion de la marchandise */

ALTER TABLE Stock
  ADD CONSTRAINT FK_Stock_idProduit
    FOREIGN KEY (idProduit)
      REFERENCES Produit (id);

ALTER TABLE Stock
  ADD CONSTRAINT FK_Stock_idFournisseur
    FOREIGN KEY (idFournisseur)
      REFERENCES Fournisseur (idPersonne);

/* Gestion des valeurs numériques */

ALTER TABLE Transaction
  ADD CONSTRAINT CK_Transaction_montantPayé
    CHECK (montantPayé >= 0);

ALTER TABLE Transaction_Stock
  ADD CONSTRAINT CK_Transaction_Stock_quantité
    CHECK (quantité > 0);

ALTER TABLE Stock
  ADD CONSTRAINT CK_Stock_quantité
    CHECK (quantité >= 0);

ALTER TABLE Produit
  ADD CONSTRAINT CK_Produit_prixUnitaire
    CHECK (prixUnitaire >= 0);
