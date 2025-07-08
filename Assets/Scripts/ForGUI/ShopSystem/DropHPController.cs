using UnityEngine;

public class DropHPController : MonoBehaviour
{
    [SerializeField] private GameObject dropItem;
    [SerializeField] private Transform torgashTransform;

    public void DropHP()
    {
        Instantiate(dropItem, torgashTransform.position + new Vector3(-1, 0, 0), Quaternion.identity);
        Debug.Log("упал");
    }
}
