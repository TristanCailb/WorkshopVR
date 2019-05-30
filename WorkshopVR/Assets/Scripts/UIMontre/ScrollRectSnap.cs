using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour
{
    public RectTransform menusHolder;           //Panel qui contient les menus
    public GameObject[] menus;                  //Tous les menus contenus dans le panel Content
    public RectTransform centerToCompare;       //Centre du panel principal

    private float[] distances;                  //Distances de chaque menu par rapport au centre
    private bool dragging = false;              //Passe à true quand on drag le panel
    private int menusDistance;                  //Distance entre chaque menu
    private int minMenuIndex;                   //Index du menu le plus proche du centre

    void Start()
    {
        int menuLength = menus.Length; //Nombre de menus dans l'array
        distances = new float[menuLength]; //Créer un nouvel array de float de la taille du nombre de menus
        // Calculer la distance entre 2 menus
        menusDistance = (int)Mathf.Abs(menus[1].GetComponent<RectTransform>().anchoredPosition.x - menus[0].GetComponent<RectTransform>().anchoredPosition.x);
    }

    void Update()
    {
        for(int i = 0; i < menus.Length; i++)
        {
            distances[i] = Mathf.Abs(centerToCompare.transform.position.x - menus[i].transform.position.x); //Actualiser les distances des menus par rapport au centre
        }
        float minDistance = Mathf.Min(distances); //Récupérer la distance la plus petite
        for (int a = 0; a < menus.Length; a++)
        {
            if(minDistance == distances[a])
            {
                minMenuIndex = a; //Actualiser l'index du menu le plus proche du centre
            }
        }

        if(!dragging)
        {
            LerpToMenu(minMenuIndex * -menusDistance);
        }
    }

    private void LerpToMenu(int position)
    {
        float newX = Mathf.Lerp(menusHolder.anchoredPosition.x, position, Time.deltaTime * 2f); //Lerp la nouvelle position
        Vector2 newPos = new Vector2(newX, menusHolder.anchoredPosition.y);

        menusHolder.anchoredPosition = newPos;
    }

    public void StartDrag()
    {
        dragging = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }
}
