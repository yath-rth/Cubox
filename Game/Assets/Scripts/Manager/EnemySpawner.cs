using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    ObjectPooler pool;
    grid Grid;
    [SerializeField] private float spawnTime = 4f;
    Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();

    private void Start()
    {
        pool = ObjectPooler.instance;
        Grid = GetComponent<grid>();
    }

    public void updateEnemies(ServerMessage msg)
    {
        if (msg.enemies == null) return;

        foreach (string id in msg.enemies.Keys)
        {
            if (!enemies.ContainsKey(id) && ObjectPooler.instance != null)
            {
                StartCoroutine(SpawnEnemy(msg, id));
            }

            if (enemies[id] == null) continue;

            //enemies[id].transform.position = msg.enemies[id].position;
            enemies[id].transform.DOMove(msg.enemies[id].position, 0.2f);
            enemies[id].transform.LookAt(msg.enemies[id].direction);

            DamageableItem healthUnit = enemies[id].GetComponent<DamageableItem>();
            if (healthUnit != null)
            {
                healthUnit.UpdateHealth(msg.enemies[id].health);
            }
        }

        foreach (string id in enemies.Keys.ToList())
        {
            if (!msg.enemies.ContainsKey(id))
            {
                if (ObjectPooler.instance == null) continue;

                enemies[id].transform.parent = pool.transform;
                pool.ReturnObject(enemies[id], 2);
                enemies.Remove(id);
            }
        }
    }

    // private void Update()
    // {
    //     if (player != null)
    //     {
    //         if (Time.time > nextCampCheckTime)
    //         {
    //             nextCampCheckTime = Time.time + timeBetweenCampingChecks;

    //             isCamping = (Vector3.Distance(player.gameObject.transform.position, campPositionOld) < campThresholdDistance);
    //             campPositionOld = player.gameObject.transform.position;
    //         }


    //         if (enemiesKilled > (wave * waveSpeed) && enemiesKilled < ((wave + 1) * waveSpeed) && enemiesKilled != 0)
    //         {
    //             spawnTime = Mathf.Clamp(spawnTime - spawnTimeDecrease, .1f, spawnTime);
    //             //maxEnemy = Mathf.Clamp(maxEnemy + 5, 0, 25);
    //             wave++;
    //         }

    //         if (isCamping) Debug.Log("U camper");
    //     }
    // }

    // //IEnumerator Wave()
    // {
    //     while (player.playerStats.getStat(StatTypes.hitpoints) > 0)
    //     {
    //         if (enemiesCount <= maxEnemy)
    //         {
    //             StartCoroutine(SpawnEnemy());
    //         }

    //         yield return new WaitForSeconds(spawnTime);
    //     }
    // }

    IEnumerator SpawnEnemy(ServerMessage msg, string id)
    {
        float spawnTimer = 0;
        GameObject newTile = Grid.getTileAtPosition(msg.enemies[id].position);

        enemies[id] = null;
        GameObject enemy = pool.GetObject(2);
        enemy.transform.position = msg.enemies[id].position;
        enemy.transform.LookAt(msg.enemies[id].direction);
        enemies[id] = enemy;

        if (newTile != null)
        {
            Material newTileMat = newTile.GetComponent<Renderer>().material;

            while (spawnTimer < spawnTime / 3)
            {
                newTileMat.SetFloat("Amt_Of_Outline", Mathf.PingPong(spawnTimer, 2));

                spawnTimer += Time.deltaTime;
                yield return null;
            }

            newTileMat.SetFloat("Amt_Of_Outline", 2);
        }
    }
}

[Serializable]
public class EnemyAttack : UnityEvent<int> { }
