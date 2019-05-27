using UnityEngine;

[System.Serializable]
public struct SMission //Structure de la mission
{
    [Tooltip("Titre de la Mission")]
    public string titre;
    [Tooltip("Objectif de la Mission")]
    public string objectif;
    [Tooltip("Description de la Mission")]
    public string description;
    [Tooltip("Type de la Mission")]
    public EMission typeMission;
    [Tooltip("Objectifs à remplir si la mission est de type Collecte")]
    public SCollect objectifCollect;
    [Tooltip("Etat de la Mission")]
    public EEtatMission etat;
    [Header("Dépendance")]
    [Tooltip("Si la mission est dépendante d'une autre")]
    public bool isDependant;
    [Tooltip("Index de la mission dont laquelle cette mission dépend")]
    public int dependanceMission;
    [Tooltip("Si la mission est actuellement affichée")]
    public bool isShow;
}

[System.Serializable]
public struct SCollect //Structure mission de collecte
{
    [Tooltip("Item de Mission à collecter")]
    public SItem item;
    [Tooltip("Nombre d'items à collecter")]
    public int nombreItems;
}

public enum EMission //Type de mission
{
    Collecter,      //Collecter des items
    Tuer,           //Tuer un PNJ
    TrouverPnj      //Trouver un PNJ
}

public enum EEtatMission //Etat de la mission
{
    EnAttente,      //En attente si la mission n'est pas encore active
    Active,         //Active si la mission est en cours
    Completee       //Complétée si la mission a été réussie
}
