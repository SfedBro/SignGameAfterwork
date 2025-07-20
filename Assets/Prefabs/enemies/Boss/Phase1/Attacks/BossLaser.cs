using System.Linq;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    public float laserMaxDistance = 10;
    public int damage = 1;
    public LayerMask ignoredMasks;

    LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update() {
        ShootLaser();
    }

    void ShootLaser() {
        RaycastHit2D circleHit = Physics2D.CircleCast(transform.position, lineRenderer.startWidth / 2, transform.right, laserMaxDistance, ~ignoredMasks);
        if (circleHit) {
            DrawLaser(transform.position, transform.position + transform.right * Vector2.Distance(transform.position, circleHit.point));
            if (circleHit.transform.gameObject.CompareTag("Player")) circleHit.transform.GetComponent<Player>().TakeDamage(damage);
        } else DrawLaser(transform.position, transform.position + transform.right * laserMaxDistance);
    }

    void DrawLaser(Vector3 startPos, Vector3 endPos) {
        lineRenderer.SetPositions(new[]{startPos, endPos});
    }
}
