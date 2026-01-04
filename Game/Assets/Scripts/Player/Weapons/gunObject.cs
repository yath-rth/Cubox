using UnityEngine;

[CreateAssetMenu(menuName = "Gun Object", order = 0)]
public class gunObject : Stats
{
    public GameObject gun;
    string Name;
    public Sprite sprite;
    public int quantity;

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