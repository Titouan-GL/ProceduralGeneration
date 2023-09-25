using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
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

    //map
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float perlinScale = 1f;

    private Tile[][] map;

    //tiles
    [SerializeField] private float tileRadius = 2;
    private float tileWidth;
    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject mountainTile;
    [SerializeField] private GameObject waterTile;

    List<List<GameObject>> tiles = new List<List<GameObject>>();

    //biomes
    [SerializeField] private int gridSize = 20;
    [SerializeField] private AnimationCurve biomeImportanceCurve;

    private BiomeLocation[,] pointPositions;
    public List<Threshold> thresholds;
    public List<Biome> biomes;
    private int horizontalNumberOfPoints;
    private int verticalNumberOfPoints;

    //mesh
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private Vector3[] meshPointsOffset;

    private void Awake()
    {
        tileWidth = tileRadius * Mathf.Cos(Mathf.Deg2Rad * 30); 
        map = new Tile[height][];


        horizontalNumberOfPoints = width / gridSize;
        verticalNumberOfPoints = height / gridSize;

        mesh = new Mesh();
        //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        meshPointsOffset = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, tileRadius/2),
            new Vector3(tileWidth/2, 0, tileRadius/4),
            new Vector3(tileWidth/2, 0, -tileRadius/4),
            new Vector3(0, 0, -tileRadius/2),
            new Vector3(-tileWidth/2, 0, -tileRadius/4),
            new Vector3(-tileWidth/2, 0, tileRadius/4)
    };

    }

    void Start()
    {
        GeneratePoints();
        SetMap();
        createMesh();
        updateMesh();


        /*for (int i = 0; i < height; i++)
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
        }*/
    }

    void createMesh()
    {
        vertices = new Vector3[width * height * 7];
        triangles = new int[width * height * 36];

        for (int i = 0; i < height; i++)
        {
            tiles.Add(new List<GameObject>());
            for (int j = 0; j < width; j++)
            {
                int indexV = 7 * (i * width + j);
                int indexT = 36 * (i * width + j);

                Vector3 pos = map[i][j].position;
                for (int k = 0; k < 7; k++)
                {
                    vertices[indexV + k] = pos + meshPointsOffset[k];
                }
                triangles[indexT] = indexV;
                triangles[indexT + 1] = indexV + 6;
                triangles[indexT + 2] = indexV + 1;
                for (int k = 0; k < 5; k++)
                {
                    triangles[indexT + k * 3 + 3] = indexV;
                    triangles[indexT + k * 3 + 4] = indexV + k + 1;
                    triangles[indexT + k * 3 + 5] = indexV + k + 2;
                }

                //doing the sides....
                if(j > 0)
                {
                    triangles[indexT + 18] = indexV + 5;
                    triangles[indexT + 19] = indexV - 4;
                    triangles[indexT + 20] = indexV + 6;

                    triangles[indexT + 21] = indexV - 4;
                    triangles[indexT + 22] = indexV - 5;
                    triangles[indexT + 23] = indexV + 6;
                }
                if(i < height-1 && i%2 == 0)
                {
                    triangles[indexT + 24] = indexV + 2;
                    triangles[indexT + 25] = indexV + 1;
                    triangles[indexT + 26] = indexV + 4 + 7 * width;

                    triangles[indexT + 27] = indexV + 1;
                    triangles[indexT + 28] = indexV + 5 + 7 * width;
                    triangles[indexT + 29] = indexV + 4 + 7 * width;
                    if(j > 0)
                    {
                        triangles[indexT + 30] = indexV + 1;
                        triangles[indexT + 31] = indexV + 6;
                        triangles[indexT + 32] = indexV + 3 + 7 * (width - 1);

                        triangles[indexT + 33] = indexV + 6;
                        triangles[indexT + 34] = indexV + 4 + 7 * (width - 1);
                        triangles[indexT + 35] = indexV + 3 + 7 * (width - 1);
                    }
                }
                else if(i < height-1 && i % 2 == 1)
                {
                    triangles[indexT + 24] = indexV + 1;
                    triangles[indexT + 25] = indexV + 6;
                    triangles[indexT + 26] = indexV + 3 + 7 * width;

                    triangles[indexT + 27] = indexV + 6;
                    triangles[indexT + 28] = indexV + 4 + 7 * width;
                    triangles[indexT + 29] = indexV + 3 + 7 * width;
                    if (j < width-1)
                    {
                        triangles[indexT + 30] = indexV + 2;
                        triangles[indexT + 31] = indexV + 1;
                        triangles[indexT + 32] = indexV + 4 + 7 * (width + 1);

                        triangles[indexT + 33] = indexV + 1;
                        triangles[indexT + 34] = indexV + 5 + 7 * (width + 1);
                        triangles[indexT + 35] = indexV + 4 + 7 * (width + 1);
                    }
                }
            }
        }
    }

    void updateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
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
                BiomeLocation nearestPoint = new BiomeLocation();
                int gridX = i / gridSize;
                int gridY = j / gridSize;

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
                            if(distance < nearestDistance)
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
                    ratio = biomeImportanceCurve.Evaluate(distance/ 60);
                    if(ratio > 0.1)
                    {
                        heightScale += biome.biomeType.scale * ratio;
                        heightOffset += biome.biomeType.offset * ratio;
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
