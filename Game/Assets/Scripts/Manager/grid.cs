using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class grid : MonoBehaviour
{
    public static grid Grid;

    [SerializeField] GameColorManager colorManager;

    Vector2 mapsize;
    Vector2 Maxmapsize = new Vector2(12, 12);
    public GameObject tile, coll, camPos, healthBar, ammoBar, staminaBar;
    [Range(0, 50)]
    public int maxSizeX, maxSizeY, minSizeX, minSizeY;
    [Range(0, 2)]
    public float outlinePercent, tileHeight = 1f, camOffset;
    [Range(-5f, 5f)]
    public float tileSize, healthBarOffset, ammoBarOffset, staminaBarOffset, otherBarOffsetY;
    [Range(0, 20f)] public float otherBarOffsetX;

    Color foregroundColor;
    Color backgroundColor;

    [SerializeField] Cinemachine.CinemachineVirtualCamera cam;

    [HideInInspector]
    public List<GameObject> positions;
    List<Tile> tileScripts;

    List<Transform> mapBlockers;

    void Start()
    {
        if (Grid != null && Grid != this)
        {
            Destroy(this);
        }
        else
        {
            Grid = this;
        }

        positions = new List<GameObject>((int)(mapsize.x * mapsize.y));
        tileScripts = new List<Tile>((int)(mapsize.x * mapsize.y));

        GenerateGrid();
    }

    private void OnDestroy()
    {
        if (Grid == this)
        {
            Grid = null;
        }
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapsize.x / 2f + 0.5f + x, 0, -mapsize.y / 2f + 0.5f + y) * tileSize;
    }

    public void GenerateGrid()
    {
        Color[] colors = colorManager.getColor();
        backgroundColor = colors[0];
        foregroundColor = colors[1];

        mapsize = new Vector2(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY));
        Maxmapsize = new Vector2(maxSizeX, maxSizeY);

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
            positions = new List<GameObject>((int)(mapsize.x * mapsize.y));
            tileScripts = new List<Tile>((int)(mapsize.x * mapsize.y));
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapsize.x; x++)
        {
            for (int y = 0; y < mapsize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                GameObject newTile = Instantiate(tile, tilePosition, tile.transform.rotation);

                positions.Add(newTile);

                newTile.transform.parent = mapHolder;
                newTile.transform.localScale = new Vector3(1 * (1 - outlinePercent) * -tileSize, (1 - outlinePercent) * -tileSize * tileHeight, 1 * (1 - outlinePercent) * -tileSize);

                tileScripts.Add(newTile.GetComponent<Tile>());

                Renderer obstacleRenderer = newTile.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = x / (float)mapsize.x;
                obstacleMaterial.SetColor("_color", Color.Lerp(foregroundColor, backgroundColor, colourPercent));
                obstacleRenderer.sharedMaterial = obstacleMaterial;

            }
        }

        Transform obstacleHolder = new GameObject("obstacle holder").transform;
        obstacleHolder.parent = mapHolder;

        CinemachineTargetGroup targetGroup = obstacleHolder.gameObject.AddComponent<CinemachineTargetGroup>();
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[10];
        targetGroup.m_UpdateMethod = CinemachineTargetGroup.UpdateMethod.FixedUpdate;

        CinemachineTargetGroup.Target target;
        target.weight = 1;
        target.radius = 0;

        mapBlockers = new List<Transform>();

        Transform maskLeft = Instantiate(coll, Vector3.left * (mapsize.x + Maxmapsize.x) / 4f * tileSize, Quaternion.identity).transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((Maxmapsize.x - mapsize.x) / 2f, 5, mapsize.y + 1f) * tileSize;

        Transform maskRight = Instantiate(coll, Vector3.right * (mapsize.x + Maxmapsize.x) / 4f * tileSize, Quaternion.identity).transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((Maxmapsize.x - mapsize.x) / 2f, 5, mapsize.y + 1f) * tileSize;

        Transform maskTop = Instantiate(coll, Vector3.forward * (mapsize.y + Maxmapsize.y) / 4f * tileSize, Quaternion.identity).transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(Maxmapsize.x + 1f, 5, (Maxmapsize.y - mapsize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(coll, Vector3.back * (mapsize.y + Maxmapsize.y) / 4f * tileSize, Quaternion.identity).transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(Maxmapsize.x + 1f, 5, (Maxmapsize.y - mapsize.y) / 2f) * tileSize;


        mapBlockers.Add(maskLeft);
        mapBlockers.Add(maskRight);
        mapBlockers.Add(maskTop);
        mapBlockers.Add(maskBottom);


        for (int i = 0; i < 3; i++)
        {
            target.target = Instantiate(camPos, mapBlockers[i].position * camOffset, Quaternion.identity).transform;
            target.target.parent = mapHolder;
            targetGroup.m_Targets[i] = target;
        }

        if (healthBar != null)
        {
            healthBar.transform.position = new Vector3(0, 0, -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) + healthBarOffset));
            target.target = healthBar.transform;
            targetGroup.m_Targets[4] = target;
        }

        if (ammoBar != null)
        {
            ammoBar.transform.position = new Vector3(otherBarOffsetX, 0, -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) + healthBarOffset + otherBarOffsetY + ammoBarOffset));
            target.target = ammoBar.transform;
            targetGroup.m_Targets[5] = target;
        }

        if (staminaBar != null)
        {
            staminaBar.transform.position = new Vector3(otherBarOffsetX, 0, -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) + healthBarOffset + otherBarOffsetY + ammoBarOffset + staminaBarOffset));
            target.target = staminaBar.transform;
            targetGroup.m_Targets[6] = target;
        }

        if (cam != null) cam.m_LookAt = obstacleHolder;
    }

    public GameObject getRandomPos()
    {
        return positions[Random.Range(0, positions.Count)];
    }

    public GameObject getRandomPosNearPlayer()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            if (tileScripts[i].isContact)
            {
                return positions[i];
            }
        }

        return null;
    }
}
