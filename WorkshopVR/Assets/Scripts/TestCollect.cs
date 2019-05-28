using UnityEngine;

public class TestCollect : MonoBehaviour
{
    public Item itemToCollect;
    public SuiviCollecte[] suivis;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(SuiviCollecte suivi in suivis)
            {
                if (itemToCollect.item == suivi.item)
                {
                    suivi.nombreItemsCollectes++;
                    MissionManager.instance.CheckCollectMission(itemToCollect, suivi.nombreItemsCollectes);
                }
            }
        }
    }
}

[System.Serializable]
public class SuiviCollecte
{
    public SItem item;
    public int nombreItemsCollectes;
}