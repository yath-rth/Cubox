using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

class BulletManager : MonoBehaviour
{
    Dictionary<string, GameObject> bullets = new Dictionary<string, GameObject>();

    public void updateBullets(ServerMessage msg, Dictionary<string, PlayerNetworkObject> players)
    {
        if (msg.bullets == null) return;
        if (msg.players == null) return;

        foreach (string id in msg.bullets.Keys)
        {
            if (bullets == null) continue;
            //if (!msg.players.ContainsKey(msg.bullets[id].owner)) continue;

            if (msg.players[msg.bullets[id].owner].isReloading == 1)
            {
                players[msg.bullets[id].owner].Reload();
            }
            else
            {
                if (!bullets.ContainsKey(id) && ObjectPooler.instance != null)
                {
                    GameObject bullet = ObjectPooler.instance.GetObject(1);
                    bullets[id] = bullet;
                    bullet.transform.rotation = Quaternion.LookRotation(msg.bullets[id].direction);

                    if (players[msg.bullets[id].owner] != null)
                    {
                        players[msg.bullets[id].owner].Shoot();
                    }
                }
            }

            if(!bullets.ContainsKey(id)) continue;

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