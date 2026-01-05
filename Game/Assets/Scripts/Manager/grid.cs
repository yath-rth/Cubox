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

    Coord mapCentre;
    Queue<Coord> ShuffledArray;

    // ---- Added fields ----
    List<Coord> allTileCoords = new List<Coord>();
    List<Coord> OpenCoord = new List<Coord>();

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

        bool[,] obstacleMap = new bool[(int)mapsize.x, (int)mapsize.y];
        int obstacleCount = (int)(mapsize.x * mapsize.y * .6);
        int currentObstacleCount = 0;

        allTileCoords.Clear();
        OpenCoord.Clear();

        for (int x = 0; x < mapsize.x; x++)
        {
            for (int y = 0; y < mapsize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
                OpenCoord.Add(new Coord(x, y));
            }
        }

        ShuffledArray = new Queue<Coord>(ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord((int)mapsize.x / 2, (int)mapsize.y / 2);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord random = RandomCoord();
            obstacleMap[random.x, random.y] = true;
            currentObstacleCount++;

            if (random != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount, mapsize))
            {
                Vector3 obstaclePos =
                    new Vector3(-mapsize.x / 2f + .5f + random.x, 0,
                    -mapsize.y / 2f + .5f + random.y) * tileSize;

                float obstacleHeight = Mathf.Lerp(1, 3, (float)prng.NextDouble());

                GameObject NewObstacle =
                    Instantiate(obstacle, obstaclePos + Vector3.up * obstacleHeight / 2, Quaternion.identity);

                NewObstacle.transform.localScale =
                    new Vector3(scaleValue.x, obstacleHeight, scaleValue.z);

                NewObstacle.transform.parent = mapHolder;

                Renderer obstacleRenderer = NewObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = random.y / (float)mapsize.y;
                obstacleMaterial.color = Color.Lerp(foregroundColor, backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                OpenCoord.Remove(random);
                RandomPos();
            }
            else
            {
                obstacleMap[random.x, random.y] = false;
                currentObstacleCount--;
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

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount, Vector2 mapsize)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0)
                            && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapsize.x * mapsize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
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

    Coord RandomCoord()
    {
        Coord c = ShuffledArray.Dequeue();
        ShuffledArray.Enqueue(c);
        return c;
    }

    void RandomPos() { }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coord)) return false;
            Coord other = (Coord)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return (x * 397) ^ y;
        }
    }
}
