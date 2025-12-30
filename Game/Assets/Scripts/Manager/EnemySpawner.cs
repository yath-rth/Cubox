using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    Player player;
    Points points;
    ObjectPooler pool;
    grid Grid;
    int enemiesKilled = 0;
    int enemiesCount = 0;
    int wave = 1;
    int richEnemy = 0;

    public int maxEnemy = 10;
    public float spawnTime = 2;
    public float spawnTimeDecrease = .2f;
    public float enemySpeed;
    public int waveSpeed = 25;
    public List<WeightedVal> enemyTypes;
    public Color flashColour = Color.red;
    Color initialColour;

    private float nextCampCheckTime;
    bool isCamping;
    public float timeBetweenCampingChecks;
    private Vector3 campPositionOld;
    public float campThresholdDistance;

    private void Start()
    {
        player = Player.playerInstance;
        pool = ObjectPooler.instance;
        points = Points.instance;
        Grid = GetComponent<grid>();

        StartCoroutine(Wave());
    }

    private void OnEnable()
    {
        Enemy.OnDeath += enemyKilled;
    }

    private void OnDisable()
    {
        Enemy.OnDeath -= enemyKilled;
    }

    void enemyKilled(int i, bool a)
    {
        if (a)
        {
            if (i == 3)
            {
                richEnemy--;
            }
            enemiesKilled++;
            enemiesCount--;
        }
        else
        {
            enemiesCount--;
        }
    }

    private void Update()
    {
        if (player.Alive)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(player.gameObject.transform.position, campPositionOld) < campThresholdDistance);
                campPositionOld = player.gameObject.transform.position;
            }


            if (enemiesKilled > (wave * waveSpeed) && enemiesKilled < ((wave + 1) * waveSpeed) && enemiesKilled != 0)
            {
                spawnTime = Mathf.Clamp(spawnTime - spawnTimeDecrease, .1f, spawnTime);
                //maxEnemy = Mathf.Clamp(maxEnemy + 5, 0, 25);
                wave++;
            }

            if (isCamping) Debug.Log("U camper");
        }
    }

    IEnumerator Wave()
    {
        while (player.playerStats.getStat(StatTypes.hitpoints) > 0)
        {
            if (enemiesCount <= maxEnemy)
            {
                StartCoroutine(SpawnEnemy());
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    IEnumerator SpawnEnemy()
    {
        //Vector3 spawnPos = GetRandomPointAbove(GetComponent<BoxCollider>());

        float spawnTimer = 0;
        GameObject newTile;

        if (!isCamping)
        {
            newTile = Grid.getRandomPos();
        }
        else
        {
            //newTile = Grid.getRandomPosNearPlayer();
            newTile = Grid.getRandomPos();
        }

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

            if (newTile != null)
            {
                Vector3 spawnPos = newTile.transform.position + new Vector3(0, 1, 0);
                GameObject enemy = pool.GetObject(2);

                if (enemy != null)
                {
                    enemy.transform.position = spawnPos;
                    Enemy enemy_script = enemy.GetComponent<Enemy>();

                    int x = RandomVal.GetRandmVal(enemyTypes);
                    if (x == 3)
                    {
                        if (richEnemy < 3)
                        {
                            enemy_script.SetProperties(x, enemySpeed);
                            richEnemy++;
                        }
                        else
                        {
                            x = RandomVal.GetRandmVal(enemyTypes);
                            enemy_script.SetProperties(x, enemySpeed);
                        }
                    }
                    else
                    {
                        enemy_script.SetProperties(x, enemySpeed);
                    }

                    enemiesCount++;
                    yield return null;
                }
            }
        }
    }

    Vector3 GetRandomPointAbove(BoxCollider mesh)
    {
        float minx = mesh.bounds.min.x;
        float minz = mesh.bounds.min.z;
        float maxx = mesh.bounds.max.x;
        float maxz = mesh.bounds.max.z;
        float x = UnityEngine.Random.Range(minx, maxx);
        float y = 0;
        float z = UnityEngine.Random.Range(minz, maxz);
        var localPos = new Vector3(x, y, z);
        return transform.TransformPoint(localPos);
    }
}

[Serializable]
public class EnemyAttack : UnityEvent<int> {}
