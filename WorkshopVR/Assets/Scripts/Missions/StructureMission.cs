using System.Collections.Generic;
using UnityEngine;
using VRTK.UnityEventHelper;

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
    [Tooltip("Etat de la Mission")]
    public EEtatMission etat;
    [Tooltip("Objectifs à remplir si la mission est de type Collecter")]
    public SCollect objectifCollect;
    [Tooltip("Objectifs à remplir si la mission est de type Placer")]
    public SPlace objectifPlace;
    [Tooltip("Objectifs à remplir si la mission est de type Embeter")]
    public SEmbeter objectifEmbeter;
    [Tooltip("Objectifs à remplir si la mission est de type Blesser")]
    public SBlesser objectifBlesser;
    [Tooltip("Objectifs à remplir si la mission est de type Tuer")]
    public STuer objectifTuer;
    [Tooltip("Objectifs à remplir si la mission est de type Détruire")]
    public SDetruire objectifDetruire;
    [Header("Dépendance")]
    [Tooltip("Si la mission est dépendante d'une autre")]
    public bool isDependant;
    [Tooltip("Indexes de la mission dont laquelle cette mission dépend")]
    public int[] dependancesMission;
    [Tooltip("Si la mission est actuellement affichée")]
    public bool isShow;
}

[System.Serializable]
public class SCollect //Structure mission de collecte
{
    [Tooltip("Item de Mission à collecter")]
    public SItem item;
    [Tooltip("Nombre d'items à collecter")]
    public int nombreItems;
}

[System.Serializable]
public class SPlace //Structure mission de placement d'items
{
    [Tooltip("Item de mission à placer")]
    public SItem item;
    [Tooltip("Nombre d'items à placer")]
    public int nbItems;
    [Tooltip("Zones ou placer les items")]
    public VRTK_SnapDropZone_UnityEvents[] dropZones;
}

[System.Serializable]
public class SEmbeter //Structure de la mission où on embete les PNJ
{
    [Tooltip("Les PNJ à embeter dans cette mission")]
    public List<IAReaction> pnjAEmbeter = new List<IAReaction>();
    [Tooltip("Nombre de PNJ à embeter")]
    public int nbPnjAEmbeter;
}

[System.Serializable]
public class SBlesser //Structure de la mission ou on blesse les PNJ
{
    [Tooltip("Les PNJ à blesser dans cette mission")]
    public List<IAReaction> pnjABlesser = new List<IAReaction>();
    [Tooltip("Nombre de PNJ à blesser")]
    public int nbPnjABlesser;
}

[System.Serializable]
public class STuer //Structure de la mission ou on tue les PNJ
{
    [Tooltip("Si on doit tuer tous les PNJ")]
    public bool tuerToutLeMonde;
    [Tooltip("Les PNJ à tuer dans cette mission")]
    public List<IAReaction> pnjATuer = new List<IAReaction>();
    [Tooltip("Nombre de PNJ à tuer, laisser à 0 si on doit tuer tout le monde")]
    public int nbPnjATuer;
}

[System.Serializable]
public class SDetruire //Structure de la mission de destruction d'objets
{
    [Tooltip("Pourcentage d'objets à détruire (entre 0 et 1)")]
    [Range(0f, 1f)] public float pourcentage;
    [Tooltip("Laisser ce nombre à 0, il est initialisé tout seul au début du jeu")]
    public int nbItemsMap; //Nombre d'items qu'il y a sur la map
    [Tooltip("Nombre d'items cassés dans la map, ne pas remplir à la main")]
    public float nbItemsCasses;
    [Tooltip("Pourcentage actuel d'objets détruits")]
    [Range(0f, 1f)] public float pourcentageActuel;
    [Tooltip("Liste des items en jeu, ne pas remplir à la main")]
    public List<Item> itemsInGame = new List<Item>();

    public void InitNombreItemsMap() //Récupérer le nombre d'items présents sur la map
    {
        itemsInGame = new List<Item>();
        foreach(Item i in GameObject.FindObjectsOfType<Item>())
        {
            if(i.item.type == EItemType.Destructible)
            {
                itemsInGame.Add(i);
            }
        }
        nbItemsMap = itemsInGame.Count;
    }

    public void RefreshPourcentageDestruction()
    {
        float a = itemsInGame.Count; //Nombre d'items en jeu
        nbItemsCasses = 0f;
        foreach(Item i in itemsInGame)
        {
            if(i.item.etat == EItemEtat.Casse)
            {
                nbItemsCasses++;
            }
        }
        
        pourcentageActuel = nbItemsCasses / a;
    }
}

public enum EMission //Types de mission
{
    Collecter,      //Collecter des items
    Placer,         //Placer des items (ex: Caméras)
    Embeter,        //Embeter un PNJ avec un item qui dérange
    Blesser,        //Blesser un PNJ avec un item qui blesse
    Tuer,           //Tuer un PNJ avec un objet qui tue
    Detruire        //Détruire des objets
}

public enum EEtatMission //Etat de la mission
{
    EnAttente,      //En attente si la mission n'est pas encore active
    Active,         //Active si la mission est en cours
    Completee       //Complétée si la mission a été réussie
}
