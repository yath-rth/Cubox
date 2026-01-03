using UnityEngine;

[CreateAssetMenu(menuName = "Gun Object", order = 0)]
public class gunObject : Stats, InventoryItemSO
{
    string InventoryItemSO.displayName { get => Name; }
    Sprite InventoryItemSO.image{ get => sprite; }
    int InventoryItemSO.quantity{ get => quantity; }

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

    public void EquipItem()
    {
        if (Player.playerInstance != null)
        {
            Player.playerInstance.gun.activeGun = this;
            Player.playerInstance.gun.SetUpGun();
        }
    }

    public void AddItem()
    {
        if (Inventory.instance != null)
        {
            Inventory.instance.objects.Add(this);
        }
    }
}