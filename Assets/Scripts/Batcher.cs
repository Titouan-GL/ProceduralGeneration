using System.Collections.Generic;
using UnityEngine;

public class Batcher : MonoBehaviour
{
    new public string name;

    public Mesh instancedMesh;

    public Material[] materials;

    public List<Matrix4x4> batchedTransform = new List<Matrix4x4>();

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

    public void CreateBatch()
    {
        int addedMatrices = 0;
        batches.Add(new List<Matrix4x4>());

        for (int i = 0; i < batchedTransform.Count; i++)
        {
            if (addedMatrices < 1000)
            {
                batches[batches.Count - 1].Add(batchedTransform[i]);
                addedMatrices++;
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                batches[batches.Count - 1].Add(batchedTransform[i]);
                addedMatrices = 0;
            }
        }
    }
}
