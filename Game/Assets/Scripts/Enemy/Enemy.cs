using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public delegate void deathAction(int type, bool killedORattacked);
    public static event deathAction OnDeath;

    Player player;
    ObjectPooler pool;
    Points points;
    NavMeshAgent navAgent;
    MeshRenderer render;
    ParticleSystemRenderer deathParticles;
    Collider col;
    float maxHealth;
    bool powerupSpawned = false;

    [Header("Properties")]
    public bool isAlive = true;
    public float speed;
    public float health;
    public int damage;
    public float attackDis;
    public int enemyType;

    [Header("Materials")]
    public Material basic;
    public Material speedy;
    public Material rich;

    [Header("Misc")]
    public GameObject deathEffect;
    public bool isKilled = false, attacking = false;
    int damageTemp;
    Vector3 temp, finalOffsetPos;
    NavMeshHit hit;

    [Header("Events")]
    public EnemyAttack Attack;

    private void OnEnable()
    {
        Get();
        deathEffect.SetActive(false);
        maxHealth = health;
        isAlive = true;
        col.enabled = true;
        isKilled = false;
        if (navAgent != null) navAgent.speed = speed;
    }

    void Update()
    {
        if (player.Alive == true && isAlive == true)
        {
            move();
            checkHealth();
            col.enabled = true;
        }
    }

    void move()
    {
        if ((player.gameObject.transform.position - transform.position).magnitude > attackDis && player.Alive && !attacking)
        {
            transform.LookAt(player.gameObject.transform);
            navAgent.enabled = true;
            navAgent.SetDestination(player.gameObject.transform.position);
        }
        else if (!attacking)
        {
            Debug.Log("Attacking U U lil bitch");

            attacking = true;
            if (Attack != null) Attack.Invoke(damageTemp);
        }
    }

    public void takeDamage()
    {
        health -= player.GetDamage();
        checkHealth();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            takeDamage();

            //Add this part of the code if you want the bullets to only hit one target therefore increasing the difficulty of the game
            /*if (!player.WallBang)
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                pool.ReturnObject(other.gameObject, 1);
            }*/
        }
    }

    void checkHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health > 0)
        {
            isAlive = true;
        }
        else if (health <= 0)
        {
            StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        render.enabled = false;
        deathEffect.SetActive(true);
        navAgent.enabled = false;
        isAlive = false;

        yield return new WaitForSeconds(.05f);

        if (isKilled == false)
        {
            isKilled = true;
            if (OnDeath != null)
            {
                OnDeath(enemyType, true);
            }
        }

        if (PowerUpManager.instance != null) PowerUpManager.instance.SpawnPowerUp(transform.position);

        if (!powerupSpawned && points.getPoints() > (points.getWave() * 100) && points.getPoints() < ((points.getWave() + 1) * 100))
        {
            powerupSpawned = true;
            points.setWave(1);
        }

        yield return new WaitForSeconds(.95f);

        health = maxHealth;
        render.enabled = true;
        navAgent.enabled = true;
        col.enabled = true;
        attacking = false;

        isKilled = false;
        isAlive = true;

        deathEffect.SetActive(false);
        pool.ReturnObject(this.gameObject, 2);
    }

    public void SetProperties(int x, float Speed)
    {
        Material[] mat = render.materials;
        switch (x)
        {
            case 1:
                health = 50;
                speed = Speed;
                damageTemp = damage + 3;
                enemyType = x;
                mat[0] = basic;
                navAgent.speed = speed;
                deathParticles.material = mat[0];

                render.materials = mat;
                break;

            case 2:
                health = 25;
                speed = Speed + 2;
                damageTemp = damage;
                enemyType = x;
                mat[0] = speedy;
                navAgent.speed = speed;
                deathParticles.material = mat[0];

                render.materials = mat;
                break;

            case 3:
                maxHealth = 150;
                health = 150;
                speed = Mathf.Clamp(Speed - 4, 1, Speed);
                damageTemp = damage * 3;
                enemyType = x;
                mat[0] = rich;
                navAgent.speed = speed;
                deathParticles.material = mat[0];

                render.materials = mat;
                break;
        }
    }

    public void Get()
    {
        pool = ObjectPooler.instance;
        player = Player.playerInstance;
        points = Points.instance;

        render = GetComponent<MeshRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        col = GetComponent<BoxCollider>();

        if (deathEffect != null)
        {
            deathParticles = deathEffect.GetComponent<ParticleSystemRenderer>();
            deathEffect.SetActive(false);
        }
    }

    public void finishedAttack()
    {
        if (OnDeath != null) OnDeath(enemyType, false);
        health = maxHealth;
        render.enabled = true;
        navAgent.enabled = true;
        col.enabled = true;
        attacking = false;
    }

    public int getDamage()
    {
        return damageTemp;
    }

    public IEnumerator waveImpact(float offset)
    {
        navAgent.enabled = true;

        temp = -transform.forward * offset;

        navAgent.Raycast(temp, out hit);
        finalOffsetPos = hit.position;

        navAgent.enabled = false;

        float timer = 0;

        while (timer <= 1)
        {
            transform.position = Vector3.Lerp(transform.position, finalOffsetPos, timer);
            timer += Time.deltaTime;

            yield return null;
        }
    }
}
