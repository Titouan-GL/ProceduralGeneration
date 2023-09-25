using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Instancer : MonoBehaviour
{

    public Mesh instancedMesh;

    public Material[] materials;

    private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    private void RenderBatches()
    {
        foreach (var batch in batches)
        {
            for (int i = 0; i < instancedMesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(instancedMesh, i, materials[i], batch);
            }
        }
    }

    private void Update()
    {
        RenderBatches();
    }

    public void CreateBatch(List<TerrainGenerator.FakeTransform> tiles)
    {
        int addedMatrices = 0;
        batches.Add(new List<Matrix4x4>());

        for (int i = 0;i < tiles.Count ;i++)
        {
            if(addedMatrices < 1000)
            {
                batches[batches.Count - 1].Add(Matrix4x4.TRS(tiles[i].position, tiles[i].rotation, tiles[i].scale));
                addedMatrices++;
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                batches[batches.Count - 1].Add(Matrix4x4.TRS(tiles[i].position, tiles[i].rotation, tiles[i].scale));
                addedMatrices = 0;
            }
        }
    }
}
