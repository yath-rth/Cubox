using System;
using DG.Tweening;
using UnityEngine;

class PlayerNetworkObject: MonoBehaviour
{
    public String id;

    public void SetUp(string id)
    {
        this.id = id;
    }

    public void UpdateTransforms(TransformDTO _transform)
    {
        transform.DOMove(_transform.position, 0.2f);
        transform.DORotate(_transform.rotation, 0.2f);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.DOMove(position, 0.2f);
    }
}