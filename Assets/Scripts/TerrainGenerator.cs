using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Structs;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    //map
    [SerializeField] private string seed = "default";
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float perlinScale = 1f;

    private MapTile[][] map;


    //tiles
    [SerializeField] private float tileRadius = 1;
    private float tileWidth;

    //biomes
    [SerializeField] private int gridSize = 20;
    [SerializeField] private AnimationCurve biomeImportanceCurve;

    private BiomeLocation[,] pointPositions;
    private List<Batcher> thresholds = new List<Batcher>();
    public List<Biome> biomes;
    private int horizontalNumberOfPoints;
    private int verticalNumberOfPoints;
    private int totalBiomeProba;

    private List<Batcher> batchers = new List<Batcher>();

    private void Awake()
    {
        UnityEngine.Random.InitState(seed.GetHashCode());
        tileWidth = tileRadius * Mathf.Cos(Mathf.Deg2Rad * 30);


        horizontalNumberOfPoints = width / gridSize;
        verticalNumberOfPoints = height / gridSize;

        GenerateBatchersList();

    }

    void Start()
    {
        GenerateVoronoiPoints();
        SetMap();
        List<List<Matrix4x4>> instanciatedTiles = new List<List<Matrix4x4>>();
        foreach (Threshold t in thresholds)
        {
            instanciatedTiles.Add(new List<Matrix4x4>());
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int k = 0;
                while (k < map[i][j].biomeType.thresholds.Count - 1 && map[i][j].position.y > map[i][j].biomeType.thresholds[k].height)
                {
                    k++;
                }
                map[i][j].biomeType.thresholds[k].batcher.batchedTransform.Add(Matrix4x4.TRS(map[i][j].position, Quaternion.Euler(new Vector3(-90, 0, 0)), new Vector3(100, 100, 100)));

            }
        }
        for (int i = 0; i < thresholds.Count; i++)
        {
            thresholds[i].CreateBatch();
        }
        foreach (Batcher b in batchers)
        {
            b.CreateBatch();
        }
    }


    private void SetMap()
    //set the map variable that will be used to store informations about tiles
    {
        map = new MapTile[height][];
        for (int i = 0; i < height; i++)
        {
            float offset = i % 2 == 1 ? tileWidth / 2 : 0;
            MapTile[] mapPart = new MapTile[width];

            for (int j = 0; j < width; j++)
            {
                //voronoi for biome
                float nearestDistance = Mathf.Infinity;
                BiomeLocation nearestPoint = new BiomeLocation();
                int gridX = i / gridSize;
                int gridY = j / gridSize;

                List<BiomeLocation> surroudingBiomes = new List<BiomeLocation>();

                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {

                        int X = gridX + a;
                        int Y = gridY + b;
                        if (!(X < 0 || Y < 0 || X >= horizontalNumberOfPoints || Y >= verticalNumberOfPoints))
                        {
                            surroudingBiomes.Add(pointPositions[X, Y]);
                            float distance = Vector2Int.Distance(new Vector2Int(i, j), pointPositions[X, Y].position);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                nearestPoint = pointPositions[X, Y];
                            }
                        }

                    }
                }

                float widthRatio = ((j * 1.0f) / width) * perlinScale;
                float heightRatio = ((i * 1.0f) / height) * perlinScale;

                float heightScale = 0;
                float heightOffset = 0;

                foreach (BiomeLocation biome in surroudingBiomes)
                {
                    float distance = (Vector2Int.Distance(new Vector2Int(i, j), biome.position));
                    float ratio;
                    ratio = biomeImportanceCurve.Evaluate(distance / 60);
                    heightScale += biome.biomeType.scale * ratio;
                    heightOffset += biome.biomeType.offset * ratio;
                }
                mapPart[j] = new MapTile(nearestPoint.biomeType,
                    new Vector3(j * tileWidth + offset, Mathf.PerlinNoise(widthRatio, heightRatio) * heightScale + heightOffset, i * tileRadius * 0.75f),
                    1f);

                //decor for the tile
                float decorValue = UnityEngine.Random.Range(0f, 1f);
                int decorIndex = 0;
                while (decorValue > 0f && decorIndex < nearestPoint.biomeType.decors.Count)
                {
                    DecorData dd = nearestPoint.biomeType.decors[decorIndex];
                    if (dd.heightRange.x < mapPart[j].position.y && dd.heightRange.y > mapPart[j].position.y)
                    {
                        decorValue -= dd.probability;
                        if (decorValue <= 0f)
                        {
                            float size = UnityEngine.Random.Range(80f, 120f);
                            dd.batcher.batchedTransform.Add(
                                Matrix4x4.TRS(mapPart[j].position, Quaternion.Euler(new Vector3(-90, 0, UnityEngine.Random.Range(0, 360))), new Vector3(size, size, size)));
                        }
                    }
                    decorIndex++;
                }
            }

            map[i] = mapPart;
        }
    }

    private void GenerateBatchersList()
    {
        foreach(Biome b in biomes)
        {
            foreach (ThresholdData t in b.thresholds)
            {
                if (!thresholds.Contains(t.batcher))
                {
                    thresholds.Add(t.batcher);
                }
            }
            foreach (DecorData d in b.decors)
            {
                if (!batchers.Contains(d.batcher))
                {
                    batchers.Add(d.batcher);
                }
            }
            totalBiomeProba += b.probabilityStrength;
        }
    }

    private void GenerateVoronoiPoints()
    {
        pointPositions = new BiomeLocation[horizontalNumberOfPoints, verticalNumberOfPoints];
        for (int i = 0; i < horizontalNumberOfPoints; i++)
        {
            for (int j = 0; j < verticalNumberOfPoints; j++)
            {
                int proba = UnityEngine.Random.Range(0, totalBiomeProba);
                int biomeIndex = 0;
                while (proba > 0)
                {
                    proba -= biomes[biomeIndex].probabilityStrength;
                    if(proba > 0) biomeIndex++;
                }
                Biome type = biomes[biomeIndex];
                pointPositions[i, j] = new BiomeLocation(type, new Vector2Int(i * gridSize + UnityEngine.Random.Range(0, gridSize - 1), j * gridSize + UnityEngine.Random.Range(0, gridSize - 1)));
            }
        }
    }
}
