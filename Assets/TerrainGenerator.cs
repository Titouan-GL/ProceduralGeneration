using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public struct Tile
    {
        public Vector3 position;
        public string type { get; set; }
        public Tile(string newtype, Vector3 newposition)
        {
            type = newtype;
            position = newposition;
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

    [SerializeField] private float tileWidth;

    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject mountainTile;
    [SerializeField] private GameObject waterTile;

    private float tileRadius;

    private Tile[][] map;

    List<List<GameObject>> tiles = new List<List<GameObject>>();

    public List<Threshold> thresholds;

    private void Awake()
    {
        tileRadius = tileWidth / Mathf.Cos(Mathf.Deg2Rad * 30); // a side is : "adjacent/cos(30)" and this times two gives the radius, and tileWidth/2 = adjacent
        map = new Tile[height][];
    }

    void Start()
    {
        SetMap();

        for (int i = 0; i < height; i++)
        {
            tiles.Add(new List<GameObject>());
            for (int j = 0; j < width; j++)
            {
                GameObject go = Instantiate(grassTile, map[i][j].position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                tiles[i].Add(go);
            }
        }
    }

    void Update()
    {
        SetMap();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tiles[i][j].transform.position = map[i][j].position;
                int k = 0;
                while (tiles[i][j].transform.position.y > thresholds[k].height && k < thresholds.Count)
                {
                    k++;
                }
                tiles[i][j].GetComponent<Renderer>().material = thresholds[k].color;
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
                float widthRatio = ((j * 1.0f) / width) * perlinScale;
                float heightRatio = ((i * 1.0f) / height) * perlinScale;
                float heightScale = 11;
                float heightOffset = -3;
                mapPart[j] = new Tile("grass", new Vector3(j * tileWidth + offset, Mathf.PerlinNoise(widthRatio, heightRatio) * heightScale + heightOffset, i * tileRadius * 0.75f));
            }

            map[i] = mapPart;
        }
    }

}
