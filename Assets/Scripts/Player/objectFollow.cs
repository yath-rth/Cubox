using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class objectFollow : MonoBehaviour
{
    Vector3 pos, pos1, pos2;
    [SerializeField] Transform followObj;

    Vector3 offset;

    private void Awake()
    {
        pos1 = new Vector3(Mathf.Abs(followObj.position.x), Mathf.Abs(followObj.position.y), Mathf.Abs(followObj.position.z));
        pos2 = new Vector3(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y), -Mathf.Abs(transform.position.z));
        offset = pos2 - pos1;
    }

    void FixedUpdate()
    {
        pos = followObj.position + offset;

        transform.position = Vector3.Lerp(transform.position, pos, 1f);
    }
}
