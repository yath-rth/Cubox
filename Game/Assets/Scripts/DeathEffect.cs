using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(cleanUp());
    }

    public IEnumerator cleanUp()
    {
        yield return new WaitForSeconds(5f);
        DestroyImmediate(gameObject);
    }
}
