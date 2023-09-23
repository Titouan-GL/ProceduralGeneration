using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private float width;
    [SerializeField] private float height;

    [SerializeField] private float tileWidth;

    [SerializeField] private GameObject grassTile;
    [SerializeField] private GameObject mountainTile;
    [SerializeField] private GameObject waterTile;

    private float tileRadius;

    private void Awake()
    {
        tileRadius = tileWidth / Mathf.Cos(Mathf.Deg2Rad * 30); // a side is : "adjacent/cos(30)" and this times two gives the radius, and tileWidth/2 = adjacent
    }

    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            float offset = i % 2 == 1 ? tileWidth / 2 : 0;

            for (int j = 0; j < width; j++)
            {
                Instantiate(grassTile, transform.position + new Vector3(j * tileWidth + offset, 0, i * tileRadius * 0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
    }

}
