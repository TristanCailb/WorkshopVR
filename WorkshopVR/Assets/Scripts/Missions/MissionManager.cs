using UnityEngine;

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
            //Si la mission a une dépendance et que la mission dont elle dépend est complétée alors on actualise la mission
            SMission mission = missions[i];
            if (mission.isDependant && missions[mission.dependanceMission].etat == EEtatMission.Completee && !mission.isShow)
            {
                ActiverMission(i);
            }
        }
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
}
