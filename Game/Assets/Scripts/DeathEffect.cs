using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] float duration = 3f;

    void Awake()
    {
        StartCoroutine(cleanUp());
    }

    public IEnumerator cleanUp()
    {
        yield return new WaitForSeconds(duration);
        DestroyImmediate(gameObject);
    }
}
