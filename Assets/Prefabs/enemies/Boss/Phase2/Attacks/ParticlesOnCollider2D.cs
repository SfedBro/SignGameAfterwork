using UnityEngine;

public class ParticlesOnCollider2D : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private PolygonCollider2D sourceCollider;
    [SerializeField]
    private int emissionAmount;
    private int realEmission;
    private void Start()
    {
        realEmission = Mathf.RoundToInt(emissionAmount * Time.fixedDeltaTime);
    }
    private void Update()
    {
        if (ps == null || sourceCollider == null)
            return;

        var emitParams = new ParticleSystem.EmitParams();

        for (int i = 0; i < realEmission; i++)
        {
            Vector2 point = GetRandomPointInCollider();
            emitParams.position = point;
            ps.Emit(emitParams, 1);
        }
    }
    Vector2 GetRandomPointInCollider()
    {
        Bounds bounds = sourceCollider.bounds;
        Vector2 point;
        int attempts = 0;
        do
        {
            point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
            attempts++;
            if (attempts > 1000) break;
        }
        while (!sourceCollider.OverlapPoint(point));
        return point;
    }
}
