using UnityEngine;

public class RotateRandomlyZ : MonoBehaviour
{
    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }
}
