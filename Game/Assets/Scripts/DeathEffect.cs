using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public IEnumerator cleanUp()
    {
        yield return new WaitForSeconds(5f);
        DestroyImmediate(gameObject);
    }
}
