[UnityEngine.CreateAssetMenu(fileName = "New Item", menuName = "Créer un item de mission")]
public class SItem : UnityEngine.ScriptableObject //Structure de l'item
{
    public string nom;              //Nom de l'item
    public string desciption;       //Description de l'item
    public EItemEtat etat;          //Etat de l'item
    public EItemType type;          //Type d'item
    public EItemAction action;      //Action de l'item sur PNJ
}

public enum EItemType //Types d'items
{
    ObjetDeMission,
    Consommable,
    Autre
}

public enum EItemEtat
{
    Normal,
    Casse
}

public enum EItemAction //Action de l'item sur le PNJ quand il le reçoit
{
    Derange,
    Blesse,
    Tue
}