using System;
using DG.Tweening;
using UnityEngine;

class PlayerNetworkObject : MonoBehaviour
{
    public string id;
    public MeshRenderer graphics;

    public void SetUp(ServerMessage msg, string id)
    {
        this.id = id;
        if (msg.players != null)
        {
            MaterialPropertyBlock mat = new MaterialPropertyBlock();
            graphics.GetPropertyBlock(mat);

            Color color;
            if (ColorUtility.TryParseHtmlString(msg.players[id].color, out color))
            {
                mat.SetColor("_color", color);
            }

            graphics.SetPropertyBlock(mat);
        }
    }

    public void UpdateTransforms(PlayerDTO _transform)
    {
        transform.DOMove(_transform.position, 0.5f);
        transform.DORotate(_transform.rotation, 0.3f);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.DOMove(position, 0.2f);
    }
}