using UnityEngine;

public class RotateCorresponding : MonoBehaviour
{
    [SerializeField]
    private Transform correspondingTransform;
    private void Awake()
    {
        correspondingTransform = FindFirstObjectByType<RotateRandomlyZ>().transform;
        transform.rotation = correspondingTransform.rotation;
    }
}
