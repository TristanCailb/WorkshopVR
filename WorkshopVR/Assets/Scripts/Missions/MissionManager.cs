using UnityEngine;
using UnityEngine.UI;
using VRTK;
using VRTK.UnityEventHelper;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;      //Singleton
    public SMission[] missions;                 //Missions à effectuer
    [Header("HUD")]
    public VerticalLayoutGroup missionsHolder;  //Le conteneur des missions sur le HUD
    public GameObject missionHudPrefab;         //Prefab du HUD de mission
    public Image pbFillBurnout;                 //Progress Bar du Burnout dans le HUD
    public Text txtPourcentageBurnout;          //Texte du pourcentage de Burnout dans le HUD

    private float normMissionProgress;          //Progression dans les missions (entre 0 et 1) : missionsCompletes / totalMissions
    private float totalMissions;                //Nombre total de missions
    private float missionsCompletes;            //Nombre de missions réussie
    private int pourcentageBurnout;             //Pourcentage du Burnout : Mathf.Round(normMissionProgress * 100f)


    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    void OnEnable()
    {
        foreach (SMission m in missions)
        {
            if (m.typeMission == EMission.Placer)
            {
                foreach (VRTK_SnapDropZone_UnityEvents dz in m.objectifPlace.dropZones)
                {
                    //Ajouter les events aux drop zones
                    dz.OnObjectSnappedToDropZone.AddListener(delegate { CheckPlaceMission(dz.GetComponent<VRTK_SnapDropZone>()); });
                }
            }

            if(m.typeMission == EMission.Tuer && m.objectifTuer.tuerToutLeMonde)
            {
                m.objectifTuer.nbPnjATuer = m.objectifTuer.pnjATuer.Count; //Initialiser le nombre de PNJ à tuer si on doit tous les tuer
            }

            if(m.typeMission == EMission.Detruire)
            {
                m.objectifDetruire.InitNombreItemsMap(); //Initialiser le nombre d'items sur la map
            }

            totalMissions = missions.Length;
            RefreshHUDMissions();
            RefreshHUDBurnout();
        }
    }

    public void ActiverMission(int index)
    {
        missions[index].etat = EEtatMission.Active; //Activer la mission
        missions[index].isShow = true; //Boolean de debug pour éviter que la mission reste active
    }

    public void CompleterMission(int index)
    {
        missions[index].etat = EEtatMission.Completee; //Compléter la mission
        RefreshMissionsActives(); //Raffraichir la liste des missions actives
        RefreshHUDMissions(); //Refresh Ecran des Missions
        RefreshHUDBurnout(); //Refresh Jauge de Burnout
    }

    public void RefreshHUDBurnout()
    {
        missionsCompletes = 0f;
        foreach(SMission m in missions)
        {
            if(m.etat == EEtatMission.Completee)
            {
                missionsCompletes++;
            }
        }
        normMissionProgress = missionsCompletes / totalMissions;
        pbFillBurnout.fillAmount = normMissionProgress;
        pourcentageBurnout = (int) Mathf.Round(normMissionProgress * 100f);
        txtPourcentageBurnout.text = pourcentageBurnout.ToString() + "%";
    }

    public void RefreshMissionsActives()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            //Si la mission a une dépendance et que les missions dont elle dépend sont complétées alors on active la mission
            SMission mission = missions[i];
            if (mission.isDependant)
            {
                if (CheckAllDependancesCompleted(mission) && !mission.isShow)
                {
                    ActiverMission(i);
                }
            }
        }
    }

    public void RefreshHUDMissions()
    {
        for(int i = 0; i < missionsHolder.transform.childCount; i++) //Vider la liste des missions dans le HUD
        {
            Transform child = missionsHolder.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        foreach(SMission m in missions)
        {
            if(m.etat == EEtatMission.Active)
            {
                //Créer le prefab de mission HUD
                GameObject hud = Instantiate(missionHudPrefab, missionsHolder.transform.position, missionsHolder.transform.rotation, missionsHolder.transform);
                Text titre = hud.transform.Find("TXT_Titre").GetComponent<Text>(); //Récupérer le titre
                Text description = hud.transform.Find("TXT_Description").GetComponent<Text>(); //Récupérer la description
                Text objectif = hud.transform.Find("TXT_Objectifs").GetComponent<Text>(); //Récupérer les objectifs
                titre.text = m.titre; //Ecrire le titre dans le HUD
                description.text = m.description; //Ecrire la description dans le HUD
                objectif.text = "Objectif :\n  - " + m.objectif; //Ecrire l'objectif dans le HUD
            }
        }
    }

    private bool CheckAllDependancesCompleted(SMission m)
    {
        //Vérifier si toutes les missions dont la mission dépend sont complétées
        for (int i = 0; i < m.dependancesMission.Length; i++)
        {
            if (missions[i].etat != EEtatMission.Completee)
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
            if (mission.typeMission == EMission.Collecter && mission.etat == EEtatMission.Active) //Si la mission est de type Collecte et active
            {
                if (item.item == mission.objectifCollect.item) //Si l'item qu'on ramasse est l'item de la mission à ramasser
                {
                    if (mission.objectifCollect.nombreItems <= nombre) //Si le nombre d'items demandé est inférieur ou égal à celui collecté
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
        for (int i = 0; i < missions.Length; i++)
        {
            SMission mission = missions[i];
            if (mission.typeMission == EMission.Placer && mission.etat == EEtatMission.Active) //Si c'est une mission de placement active
            {
                if (item.item == mission.objectifPlace.item) //Si on place le bon item
                {
                    mission.objectifPlace.nbItems--; //Diminuer le nombre d'items à placer
                    if (mission.objectifPlace.nbItems == 0) //Si on a placé tous les items
                    {
                        CompleterMission(i); //Terminer la mission
                    }
                }
            }
        }
    }

    public void CheckReactionMission(Item item, EItemAction action, IAReaction pnjTouche)
    {
        //Vérifier si tous les PNJ de la mission ont réagi comme on veut (quand on leur jette un item dessus)
        for (int i = 0; i < missions.Length; i++)
        {
            SMission mission = missions[i];
            if (mission.etat == EEtatMission.Active) //Si c'est une mission active
            {
                if (item.item.action == action) //Vérifier si l'action de l'item reçue est celle voulue
                {
                    switch(mission.typeMission) //Vérifier le type de mission
                    {
                        case EMission.Embeter:
                            if(mission.objectifEmbeter.pnjAEmbeter.Contains(pnjTouche)) //Si le PNJ touché fait partie de la liste
                            {
                                mission.objectifEmbeter.nbPnjAEmbeter--; //Diminuer le nombre de pnj à déranger
                                mission.objectifEmbeter.pnjAEmbeter.Remove(pnjTouche); //Retirer le PNJ de la liste
                                if (mission.objectifEmbeter.nbPnjAEmbeter == 0) //Si on a dérangé assez de PNJ alors mission réussie
                                {
                                    CompleterMission(i);
                                }
                            }
                            break;

                        case EMission.Blesser:
                            if(mission.objectifBlesser.pnjABlesser.Contains(pnjTouche)) //Si le PNJ touché fait partie de la liste
                            {
                                mission.objectifBlesser.nbPnjABlesser--; //Diminuer le nombre de pnj à déranger
                                mission.objectifBlesser.pnjABlesser.Remove(pnjTouche); //Retirer le PNJ de la liste
                                if (mission.objectifBlesser.nbPnjABlesser == 0) //Si on a blessé assez de PNJ alors mission réussie
                                {
                                    CompleterMission(i);
                                }
                            }
                            break;

                        case EMission.Tuer:
                            if(mission.objectifTuer.pnjATuer.Contains(pnjTouche)) //Si le PNJ touché fait partie de la liste
                            {
                                mission.objectifTuer.nbPnjATuer--; //Diminuer le nombre de pnj à tuer
                                mission.objectifTuer.pnjATuer.Remove(pnjTouche); //Retirer le PNJ de la liste
                                if (mission.objectifTuer.nbPnjATuer == 0) //Si on a tué assez de PNJ alors mission réussie
                                {
                                    CompleterMission(i);
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }
        }
    }

    public void CheckDestructionMission()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            SMission mission = missions[i]; //mission actuellement vérifiée
            if (mission.typeMission == EMission.Detruire && mission.etat == EEtatMission.Active) //Si c'est une mission de destruction active
            {
                mission.objectifDetruire.RefreshPourcentageDestruction(); //Refresh le nombre d'objets détruits
                if(mission.objectifDetruire.pourcentageActuel >= mission.objectifDetruire.pourcentage) //Si le pourcentage d'objets détruits est bon
                {
                    CompleterMission(i);
                }
            }
        }
    }
}
