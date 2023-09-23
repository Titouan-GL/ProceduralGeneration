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


    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private float tileWidth;

    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject mountainTile;
    [SerializeField] private GameObject waterTile;

    private float tileRadius;

    private Tile[][] map;

    private void Awake()
    {
        tileRadius = tileWidth / Mathf.Cos(Mathf.Deg2Rad * 30); // a side is : "adjacent/cos(30)" and this times two gives the radius, and tileWidth/2 = adjacent
        map = new Tile[height][];
    }

    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            float offset = i % 2 == 1 ? tileWidth / 2 : 0;
            Tile[] mapPart = new Tile[width];

            for (int j = 0; j < width; j++)
            {
                float widthRatio = (j * 1.0f) / width;
                float heightRatio = (i*1.0f) / height;
                mapPart[j] = new Tile("grass", new Vector3(j * tileWidth + offset, Mathf.PerlinNoise(widthRatio, heightRatio) * 5, i * tileRadius * 0.75f));
            }

            map[i] = mapPart;
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Instantiate(grassTile, map[i][j].position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
    }



}
