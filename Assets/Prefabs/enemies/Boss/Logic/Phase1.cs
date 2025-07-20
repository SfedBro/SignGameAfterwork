using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Phase1 : MonoBehaviour
{
    public List<AttackInfo> attackPrefAndWarn;
    public float attackTimeout;
    public List<GameObject> enemyPrefabs;

    int lastAttackIndex = -1;
    public Coroutine curPhaseCoroutine;

    public void StartPhase() {
        curPhaseCoroutine = StartCoroutine(PhaseCoroutine());
    }

    IEnumerator PhaseCoroutine() {
        while (true) {
            AttackInfo attack = attackPrefAndWarn.Where((at, i) => i != lastAttackIndex).OrderBy(at => Random.value).First();
            lastAttackIndex = attackPrefAndWarn.IndexOf(attack);
            StartCoroutine(PerformAttack(attack));
            yield return new WaitForSeconds(attackTimeout);
        }
    }

    IEnumerator PerformAttack(AttackInfo attack) {
        GameObject warn = Instantiate(attack.warnPrefab, transform, false);
        yield return new WaitForSeconds(attack.timeBeforeAttack);
        Destroy(warn);
        Instantiate(attack.attackPrefab, transform, false);
    }
}

[Serializable]
public struct AttackInfo {
    public GameObject attackPrefab;
    public GameObject warnPrefab;
    public float timeBeforeAttack;
}
