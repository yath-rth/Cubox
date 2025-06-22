using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    ObjectPooler pool;

    public Rigidbody rb;
    public float forceMin;
    public float forceMax;

    float lifetime = 1;

    private void Start() {
        pool = ObjectPooler.instance;
    }

    public void Spawned(){
        float force = Random.Range(forceMin, forceMax);
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        rb.linearVelocity = Vector3.zero;

        pool.ReturnObject(gameObject, 3);
    }
}
