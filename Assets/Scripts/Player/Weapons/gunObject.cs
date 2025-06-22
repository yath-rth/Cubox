using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun Object", order = 0)]
public class gunObject : Stats
{
    public gunType guntype;
    public GameObject gun;

    [Header("Properties")]
    public float bulletSpeed;
    public float range;
    public int ammo;
    public float reloadTime;

    [Header("Objects")]
    public AudioClip audioClip;

    [Header("Activity")]
    public bool active;
    public bool infiniteAmmo = false;

    
}

[Serializable]
public enum gunType{
    machineGun,
    Pistol,
    Shotgun,
}
