using System.Collections.Generic;
using UnityEngine;

public class Phase2 : MonoBehaviour
{
    [SerializeField]
    private Dictionary<GameObject, (GameObject, float)> attackPrefAndWarn;
    [SerializeField]
    private List<Coroutine> attackCoroutines;
    [SerializeField]
    private float attackTimeout;
    [SerializeField]
    private List<GameObject> enemyPrefabs;
    [SerializeField]
    private Dictionary<string, Color> elementsDict;
    [SerializeField]
    private int lastAttackIndex;
}
