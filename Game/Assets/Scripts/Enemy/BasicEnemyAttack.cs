using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAttack : MonoBehaviour
{
    Player player;
    ObjectPooler pool;
    Enemy enemy;

    NavMeshAgent navAgent;
    MeshRenderer render;

    [Range(0f, 5f), SerializeField] float attackSpeed;

    public AnimationCurve animCurve;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        navAgent = GetComponent<NavMeshAgent>();
        render = GetComponent<MeshRenderer>();

        player = Player.playerInstance;
        pool = ObjectPooler.instance;
    }

    private void OnEnable()
    {
        enemy = GetComponent<Enemy>();
        navAgent = GetComponent<NavMeshAgent>();
        render = GetComponent<MeshRenderer>();

        player = Player.playerInstance;
        pool = ObjectPooler.instance;
    }

    public IEnumerator attack(int damage)
    {
        float percent = 0;
        Vector3 oldPos = transform.position;
        navAgent.enabled = false;

        while (percent < 1)
        {
            percent += Time.deltaTime * attackSpeed;

            float val = animCurve.Evaluate(percent);
            transform.position = Vector3.Lerp(oldPos, player.transform.position, val);

            yield return null;
        }

        if (!player.dashing)
        {
            StartCoroutine(player.takeDamagePlayer(damage));
        }

        render.enabled = false;
        yield return new WaitForSeconds(.25f);

        render.enabled = true;
        navAgent.enabled = true;

        enemy.finishedAttack();

        pool.ReturnObject(this.gameObject, 2);
    }

    public void StartAttack(int damage)
    {
        StartCoroutine(attack(damage));
    }
}
