using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Phases : MonoBehaviour
{
    public List<AttackInfo> attackPrefAndWarn;
    public float attackTimeout;
    public List<GameObject> enemyPrefabs;

    public List<AttackInfo> attackPrefAndWarn2;
    public float attackTimeout2;
    public List<GameObject> enemyPrefabs2;

    int lastAttackIndex = -1;
    public Coroutine curPhaseCoroutine;
    public bool isPhaseInProcess;

    //void Start() {
    //    StartPhaseOne();
    //}

    public void StartPhaseOne() {
        curPhaseCoroutine = StartCoroutine(PhaseCoroutine());
    }
    public void StartPhaseTwo()
    {
        curPhaseCoroutine = StartCoroutine(WaitForEndOfThePhase());
    }

    IEnumerator PhaseCoroutine() {
        while (isPhaseInProcess) {
            AttackInfo attack = attackPrefAndWarn.Where((at, i) => i != lastAttackIndex).OrderBy(at => Random.value).First();
            lastAttackIndex = attackPrefAndWarn.IndexOf(attack);
            StartCoroutine(PerformAttack(attack));
            yield return new WaitForSeconds(attackTimeout);
        }
        curPhaseCoroutine = null;
    }

    IEnumerator PerformAttack(AttackInfo attack) {
        GameObject warn = Instantiate(attack.warnPrefab, transform, false);
        yield return new WaitForSeconds(attack.timeBeforeAttack);
        Destroy(warn);
        GameObject curAttack = Instantiate(attack.attackPrefab, transform, false);
        yield return new WaitForSeconds(attack.attackTime);
        Destroy(curAttack);
    }
    IEnumerator WaitForEndOfThePhase()
    {
        while (isPhaseInProcess || curPhaseCoroutine != null)
        {
            yield return new WaitForEndOfFrame();
        }
        curPhaseCoroutine = StartCoroutine(PhaseTwoCoroutine());
        isPhaseInProcess = true;
    }
    IEnumerator PhaseTwoCoroutine()
    {
        while (isPhaseInProcess)
        {
            AttackInfo attack = attackPrefAndWarn2.Where((at, i) => i != lastAttackIndex).OrderBy(at => Random.value).First();
            lastAttackIndex = attackPrefAndWarn2.IndexOf(attack);
            StartCoroutine(PerformAttack(attack));
            yield return new WaitForSeconds(attackTimeout2);
        }
        curPhaseCoroutine = null;
    }
    IEnumerator TimeoutAfterAttack(float time)
    {
        transform.position = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(time);
    }
}

[Serializable]
public struct AttackInfo {
    public GameObject attackPrefab;
    public GameObject warnPrefab;
    public float timeBeforeAttack;
    public float attackTime;
}
