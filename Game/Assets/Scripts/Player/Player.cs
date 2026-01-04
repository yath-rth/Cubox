using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

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
    [SerializeField] GameObject graphics, deathEffect, inventoryUI;
    [HideInInspector] public Gun gun;

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
    [HideInInspector] public bool WallBang = false, shouldRegen;
    float canRegen, shoot;
    int temp;
    Vector2 move;
    InputDir currInput = InputDir.NONE;
    InputDir prevInput = InputDir.FRONT;


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

        if (newInput != null)
        {
            if (inventoryUI != null) newInput.WSAD.Inventory.performed += _ => { inventoryUI.SetActive(!inventoryUI.activeInHierarchy); };

            newInput.WSAD.Shoot.performed += _ => Shoot();
        }
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

            /*if (crossHairVisual != null && fieldOfView.closestTarget != null) // Auto Aim functionality
            {
                crossHairVisual.position = fieldOfView.closestTarget.position + new Vector3(0, 1, 0);
            }
            else if (fieldOfView.closestTarget == null)
            {
                crossHairVisual.localPosition = defaultPos;
            } */

            crossHairVisual.localPosition = defaultPos;
            gun.gunParent.LookAt(crossHairVisual);

            /* if (GetShootInput() > 0 && cube != null) // This is to shoot in the direction of movement or something like that
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

    void input()
    {
        move = newInput.WSAD.Movement.ReadValue<Vector2>();
        look = newInput.WSAD.Look.ReadValue<Vector2>();

        if (ConnectionManager.instance != null)
        {
            if (move.x > 0 && move.y == 0) currInput = InputDir.RIGHT;
            else if (move.x < 0 && move.y == 0) currInput = InputDir.LEFT;
            else if (move.x == 0 && move.y > 0) currInput = InputDir.FRONT;
            else if (move.x == 0 && move.y < 0) currInput = InputDir.BACK;
            else if (move.x > 0 && move.y > 0) currInput = InputDir.FRIGHT;
            else if (move.x < 0 && move.y > 0) currInput = InputDir.FLEFT;
            else if (move.x > 0 && move.y < 0) currInput = InputDir.BRIGHT;
            else if (move.x < 0 && move.y < 0) currInput = InputDir.BLEFT;
            else currInput = InputDir.NONE;

            // NEED TO CHANGE THIS LOGIC CAN SIMPLY SEND THE VECTOR3 AS INPUT I AM PURELY DUMB WHAT THE FUCK IS THIS WHY WOULD YOU DO SUCH A LONG AND CUBERSOME METHOD MY GOD

            if (currInput != prevInput)
            {
                ConnectionManager.instance.SendInput(InputType.MOVE, currInput);
                prevInput = currInput;
            }
            //else ConnectionManager.instance.SendInput(InputType.NONE);
        }

        moveDirWSAD = new Vector3(move.x, 0, move.y).normalized;

        shoot = newInput.WSAD.Shoot.ReadValue<float>();
        if (gun != null) gun.setInput(shoot);
    }

    void Shoot()
    {
        ConnectionManager.instance.SendInput(InputType.SHOOT, InputDir.NONE);
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


        if (shouldRegen && Alive)
        {
            if (canRegen <= Time.time)
            {
                canRegen = Time.time + 5f;
                temp = 2 / 100 * playerStats.getStat(StatTypes.maxhitpoints);
                temp = Mathf.Clamp(temp, 0, playerStats.getStat(StatTypes.maxhitpoints));

                playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.hitpoints) + temp);
            }
        }
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
