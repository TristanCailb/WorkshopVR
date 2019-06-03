using UnityEngine;

public class IAReaction : MonoBehaviour
{
    public Animator animator;
    [Header("Rendering")]
    public bool generateRandomColors;
    public Genre genre;
    public Color[] couleursHaut;
    public Color[] couleursBas;
    public SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();

        if(generateRandomColors) //Générer des couleurs random pour les habits des PNJ
        {
            if (genre == Genre.Homme)
            {
                Material matHaut = meshRenderer.materials[2];
                Material matBas = meshRenderer.materials[0];
                matHaut.color = couleursHaut[Random.Range(0, couleursHaut.Length)];
                matBas.color = couleursBas[Random.Range(0, couleursBas.Length)];
            }
            else
            {
                Material matHaut = meshRenderer.materials[2];
                Material matBas = meshRenderer.materials[0];
                matHaut.color = couleursHaut[Random.Range(0, couleursHaut.Length)];
                matBas.color = couleursBas[Random.Range(0, couleursBas.Length)];
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.GetComponent<Item>() != null)
        {
            Item item = col.collider.GetComponent<Item>();
            switch(item.item.action)
            {
                case EItemAction.Derange:
                    OnDerange(item);
                    break;
                case EItemAction.Blesse:
                    OnBlesse(item);
                    break;
                case EItemAction.Tue:
                    OnTue(item);
                    break;
                default: break;
            }
        }
    }

    public void OnDerange(Item item)
    {
        animator.SetBool("IsDerange", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Derange, this);
    }

    public void OnBlesse(Item item)
    {
        animator.SetBool("IsHurt", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Blesse, this);
    }

    public void OnTue(Item item)
    {
        animator.SetBool("IsDead", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Tue, this);
    }

    public void OnKillByGun(Item item)
    {
        //L'item est le projectile tiré
        animator.SetBool("IsDeadByGun", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Tue, this);
    }

    public void OnFear()
    {
        //Quand les PNJ ont peur
        foreach(AnimatorControllerParameter param in animator.parameters)
        {
            if(param.name == "IsFear")
            {
                animator.SetBool("IsFear", true); //Vérifie si l'animator a le paramètre et si c'est le cas, activer le bool
            }
        }
    }
}

public enum Genre
{
    Homme,
    Femme
}
