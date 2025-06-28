using System;
using UnityEngine;

public class ShootSpell : MonoBehaviour
{
    public float force;
    public float lifeDistance = 20f;
    public bool lookRight;
    public string effectType;
    public Vector3 cursorPos;

    private Rigidbody2D physic;
    private Vector3 origPos;

    private void Start()
    {
        origPos = transform.position;
        physic = GetComponent<Rigidbody2D>();
        physic.AddForce(force * (cursorPos - origPos).normalized);
    }

    void Update()
    {
        CheckIfAlive();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            Debug.Log("Collision!");
            EffectsManager.Instance.effect.ApplyEffect(gameObject, collider.gameObject, effectType);
            Destroy(gameObject);
        }
    }

    private void CheckIfAlive()
    {
        if (Math.Abs((transform.position - origPos).magnitude) >= lifeDistance)
        {
            Destroy(gameObject);
        }
    }
}
