using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
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

    [Range(0, 20f)]
    public float otherBarOffsetX;

    Color foregroundColor;
    Color backgroundColor;

    [SerializeField] CinemachineCamera cam;

    [HideInInspector]
    public List<GameObject> positions;

    List<Tile> tileScripts;
    List<Transform> mapBlockers;
    public int seed = 0;
    System.Random prng;

    public GameObject obstacle;
    public Vector3 scaleValue = Vector3.one;
    // ----------------------

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

    public void SetUpWorldGrid(Dictionary<string, int> mapSize)
    {
        mapsize = new Vector2(mapSize["X"], mapSize["Y"]);
        maxSizeX = mapSize["MaxX"];
        maxSizeY = mapSize["MaxY"];

        Maxmapsize = new Vector2(maxSizeX, maxSizeY);

        positions = new List<GameObject>((int)(mapsize.x * mapsize.y));
        tileScripts = new List<Tile>((int)(mapsize.x * mapsize.y));

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        prng = new System.Random(seed);

        Color[] colors = colorManager.getColor();
        backgroundColor = colors[0];
        foregroundColor = colors[1];

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
                newTile.transform.localScale = new Vector3(
                    (1 - outlinePercent) * -tileSize,
                    (1 - outlinePercent) * -tileSize * tileHeight,
                    (1 - outlinePercent) * -tileSize);

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
        targetGroup.Targets = new List<CinemachineTargetGroup.Target>();
        targetGroup.UpdateMethod = CinemachineTargetGroup.UpdateMethods.FixedUpdate;

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
            Transform obj = Instantiate(camPos, mapBlockers[i].position * camOffset, Quaternion.identity).transform;
            CinemachineTargetGroup.Target target = newTarget(obj);
            target.Object.parent = mapHolder;
            targetGroup.Targets.Add(target);
        }

        if (healthBar != null)
        {
            healthBar.transform.position =
                new Vector3(0, 0, -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) + healthBarOffset));
            targetGroup.Targets.Add(newTarget(healthBar.transform));
        }

        if (ammoBar != null)
        {
            ammoBar.transform.position =
                new Vector3(otherBarOffsetX, 0,
                -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) +
                healthBarOffset + otherBarOffsetY + ammoBarOffset));

            targetGroup.Targets.Add(newTarget(ammoBar.transform));
        }

        if (staminaBar != null)
        {
            staminaBar.transform.position =
                new Vector3(otherBarOffsetX, 0,
                -1 * (((mapsize.y + maxSizeY) / 4f * tileSize) +
                healthBarOffset + otherBarOffsetY + ammoBarOffset + staminaBarOffset));

            targetGroup.Targets.Add(newTarget(staminaBar.transform));
        }

        if (cam != null) cam.LookAt = obstacleHolder;
    }

    public GameObject getTileAtPosition(Vector3 position)
    {
        float distance = 99999;
        GameObject tile = null;

        foreach(GameObject _tile in positions)
        {
            if(Vector3.Distance(_tile.transform.position, position) < distance)
            {
                distance = Vector3.Distance(_tile.transform.position, position);
                tile = _tile;
            }
        }

        return tile;
    }

    public GameObject getRandomPos()
    {
        return positions[UnityEngine.Random.Range(0, positions.Count)];
    }

    CinemachineTargetGroup.Target newTarget(Transform obj)
    {
        CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
        target.Weight = 1;
        target.Radius = 0;
        target.Object = obj;
        return target;
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

    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
