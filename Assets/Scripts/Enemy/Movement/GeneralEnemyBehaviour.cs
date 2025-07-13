using UnityEngine;

[RequireComponent(typeof(LayerMask))]
public class GeneralEnemyBehaviour
{
    public static bool LookingDirectlyAtPlayer(Vector3 p1, Vector3 p2, float visionDistance, LayerMask masks, string tag)
    {
        if ((p1 - p2).magnitude <= visionDistance) {
            RaycastHit2D hit = Physics2D.Linecast(p1, p2, masks);
            if (hit)
            {
                if (hit.collider.tag == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool LookingDirectlyAtPosition(Vector3 p1, Vector3 p2, LayerMask masks)
    {
        RaycastHit2D hit = Physics2D.Linecast(p1, p2, masks);
        return !hit;
    }
    //To do
    public static void DealDamage(GameObject player, float damage)
    {
        
    }
}
