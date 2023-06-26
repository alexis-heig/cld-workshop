namespace DrugLordManager.Attributes
{
    /// <summary>
    /// Cet attribut permet, lors de la création d'une classe représentant un retour de requête SQL,
    /// de spécifier, pour chacune des propriétés de la classe, le nom de la colonne SQL associée dans
    /// laquelle il faut aller chercher la valeur de la propriété.
    /// 
    /// Des exemples d'utilisations se trouvent dans les fichiers du dossier 'Views'.
    /// </summary>
    public class PostgreColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public PostgreColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
