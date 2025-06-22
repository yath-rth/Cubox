using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShockWave : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("Shock Wave")]
    [SerializeField, Range(0, 100f)] float radius;
    [SerializeField, Range(0, 1f)] float waveSpeed;
    [SerializeField, Range(0, 50f)] float waveImpactOffset, waveDamage;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] AnimationCurve timeScaleCurve;
    [SerializeField] Material waveMat;
    float tempRad;
    Collider[] enemiesCols;

    public IEnumerator shockWave()
    {
        tempRad = 0;
        float timer = 0, t1 = 0;

        agent.enabled = false;

        enemiesCols = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        if (enemiesCols.Length > 0)
        {
            for (int i = 0; i < enemiesCols.Length; i++)
            {
                enemiesCols[i].GetComponent<Enemy>().takeDamage();
            }
        }

        while (t1 <= 1)
        {
            enemiesCols = Physics.OverlapSphere(transform.position, tempRad, enemyLayer);

            if (enemiesCols.Length > 0)
            {
                for (int i = 0; i < enemiesCols.Length; i++)
                {
                    StartCoroutine(enemiesCols[i].GetComponent<Enemy>().waveImpact(waveImpactOffset));
                }
            }

            if (tempRad <= radius) tempRad += waveSpeed;

            Time.timeScale = timeScaleCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime;

            if(waveMat != null) waveMat.SetFloat("_Radius", t1 + .15f);
            t1 += Time.deltaTime;

            yield return null;
        }

        waveMat.SetFloat("_Radius", 0);
        agent.enabled = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
