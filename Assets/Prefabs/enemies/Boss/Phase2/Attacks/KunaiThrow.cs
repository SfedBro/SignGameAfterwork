using System.Collections.Generic;
using UnityEngine;

public class KunaiThrow : MonoBehaviour
{
    [Header("Kunai Parameters")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private int kunaiDamage = 10;
    [SerializeField] private float kunaiSpeed = 5f;

    [Header("Throw Parameters")]
    [SerializeField] private int thrownAmount = 5;
    [SerializeField, Range(0f, 180f)] private float throwingArc = 60f;
    [SerializeField] private float throwOffset = 0.5f;

    [Header("Testing")]
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    void Start()
    {
        if (thrownAmount <= 0) return;
        if (thrownAmount == 1)
        {
            Quaternion rotation = transform.rotation;
            Vector3 spawnPos = transform.position + rotation * Vector3.right * throwOffset;
            GameObject newKunai = Instantiate(kunaiPrefab, spawnPos, rotation, this.transform);
            SimpleDamagingScript sds = newKunai.GetComponent<SimpleDamagingScript>();
            if (sds != null)
            {
                sds.Damage = kunaiDamage;
                sds.Speed = kunaiSpeed;
            }
            objects.Add(newKunai);
            return;
        }
        float angleStep = throwingArc / (thrownAmount - 1);
        float arcStart = -throwingArc / 2f;
        for (int i = 0; i < thrownAmount; i++)
        {
            float localAngle = arcStart + angleStep * i;
            Quaternion rotation = Quaternion.AngleAxis(localAngle, Vector3.forward) * transform.rotation;
            Vector3 spawnPos = transform.position + rotation * Vector3.right * throwOffset;
            GameObject newKunai = Instantiate(kunaiPrefab, spawnPos, rotation, this.transform);
            SimpleDamagingScript sds = newKunai.GetComponent<SimpleDamagingScript>();
            if (sds != null)
            {
                sds.Damage = kunaiDamage;
                sds.Speed = kunaiSpeed;
            }
            objects.Add(newKunai);
        }
    }
    private void OnDrawGizmos()
    {
        if (thrownAmount <= 0) return;
        Vector3 origin = transform.position;
        float angleStep = thrownAmount > 1 ? throwingArc / (thrownAmount - 1) : 0f;
        float arcStart = -throwingArc / 2f;

        Gizmos.color = Color.blue;
        int arcSegments = 20;
        Vector3 prevPoint = origin + (Quaternion.AngleAxis(arcStart, Vector3.forward) * transform.rotation) * Vector3.right * 3f;
        Gizmos.DrawLine(prevPoint, origin);
        for (int i = 1; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float angle = Mathf.Lerp(arcStart, arcStart + throwingArc, t);
            Vector3 nextPoint = origin + (Quaternion.AngleAxis(angle, Vector3.forward) * transform.rotation) * Vector3.right * 3f;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
        Gizmos.DrawLine(prevPoint, origin);

        Gizmos.color = Color.red;
        if (thrownAmount == 1)
        {
            Quaternion rotation = transform.rotation;
            Vector3 arcPoint = origin + rotation * Vector3.right * throwOffset;
            Gizmos.DrawRay(arcPoint, rotation * Vector3.right * 1f);
        }
        else
        {
            for (int i = 0; i < thrownAmount; i++)
            {
                float localAngle = arcStart + angleStep * i;
                Quaternion rotation = Quaternion.AngleAxis(localAngle, Vector3.forward) * transform.rotation;
                Vector3 arcPoint = origin + rotation * Vector3.right * throwOffset;
                Gizmos.DrawRay(arcPoint, rotation * Vector3.right * 1f);
            }
        }
    }
}