using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinCount = 10;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float torqueForce = 2f;
    [SerializeField] private float lifetime = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerExplosion();
        }
    }
    public void SetCoinCount(int coins)
    {
        coinCount = coins;
    }
    public void TriggerExplosion()
    {
        if (coinPrefab == null)
        {
            Debug.LogError("Coin Prefab is not assigned in ExplosionController!", this);
            return;
        }

        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;

                rb.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);

                float randomTorque = Random.Range(-torqueForce, torqueForce);
                rb.AddTorque(randomTorque, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody2D not found on instantiated coin!", coin);
            }

            Destroy(coin, lifetime + Random.Range(-0.3f, 0.3f));
        }
    }
}