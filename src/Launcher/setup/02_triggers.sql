---- Historisation
-- Sauvegarde l'historique des niveaux de fiabilité des personnes physiques

CREATE OR REPLACE FUNCTION historise_niveau_fiabilite() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
  INSERT INTO HistoriqueNiveauFiabilite (idPersonnePhysique, dateDebut, niveauFiabilité)
  VALUES (NEW.idPersonne, NOW(), NEW.niveauFiabilite);
  RETURN NULL;
END;
$BODY$;

CREATE OR REPLACE TRIGGER after_update_niveaufiabilite_personne
AFTER INSERT OR UPDATE ON PersonnePhysique
FOR EACH ROW
EXECUTE FUNCTION historise_niveau_fiabilite();

-- Sauvegarde l'historique des status des vendeurs

CREATE OR REPLACE FUNCTION historise_statut() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
  INSERT INTO HistoriqueStatut (idVendeur, dateDebut, statut)
  VALUES (NEW.idPersonnePhysique, NOW(), NEW.statut);
  RETURN NULL;
END;
$BODY$;

CREATE OR REPLACE TRIGGER after_update_niveaustatut_vendeur
AFTER INSERT OR UPDATE ON Vendeur
FOR EACH ROW
EXECUTE FUNCTION historise_statut();


---- Contraintes d'intégrités
-- Une transaction ne peut être conduite que par un vendeur possédant le statut en vie.

CREATE OR REPLACE FUNCTION check_vendor_status() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $DATA$
DECLARE
	statutMomentTransaction varchar;
BEGIN
    SELECT statut into statutMomentTransaction
	FROM historiqueStatut 
	WHERE idVendeur = NEW.idVendeur
	AND datedebut < NEW.date
	ORDER BY datedebut DESC
	LIMIT 1;
	
	IF statutMomentTransaction != 'en vie' THEN
        RAISE EXCEPTION 'Un vendeur doit être en vie pour participer à une transaction.';
    END IF;
    RETURN NEW;
END;
$DATA$;

CREATE OR REPLACE TRIGGER before_insert_transaction_vendeur_en_vie
BEFORE INSERT OR UPDATE ON Transaction
FOR EACH ROW
EXECUTE FUNCTION check_vendor_status();


-- Ni un vendeur ni un client ne peuvent être liés à plusieurs transactions ayant lieu simultanément.

CREATE OR REPLACE FUNCTION check_transaction_overlap() RETURNS TRIGGER 
LANGUAGE plpgsql 
AS $BODY$
BEGIN
    IF EXISTS (SELECT * 
			   FROM Transaction 
			   WHERE idVendeur = NEW.idVendeur 
			   AND NEW.date >= date 
			   AND NEW.date < (date + interval '1 hour')
			   AND id <> NEW.id) THEN
		RAISE EXCEPTION 'Le vendeur est déjà en train d effectuer une transaction';
    END IF;
    IF EXISTS (SELECT * 
			   FROM Transaction 
			   WHERE idClient = NEW.idClient 
			   AND NEW.date >= date 
			   AND NEW.date < (date + interval '1 hour')
			   AND id <> NEW.id) THEN
		RAISE EXCEPTION 'Le client est déjà en train d effectuer une transaction';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_transaction_overlaps
BEFORE INSERT OR UPDATE ON Transaction
FOR EACH ROW
EXECUTE FUNCTION check_transaction_overlap();




-- Pour des raisons de sécurité, il ne doit pas être possible que deux transactions prennent place dans le
-- même lieu simultanément.

CREATE OR REPLACE FUNCTION check_location_overlap() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF EXISTS (SELECT * FROM Transaction 
			   WHERE idLieu = NEW.idLieu 
			   AND NEW.date >= date 
			   AND NEW.date < (date + interval '1 hour')
			   AND id <> NEW.id) THEN
		RAISE EXCEPTION 'Le lieu est déjà utilisé pour une autre transaction à ce moment là';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_update_transaction_lieu
BEFORE INSERT OR UPDATE ON Transaction
FOR EACH ROW
EXECUTE FUNCTION check_location_overlap();


-- Mise à jour du prix d'une transaction lors de l'ajout d'une transaction_stock

CREATE OR REPLACE FUNCTION update_transaction_price() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
	UPDATE Transaction 
	SET prix = (SELECT COALESCE(SUM(transaction_stock.quantité * prixUnitaire), 0) 
				FROM Transaction_Stock 
					JOIN Stock ON Stock.id = Transaction_Stock.idStock 
					JOIN Produit ON Produit.id = Stock.idProduit 
				WHERE idTransaction = NEW.idTransaction) 
	WHERE id = NEW.idTransaction;

    RETURN NULL;
END;
$BODY$;

CREATE OR REPLACE TRIGGER insert_update_delete_transaction_stock
AFTER INSERT OR UPDATE OR DELETE ON Transaction_Stock
FOR EACH ROW
EXECUTE FUNCTION update_transaction_price();


-- La date du stock d'une transaction_stock doit être inférieure à la date de la transaction

CREATE OR REPLACE FUNCTION check_date_stock() RETURNS TRIGGER
LANGUAGE plpgsql
AS $BODY$
BEGIN
	IF (SELECT dateArrivée 
		FROM stock
	    WHERE NEW.idStock = id
	   )
	   >
	   (SELECT date
	    FROM transaction
	   	WHERE NEW.idTransaction = id)
	THEN
		RAISE EXCEPTION 'La transaction ne peut pas contenir de stock n''étant pas encore arrivés';
	END IF;
	
	RETURN NEW;
END
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_update_transaction_stock
BEFORE INSERT OR UPDATE on transaction_stock
FOR EACH ROW
EXECUTE FUNCTION check_date_stock();


-- La quantité d’un certain stock dans une transaction ne doit pas dépasser la quantité disponible dans ce stock

CREATE OR REPLACE FUNCTION check_stock_availability() RETURNS TRIGGER
LANGUAGE plpgsql
AS $BODY$
BEGIN
	IF (SELECT SUM(transaction_stock.quantité)
		FROM stock
		LEFT JOIN transaction_stock
			ON transaction_stock.idstock = stock.id
		WHERE idstock = NEW.idstock
		GROUP BY stock.id) + NEW.quantité
		>
		(SELECT quantité 
		 FROM stock
		 WHERE id = NEW.idstock)
	THEN
		RAISE EXCEPTION 'Il n''y a plus de quantité disponible pour ce stock (ID : %)', NEW.idstock;
	END IF;
	RETURN NEW;
END
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_update_transaction_stock_quantity_available
BEFORE INSERT OR UPDATE on transaction_stock
FOR EACH ROW
EXECUTE FUNCTION check_stock_availability();


----- Héritage

-- Vérification q'une Personne existe avant d'insérer une PersonnePhysique ou un Fournisseur ( COMPLET )

CREATE OR REPLACE FUNCTION check_personne_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF NOT EXISTS (SELECT * FROM Personne WHERE id = NEW.idPersonne) THEN
        RAISE EXCEPTION 'Enregistrement du parent dans Personne introuvable';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_personnephysique
BEFORE INSERT ON PersonnePhysique
FOR EACH ROW
EXECUTE FUNCTION check_personne_exists();

CREATE OR REPLACE TRIGGER before_insert_fournisseur
BEFORE INSERT ON Fournisseur
FOR EACH ROW
EXECUTE FUNCTION check_personne_exists();


-- Verification qu'une nouvelle PersonnePhysique n'est pas dàjà Fournisseur ( DISJOINT )

CREATE OR REPLACE FUNCTION check_fournisseur_not_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF EXISTS (SELECT * FROM Fournisseur WHERE idpersonne = NEW.idPersonne) THEN
        RAISE EXCEPTION 'Cette personne est déjà un fournisseur';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_personnephysique_fournisseur
BEFORE INSERT ON PersonnePhysique
FOR EACH ROW
EXECUTE FUNCTION check_fournisseur_not_exists();


-- Verification qu'un nouveau Fournisseur n'est pas dàjà PersonnePhysique ( DISJOINT )

CREATE OR REPLACE FUNCTION check_personnephysique_not_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF EXISTS (SELECT * FROM PersonnePhysique WHERE idpersonne = NEW.idPersonne) THEN
        RAISE EXCEPTION 'Cette personne est déjà une personne physique';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_fournisseur_personnephysique
BEFORE INSERT ON Fournisseur
FOR EACH ROW
EXECUTE FUNCTION check_personnephysique_not_exists();


-- Vérification qu'une PersonnePhysique existe avant d'insérer un Client ou un Vendeur ( COMPLET )

CREATE OR REPLACE FUNCTION check_personnephysique_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF NOT EXISTS (SELECT * FROM PersonnePhysique WHERE idPersonne = NEW.idPersonnePhysique) THEN
        RAISE EXCEPTION 'Enregistrement du parent dans PersonnePhysique introuvable';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_client
BEFORE INSERT ON Client
FOR EACH ROW
EXECUTE FUNCTION check_personnephysique_exists();

CREATE OR REPLACE TRIGGER before_insert_vendeur
BEFORE INSERT ON Vendeur
FOR EACH ROW
EXECUTE FUNCTION check_personnephysique_exists();


-- Verification qu'un novuveau Client n'est pas un Vendeur 

CREATE OR REPLACE FUNCTION check_vendeur_not_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF EXISTS (SELECT * FROM Vendeur WHERE idpersonnephysique = NEW.idpersonnephysique) THEN
        RAISE EXCEPTION 'Cette personne est déjà un vendeur';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_client_vendeur
BEFORE INSERT ON Client
FOR EACH ROW
EXECUTE FUNCTION check_vendeur_not_exists();

-- Verification qu'un novuveau Vendeur n'est pas un Client

CREATE OR REPLACE FUNCTION check_client_not_exists() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $BODY$
BEGIN
    IF EXISTS (SELECT * FROM Client WHERE idpersonnephysique = NEW.idpersonnephysique) THEN
        RAISE EXCEPTION 'Cette personne est déjà un client';
    END IF;
    RETURN NEW;
END;
$BODY$;

CREATE OR REPLACE TRIGGER before_insert_vendeur_client
BEFORE INSERT ON Vendeur
FOR EACH ROW
EXECUTE FUNCTION check_client_not_exists();





