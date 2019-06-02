using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    public Vector3 offset;

    void FixedUpdate()
    {
        if(player == null)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("Player");
            if(temp != null)
            {
                player = temp.transform;
            }
        }
        else
        {
            transform.position = player.position + offset;
        }
    }
}
