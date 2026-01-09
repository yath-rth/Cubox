using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Gun : MonoBehaviour
{
    public List<gunObject> gunTypes;
    public gunObject activeGun;

    public bool isReloading = false;
    public int currAmmo = 0;

    float canFire, input;


    [SerializeField] Vector3 reloadRotation;
    public Transform gunParent;

    Player player;
    ObjectPooler pool;
    AudioSource audioSource;
    Transform gunInstance = null;

    [Header("Objects")]
    Transform shellSpawn;
    List<ParticleSystem> muzzleFlash = new List<ParticleSystem>();
    List<GameObject> flashLight = new List<GameObject>();
    public List<Transform> muzzle = new List<Transform>();

    private void Start()
    {
        pool = ObjectPooler.instance;
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();

        foreach (gunObject item in gunTypes)
        {
            if (item.active)
            {
                activeGun = item;
                //item.active = false;
                break;
            }
        }

        if (activeGun == null) activeGun = gunTypes[Random.Range(0, gunTypes.Count)];
        SetUpGun();

        if(player != null)
        {
            //player.newInput.WSAD.Shoot.performed += _ => StartCoroutine(Shoot());
            player.playerStats.setStat(StatTypes.damage, activeGun.getStat(StatTypes.damage));
        }
    }

    public void SetUpGun()
    {
        if (gunInstance != null) Destroy(gunInstance.gameObject);

        if (activeGun != null)
        {
            gunInstance = Instantiate(activeGun.gun, gunParent).transform;
            Debug.Log(gunInstance.name);

            muzzle.Clear();
            flashLight.Clear();
            muzzleFlash.Clear();

            for (int i = 0; i < gunInstance.childCount; i++)
            {
                Transform item = gunInstance.transform.GetChild(i);

                if (item.CompareTag("muzzle") && item != null)
                {
                    muzzle.Add(item);
                    flashLight.Add(item.Find("Flash").gameObject);
                    muzzleFlash.Add(item.Find("Muzzle Flash").GetComponent<ParticleSystem>());
                }

                if (item.gameObject.name == "Mag") shellSpawn = item;
            }

            currAmmo = activeGun.ammo;
            if (player != null) player.playerStats.setStat(StatTypes.damage, activeGun.getStat(StatTypes.damage));
        }
    }

    public void muzzleUnlock(int muzzleCount)
    {
        int num = muzzle.Count;
        int count = 0;

        for (int i = 0; i < num; i++)
        {
            if (muzzle[i].gameObject.activeInHierarchy) count++;
        }

        muzzleCount += count;

        for (int i = 0; i < muzzleCount; i++)
        {
            if (i < num) muzzle[i].gameObject.SetActive(true);
        }

        for (int i = num - 1; i >= muzzleCount; i--)
        {
            if (i > 0) muzzle[i].gameObject.SetActive(false);
        }
    }

    public IEnumerator Shoot()
    {
        if (pool != null)
        {
            for (int i = 0; i < muzzle.Count; i++)
            {
                if (muzzle[i].gameObject.activeInHierarchy)
                {
                    muzzleFlash[i].Play();
                    flashLight[i].SetActive(true);
                }
            }

            GameObject shell = pool.GetObject(3);
            shell.GetComponent<Shell>().Spawned(transform);
            shell.transform.SetPositionAndRotation(shellSpawn.position, shellSpawn.rotation);

            audioSource.PlayOneShot(activeGun.audioClip);

            yield return new WaitForSeconds(.2f);

            foreach (GameObject item in flashLight) item.SetActive(false);
        }
    }

    IEnumerator reloading()
    {
        Debug.Log("Reloading");
        isReloading = true;

        gunParent.transform.DORewind();
        gunParent.transform.DOPunchRotation(reloadRotation, activeGun.reloadTime - .1f, 0, 1);

        yield return new WaitForSeconds(activeGun.reloadTime);
        isReloading = false;
        //yield return new WaitForSeconds(.25f);

        currAmmo = activeGun.ammo;
    }

    public void setInput(float val)
    {
        input = val;
    }
}
