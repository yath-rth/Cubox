using System;
using DG.Tweening;
using UnityEngine;

class PlayerNetworkObject : MonoBehaviour
{
    string id;
    [SerializeField] MeshRenderer graphics;
    Color color;
    Gun gun;

    public void SetUp(ServerMessage msg, string id)
    {
        this.id = id;
        gun = GetComponent<Gun>();

        if (msg.players != null)
        {
            MaterialPropertyBlock mat = new MaterialPropertyBlock();
            graphics.GetPropertyBlock(mat);

            if (ColorUtility.TryParseHtmlString(msg.players[id].color, out color))
            {
                mat.SetColor("_color", color);
            }

            graphics.SetPropertyBlock(mat);
        }
    }

    public void UpdateTransforms(PlayerDTO _transform, bool both)
    {
        transform.DOMove(_transform.position, 0.1f);
        if (both) transform.DORotate(_transform.rotation, 0.3f);

        DamageableItem item = GetComponent<DamageableItem>();
        if (item != null)
        {
            item.UpdateHealth(_transform.health, color);
        }
    }

    public void Shoot()
    {
        if(gun == null) return;
        StartCoroutine(gun.Shoot());
    }
}