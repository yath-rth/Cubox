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
    DamageableItem item;
    Camera cam;

    public PlayerInput newInput;

    //[SerializeField] Stats playerStatOrg;
    public Stats playerStats;

    [SerializeField] Renderer visual;
    [SerializeField] GameObject graphics, deathEffect, inventoryUI;
    [HideInInspector] public Gun gun;

    [Header("Player Properties")]
    Color playerColor;
    public Color flashColour;

    [Header("Crosshair")]
    public float crosshairOffset = 2;

    [SerializeField] Transform crossHairVisual;

    [Header("Player Stats")]
    [HideInInspector] public bool dashing = false;
    [HideInInspector] public bool WallBang = false;
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

        item = GetComponent<DamageableItem>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        gun = GetComponent<Gun>();
        newInput = new PlayerInput();
        cube = crossHairVisual.transform.parent.transform;
        cam = Camera.main;
        defaultPos = new Vector3(crossHairVisual.localPosition.x, crossHairVisual.localPosition.y, crosshairOffset);

        playerStats.setStat(StatTypes.hitpoints, playerStats.getStat(StatTypes.maxhitpoints));

        graphics.SetActive(true);

        if (newInput != null)
        {
            if (inventoryUI != null) newInput.WSAD.Inventory.performed += _ => { inventoryUI.SetActive(!inventoryUI.activeInHierarchy); };

            newInput.WSAD.Shoot.performed += _ => startShoot();
            newInput.WSAD.Shoot.canceled += _ => endShoot();
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
        if (item.GetAlive())
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
    }

    void startShoot()
    {
        ConnectionManager.instance.SendShootInput(InputType.SHOOT, 1);
    }

    void endShoot()
    {
        ConnectionManager.instance.SendShootInput(InputType.SHOOT, 0);
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
        if(cam == null) return;

        float point;

        Vector3 mouse = new Vector3(look.x, look.y, 0);

        Ray ray = cam.ScreenPointToRay(mouse);

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

    public float GetDamage()
    {
        return playerStats.getStat(StatTypes.damage);
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
