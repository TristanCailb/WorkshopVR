[UnityEngine.CreateAssetMenu(fileName = "New Item", menuName = "Créer un item de mission")]
public class SItem : UnityEngine.ScriptableObject //Structure de l'item
{
    public string nom;              //Nom de l'item
    public string desciption;       //Description de l'item
    public EItemType type;          //Type d'item
}

public enum EItemType //Types d'items
{
    ObjetDeMission,
    Consommable,
    Autre
}