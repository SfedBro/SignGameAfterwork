using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAttack : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs;
    [SerializeField]
    private int amountToSpawn;
    private int spawned;
    [SerializeField]
    private float delay;
    public Phases phases;
    public Coroutine coroutine;
    void Start()
    {
        if (phases == null)
        {
            phases = FindFirstObjectByType<Phases>();
        }
        enemyPrefabs = phases.enemyPrefabs;
        coroutine = StartCoroutine(SpawnEnemiesForBoss());
    }
    IEnumerator SpawnEnemiesForBoss()
    {
        while (spawned < amountToSpawn)
        {
            yield return new WaitForSeconds(delay);
            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        GameObject pref = enemyPrefabs[Random.Range(0, (enemyPrefabs.Count - 1))];
        GameObject enemy = Instantiate(pref, transform.position, pref.transform.rotation);
        spawned++;
    }
}
