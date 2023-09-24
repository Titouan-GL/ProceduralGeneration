using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public struct Tile
    {
        public Vector3 position;
        public Biome biomeType;
        public float closenessRatio;//how far is it from center ? 0 is on it, 0.5 makes it as close to the center than to another center
        public Tile(Vector3 newposition, float newCloseness = 0, Biome newType = new Biome())
        {
            biomeType = newType;
            position = newposition;
            closenessRatio = newCloseness;
        }
    }

    public struct BiomeLocation
    {
        public Vector2Int position;
        public Biome biomeType;
        public int x;
        public int y;
        public BiomeLocation(Vector2Int newposition = new Vector2Int(), Biome newbiome = new Biome())
        {
            position = newposition;
            biomeType = newbiome;
            x = position.x;
            y = position.y;
        }
    }

    [Serializable]
    public struct Biome
    {
        public string type;
        public float offset;
        public float scale;
        public Biome(string newtype = "", float newoffset = 0, float newscale = 0)
        {
            type = newtype;
            offset = newoffset; 
            scale = newscale;
        }
    }


    [Serializable]
    public struct Threshold
    {
        public float height;
        public string type;
        public Material color;
        public Threshold(string newtype, float newheight, Material newcolor)
        {
            type = newtype;
            height = newheight;
            color = newcolor;
        }
    }


    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float perlinScale = 1f;

    [SerializeField] private float tileWidth = 1;
    [SerializeField] private int gridSize = 20;

    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject mountainTile;
    [SerializeField] private GameObject waterTile;

    private float tileRadius;

    private Tile[][] map;
    private BiomeLocation[,] pointPositions;

    List<List<GameObject>> tiles = new List<List<GameObject>>();

    public List<Threshold> thresholds;
    public List<Biome> biomes;

    private int horizontalNumberOfPoints;
    private int verticalNumberOfPoints;

    private void Awake()
    {
        tileRadius = tileWidth / Mathf.Cos(Mathf.Deg2Rad * 30); // a side is : "adjacent/cos(30)" and this times two gives the radius, and tileWidth/2 = adjacent
        map = new Tile[height][];


        horizontalNumberOfPoints = width / gridSize;
        verticalNumberOfPoints = height / gridSize;

}

    void Start()
    {
        GeneratePoints();
        SetMap();

        for (int i = 0; i < height; i++)
        {
            tiles.Add(new List<GameObject>());
            for (int j = 0; j < width; j++)
            {
                GameObject go = Instantiate(grassTile, map[i][j].position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                go.name = "tile " + i + "_" + j;
                tiles[i].Add(go);
                int k = 0;
                while (k < thresholds.Count-1 && go.transform.position.y > thresholds[k].height)
                {
                    k++;
                }
                go.GetComponent<Renderer>().material = thresholds[k].color;

            }
        }
    }


    private void SetMap()
    {
        for (int i = 0; i < height; i++)
        {
            float offset = i % 2 == 1 ? tileWidth / 2 : 0;
            Tile[] mapPart = new Tile[width];

            for (int j = 0; j < width; j++)
            {

                float nearestDistance = Mathf.Infinity;
                //float secondNearestDistance = Mathf.Infinity;
                BiomeLocation nearestPoint = new BiomeLocation();
                int gridX = i / gridSize;
                int gridY = j / gridSize;
                float totalDistance = 0;

                List<BiomeLocation> surroudingBiomes = new List<BiomeLocation>();

                for (int a = -1; a < 2; a++)
                {
                    for(int b = -1; b < 2; b++)
                    {

                        int X = gridX + a;
                        int Y = gridY + b;
                        if (!(X < 0 || Y < 0 || X >= horizontalNumberOfPoints || Y >= verticalNumberOfPoints))
                        {
                            surroudingBiomes.Add(pointPositions[X, Y]);
                            float distance = Vector2Int.Distance(new Vector2Int(i, j), pointPositions[X, Y].position);
                            totalDistance += distance;
                            if(distance < nearestDistance)
                            {
                                //secondNearestDistance = nearestDistance;
                                nearestDistance = distance;
                                nearestPoint = pointPositions[X, Y];
                            }
                            /*else if(distance < secondNearestDistance)
                            {
                                secondNearestDistance = distance;
                            }*/
                        }
                        
                    }
                }

                float widthRatio = ((j * 1.0f) / width) * perlinScale;
                float heightRatio = ((i * 1.0f) / height) * perlinScale;
                //float closenessRatio = 1- (nearestDistance / (nearestDistance + secondNearestDistance)) * 2;//how far is it from center ? from 0 to 1
                //float heightScale = nearestPoint.biomeType.scale; * closenessRatio;
                //float heightOffset = nearestPoint.biomeType.offset; * closenessRatio;
                //Debug.Log(i + " " + j + " " + closenessRatio+ " " +nearestDistance + " " + secondNearestDistance);

                float heightScale = 0;
                float heightOffset = 0;
                float totalRatioedDistance = 0;

                foreach(BiomeLocation biome in surroudingBiomes)
                {
                    totalRatioedDistance += totalDistance / (Vector2Int.Distance(new Vector2Int(i, j), biome.position));
                }

                foreach (BiomeLocation biome in surroudingBiomes)
                {
                    if ((Vector2Int.Distance(new Vector2Int(i, j), biome.position)) != 0)
                    {
                        float ratio = (totalDistance / (Vector2Int.Distance(new Vector2Int(i, j), biome.position))) / totalRatioedDistance;
                        heightScale += biome.biomeType.scale * ratio;
                        heightOffset += biome.biomeType.offset * ratio;
                    }
                    else
                    {
                        heightScale = biome.biomeType.scale;
                        heightOffset = biome.biomeType.offset;
                    }
                }
                
                mapPart[j] = new Tile(
                    new Vector3(j * tileWidth + offset, Mathf.PerlinNoise(widthRatio, heightRatio) * heightScale + heightOffset, i * tileRadius * 0.75f),
                    1f, nearestPoint.biomeType);
            }

            map[i] = mapPart;
        }
    }

    private void GeneratePoints()
    {
        pointPositions = new BiomeLocation[horizontalNumberOfPoints, verticalNumberOfPoints];
        for (int i = 0; i<horizontalNumberOfPoints; i++)
        {
            for(int j  = 0; j < verticalNumberOfPoints; j++)
            {
                Biome type = biomes[UnityEngine.Random.Range(0, biomes.Count)];
                pointPositions[i, j] = new BiomeLocation(new Vector2Int(i*gridSize + UnityEngine.Random.Range(0, gridSize-1), j *gridSize + UnityEngine.Random.Range(0, gridSize - 1)), type);
            }
        }
    }

}
