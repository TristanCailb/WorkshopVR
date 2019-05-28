using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;      //Singleton
    public SMission[] missions;                 //Missions à effectuer

    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    void OnEnable()
    {
        foreach(SMission m in missions)
        {
            if(m.typeMission == EMission.Placer)
            {
                foreach(VRTK_SnapDropZone_UnityEvents dz in m.objectifPlace.dropZones)
                {
                    //Ajouter les events aux drop zones
                    dz.OnObjectSnappedToDropZone.AddListener(delegate { CheckPlaceMission(dz.GetComponent<VRTK_SnapDropZone>()); });
                }
            }
        }
    }

    public void ActiverMission(int index)
    {
        missions[index].etat = EEtatMission.Active;
        missions[index].isShow = true;
    }

    public void CompleterMission(int index)
    {
        missions[index].etat = EEtatMission.Completee;
        RefreshMissionsActives();
    }

    public void RefreshMissionsActives()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            //Si la mission a une dépendance et que les missions dont elle dépend sont complétées alors on active la mission
            SMission mission = missions[i];
            if (mission.isDependant)
            {
                if(CheckAllDependancesCompleted(mission) && !mission.isShow)
                {
                    ActiverMission(i);
                }
            }
        }
    }

    private bool CheckAllDependancesCompleted(SMission m)
    {
        //Vérifier si toutes les missions dont la mission dépend sont complétées
        for(int i = 0; i < m.dependancesMission.Length; i++)
        {
            if(missions[i].etat != EEtatMission.Completee)
            {
                return false;
            }
        }
        return true;
    }

    public void CheckCollectMission(Item item, int nombre)
    {
        //Vérifier si tous les items de la mission de collecte sont ramassés (quand on ramasse un item)
        for (int i = 0; i < missions.Length; i++) //Parcourir toutes les missions
        {
            SMission mission = missions[i]; //mission actuellement vérifiée
            if(mission.typeMission == EMission.Collecter && mission.etat == EEtatMission.Active) //Si la mission est de type Collecte et active
            {
                if(item.item == mission.objectifCollect.item) //Si l'item qu'on ramasse est l'item de la mission à ramasser
                {
                    if(mission.objectifCollect.nombreItems <= nombre) //Si le nombre d'items demandé est inférieur ou égal à celui collecté
                    {
                        CompleterMission(i); //Rendre cette mission terminée
                    }
                }
            }
        }
    }

    public void CheckPlaceMission(VRTK_SnapDropZone drop)
    {
        Item item = drop.GetCurrentSnappedObject().GetComponent<Item>(); //Récupérer le composant item de l'objet snappé
        //Vérifier si tous les items de la mission sont placés (quand on pose un item sur une zone de drop)
        for(int i = 0; i < missions.Length; i++)
        {
            SMission mission = missions[i];
            if(mission.typeMission == EMission.Placer && mission.etat == EEtatMission.Active) //Si c'est une mission de placement active
            {
                if(item.item == mission.objectifPlace.item) //Si on place le bon item
                {
                    Debug.Log("Bon Item");
                    mission.objectifPlace.nbItems--; //Diminuer le nombre d'items à placer
                    if(mission.objectifPlace.nbItems == 0) //Si on a placé tous les items
                    {
                        CompleterMission(i); //Terminer la mission
                        Debug.Log("Mission OK");
                    }
                }
            }
        }
    }
}
