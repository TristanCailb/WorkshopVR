﻿using UnityEngine;

public class IAReaction : MonoBehaviour
{
    public Animator animator;
    [Header("Rendering")]
    public bool generateRandomColors;
    public Genre genre;
    public Color[] couleursHaut;
    public Color[] couleursBas;
    public SkinnedMeshRenderer meshRenderer;
    [Header("Particules")]
    public GameObject bloodEffectHurt;
    public GameObject bloodEffectDie;

    private PatronController patron;

    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();

        patron = GetComponent<PatronController>(); //Récupérer le script du patron

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
                    OnBlesse(item, col.GetContact(0).point, Quaternion.Euler(col.GetContact(0).normal));
                    break;
                case EItemAction.Tue:
                    if(item.item.isBullet)
                    {
                        OnKillByGun(item, col.GetContact(0).point, Quaternion.Euler(col.GetContact(0).normal));
                    }
                    else
                    {
                        OnTue(item, col.GetContact(0).point, Quaternion.Euler(col.GetContact(0).normal));
                    }
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

    public void OnBlesse(Item item, Vector3 impactPos, Quaternion rotation)
    {
        animator.SetBool("IsHurt", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Blesse, this);
        Instantiate(bloodEffectHurt, impactPos, rotation); //Particules de sang 1

        if(patron != null)
        {
            patron.etat = PatronState.Mort;
            patron.StopMovement();
        }
    }

    public void OnTue(Item item, Vector3 impactPos, Quaternion rotation)
    {
        animator.SetBool("IsDead", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Tue, this);
        MissionManager.instance.FearEverybody(); //Effrayer tout le monde quand un PNJ meurt
        Instantiate(bloodEffectDie, impactPos, rotation); //Particules de sang 2

        if (patron != null)
        {
            patron.etat = PatronState.Mort;
            patron.StopMovement();
        }
    }

    public void OnKillByGun(Item item, Vector3 impactPos, Quaternion rotation)
    {
        //L'item est le projectile tiré
        animator.SetBool("IsDeadByGun", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Tue, this);
        MissionManager.instance.FearEverybody(); //Effrayer tout le monde quand un PNJ meurt
        Instantiate(bloodEffectDie, impactPos, rotation); //Particules de sang 2

        if (patron != null)
        {
            patron.etat = PatronState.Mort;
            patron.StopMovement();
        }
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
