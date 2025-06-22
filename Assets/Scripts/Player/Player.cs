using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player playerInstance;

    Rigidbody rb;
    NavMeshAgent agent;
    Vector3 moveDirWSAD;
    Vector3 defaultPos;
    Vector2 look;
    Quaternion lookDir;
    Transform cube;

    public PlayerInput newInput;

    //[SerializeField] Stats playerStatOrg;
    public Stats playerStats;

    [SerializeField] Renderer visual;
    [SerializeField] GameObject graphics;
    [SerializeField] GameObject deathEffect;

    [HideInInspector] public Gun gun;

    public LayerMask enemies;

    [Header("Player Properties")]
    Color playerColor;
    public float damageSpeed;
    public Color flashColour;

    [Header("Crosshair")]
    public float crosshairOffset = 2;

    [SerializeField] Transform crossHairVisual;

    [Header("Player Stats")]
    [HideInInspector] public bool ShouldTakeDamage = true, dashing = false;
    public bool Alive = true;
    [HideInInspector] public bool WallBang = false;

    upgradeStats regenUpgrade;

    float canRegen;
    int temp;
    float shoot;
    Vector2 move;

    private void Awake()
    {
        //playerStats = Instantiate(playerStatOrg);

        if (playerInstance != null && playerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            playerInstance = this;
        }

        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        gun = GetComponent<Gun>();
        newInput = new PlayerInput();
        cube = crossHairVisual.transform.parent.transform;
        defaultPos = new Vector3(crossHairVisual.localPosition.x, crossHairVisual.localPosition.y, crosshairOffset);

        playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.maxhitpoints));

        graphics.SetActive(true);
    }

    private void OnDestroy()
    {
        if (playerInstance == this)
        {
            playerInstance = null;
        }
    }

    private void OnEnable()
    {
        newInput.Enable();
    }

    private void OnDisable()
    {
        newInput.Disable();
    }

    private void Update()
    {
        if (Alive)
        {
            input();
            rotate();
            crossHairVisual.Rotate(Vector3.up * .15f);

            /*if (crossHairVisual != null && fieldOfView.closestTarget != null)
            {
                crossHairVisual.position = fieldOfView.closestTarget.position + new Vector3(0, 1, 0);
            }
            else if (fieldOfView.closestTarget == null)
            {
                crossHairVisual.localPosition = defaultPos;
            } */

            crossHairVisual.localPosition = defaultPos;
            gun.gunParent.LookAt(crossHairVisual);

            /* if (GetShootInput() > 0 && cube != null)
            {
                float x = ((look.x + transform.position.x) > 0) ? (look.x + transform.position.x) + crosshairOffset : (look.x + transform.position.x) - crosshairOffset;
                float z = ((look.y + transform.position.z) > 0) ? (look.y + transform.position.z) + crosshairOffset : (look.y + transform.position.z) - crosshairOffset;

                cube.position = new Vector3(look.x + transform.position.x, 1, look.y + transform.position.z);
                transform.LookAt(cube);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            } */
        }

        CheckHealthPlayer();
    }

    private void FixedUpdate()
    {
        Debug.Log("PLayer Damage: " + playerStats.getStat(StatTypes.damage));
        if (Alive) moveFunc();
    }

    void input()
    {
        move = newInput.WSAD.Movement.ReadValue<Vector2>();
        look = newInput.WSAD.Look.ReadValue<Vector2>();

        moveDirWSAD = new Vector3(move.x, 0, move.y).normalized;

        shoot = newInput.WSAD.Shoot.ReadValue<float>();
        //if (gun != null) gun.setInput(Vector2.SqrMagnitude(shoot));
        if (gun != null) gun.setInput(look.magnitude);
    }

    void moveFunc()
    {
        Vector3 moveDirection = moveDirWSAD * playerStats.getStat(StatTypes.speed) * Time.fixedDeltaTime;

        //To move in direction player is looking
        //moveDirection = transform.TransformDirection(moveDirection);

        //transform.Translate(moveDirection);

        if (rb != null && agent == null)
        {
            rb.MovePosition(rb.position + moveDirection);
        }
        else if (rb == null && agent != null && agent.enabled)
        {
            agent.speed = playerStats.getStat(StatTypes.speed);
            agent.Move(moveDirection);
            agent.SetDestination(transform.position + moveDirection);
        }
    }

    void rotate()
    {
        float point;

        Vector3 mouse = new Vector3(look.x, look.y, 0);

        Ray ray = Camera.main.ScreenPointToRay(mouse);

        Plane groundPlane = new Plane(Vector3.up, Vector3.up);

        if (groundPlane.Raycast(ray, out point))
        {
            Vector3 Point = ray.GetPoint(point);
            Vector3 CorrectedLookAT = new Vector3(Point.x, transform.position.y, Point.z);
            transform.LookAt(CorrectedLookAT);
            cube.position = CorrectedLookAT;

            if (Vector3.Distance(CorrectedLookAT, transform.position) < 0.5f)
            {
                transform.rotation = lookDir;
            }
            else
            {
                //To avoid bug of not moving when pointer is exactly on player
                //transform.rotation = lookDir + Quaternion.eulerAngles(1, 1 ,1);
            }
        }
    }

    public IEnumerator takeDamagePlayer(int damage)
    {
        if (ShouldTakeDamage == true)
        {
            float damageTimer = 0;
            Material newTileMat = visual.sharedMaterial;

            while (damageTimer < .25f)
            {
                newTileMat.SetColor("_color", Color.Lerp(playerColor, flashColour, Mathf.PingPong(damageTimer * damageSpeed, 1)));

                damageTimer += Time.deltaTime;

                yield return null;
            }

            newTileMat.SetColor("_color", playerColor);


            playerStats.setStat(StatTypes.hitpoints, (int)Mathf.Clamp(
                        playerStats.getStat(StatTypes.hitpoints) - damage, 0, playerStats.getStat(StatTypes.hitpoints)));

            CheckHealthPlayer();
        }
    }

    void CheckHealthPlayer()
    {

        if (playerStats.getStat(StatTypes.hitpoints) > playerStats.getStat(StatTypes.maxhitpoints))
        {
            playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.maxhitpoints));
            Alive = true;
        }
        else if (playerStats.getStat(StatTypes.hitpoints) > 0)
        {
            Alive = true;
        }
        else if (playerStats.getStat(StatTypes.hitpoints) <= 0)
        {
            Debug.Log("U have died");
            Alive = false;
            graphics.SetActive(false);
            deathEffect.SetActive(true);
        }

        regenUpgrade = shouldRegen();

        if (regenUpgrade != null && Alive)
        {
            if (canRegen <= Time.time)
            {
                canRegen = Time.time + 5f;
                temp = (int)(regenUpgrade.upgrades[0].value / 100 *
                            (playerStats.getStat(StatTypes.maxhitpoints) - playerStats.getStat(StatTypes.hitpoints)));
                temp = Mathf.Clamp(temp, 0, playerStats.getStat(StatTypes.maxhitpoints));

                playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.hitpoints) + temp);
            }
        }
    }

    public upgradeStats shouldRegen()
    {
        foreach (var item in playerStats.appliedUpgrades)
        {
            //if(item.type == UpgradeTypes.regenration) return item;
            //else return null;
        }
        return null;
    }

    public float GetDamage()
    {
        return playerStats.getStat(StatTypes.damage);
    }

    public bool ShouldShoot()
    {
        if (newInput.WSAD.Shoot.ReadValue<float>() > 0) return true;
        else return false;
    }

    public GameObject GetGameobject()
    {
        return gameObject;
    }

    IEnumerator LerpPos(Transform obj, Vector3 posA, Vector3 posB, float duration, float speed)
    {
        float time = 0;

        while (time < duration)
        {
            obj.transform.localPosition = Vector3.Lerp(posA, posB, time * speed);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = posB;
    }

    public void setPlayerColor(Color color)
    {
        playerColor = color;
    }

    public Vector3 getMoveInput()
    {
        return moveDirWSAD;
    }

    public float getDashInput()
    {
        return newInput.WSAD.Dash.ReadValue<float>();
    }
}
