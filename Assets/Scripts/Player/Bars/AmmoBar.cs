using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class AmmoBar : MonoBehaviour
{
    Gun gun;
    GameObject[] ammoBars;
    int count, gap, num;

    public Color Red;

    private void Start()
    {
        gun = Player.playerInstance.GetComponent<Gun>();
        ammoBars = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            ammoBars[i] = transform.GetChild(i).gameObject;
            ammoBars[i].SetActive(true);
        }

        num = ammoBars.Length;

        if (gun != null) gap = (int)gun.activeGun.ammo / num;
    }

    private void LateUpdate()
    {
        if (gun != null)
        {
            count = num - (int)System.Math.Ceiling((double)gun.currAmmo / gap);

            for (int i = 0; i < count; i++)
            {
                if(i < num) ammoBars[i].SetActive(false);
            }

            for (int i = num - 1; i >= count; i--)
            {
                if(i < num && i > -1) ammoBars[i].SetActive(true);
            }
        }
    }
}
