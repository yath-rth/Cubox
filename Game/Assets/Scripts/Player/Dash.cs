using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dash : MonoBehaviour
{
    Player player;
    NavMeshAgent agent;
    ShockWave shockwave;

    [Range(0, 50f)] public float dashForce, dashCoolDown;
    [SerializeField, Range(0, 4f)] float dashRadius;
    [SerializeField] AnimationCurve dashSpeedCurve;
    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] LayerMask enemyLayer;

    Vector3 dashpos, input, temp;
    float timer = 0, canDash;
    Collider[] enemies;

    private void Awake()
    {
        player = GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();

        //player.newInput.WSAD.Dash.performed += _ => StartCoroutine(dash());
    }

    private void Update()
    {
        if(player.dashing) return;

        if (canDash <= Time.time && player != null)
        {
            if (player.getDashInput() > 0)
            {
                canDash = Time.time + dashCoolDown;
                StartCoroutine(dash());
            }
        }
    }

    IEnumerator dash()
    {
        agent.enabled = true;
        UnityEngine.AI.NavMeshHit hit;
        player.dashing = true;

        input = player.getMoveInput().normalized;
        //temp = input.magnitude > 0 ? transform.position + (input * dashForce) : transform.position + (transform.forward * dashForce);
        temp = transform.position + (transform.forward * dashForce);
        agent.Raycast(temp, out hit);

        //dashpos = input.magnitude > 0 ? transform.position + (input * hit.distance) : transform.position + (transform.forward * hit.distance);
        dashpos = transform.position + (transform.forward * hit.distance);

        timer = 0;
        agent.enabled = false;

        while (timer < dashSpeedCurve.keys[dashSpeedCurve.length - 1].time)
        {
            timer += Time.deltaTime;

            if (Vector3.Distance(transform.position, dashpos) < .05f)
            {
                agent.enabled = true;
                player.dashing = false;
                yield break;
            }

            transform.position = Vector3.Lerp(transform.position, dashpos, dashSpeedCurve.Evaluate(timer));

            enemies = Physics.OverlapSphere(transform.position, dashRadius, enemyLayer);

            foreach (var item in enemies)
            {
                item.GetComponent<Enemy>().health = 0;
            }

            yield return null;
        }

        StartCoroutine(shockwave.shockWave());
    }

    public float getCurrTime(){
        return canDash;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashRadius);
    }
}
