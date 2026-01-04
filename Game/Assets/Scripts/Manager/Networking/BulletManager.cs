using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

class BulletManager : MonoBehaviour
{
    Dictionary<string, GameObject> bullets = new Dictionary<string, GameObject>();

    public void updateBullets(ServerMessage msg)
    {
        if (msg.bullets == null) return;

        foreach (string id in msg.bullets.Keys)
        {
            if (bullets == null) continue;

            if (!bullets.ContainsKey(id) && ObjectPooler.instance != null)
            {
                GameObject bullet = ObjectPooler.instance.GetObject(1);
                bullets[id] = bullet;

                DOTween.Kill(bullets[id].transform);

                bullet.transform.rotation = Quaternion.LookRotation(msg.bullets[id].direction);
            }

            bullets[id].transform.position = msg.bullets[id].position;
            bullets[id].transform.DOMove(msg.bullets[id].position, 0.2f);
            bullets[id].transform.LookAt(msg.bullets[id].direction);

            if (msg.bullets[id].lifetime <= 0)
            {
                if (ObjectPooler.instance == null) continue;

                bullets[id].transform.parent = ObjectPooler.instance.transform;
                ObjectPooler.instance.ReturnObject(bullets[id], 1);
                bullets.Remove(id);
            }
        }

        foreach (string id in bullets.Keys.ToList())
        {
            if (!msg.bullets.ContainsKey(id))
            {
                if (ObjectPooler.instance == null) continue;

                bullets[id].transform.parent = ObjectPooler.instance.transform;
                ObjectPooler.instance.ReturnObject(bullets[id], 1);
                bullets.Remove(id);
            }
        }
    }
}