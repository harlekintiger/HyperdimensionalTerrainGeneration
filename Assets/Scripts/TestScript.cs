using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public void Test()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Test mesh";
        mesh.subMeshCount = 1;
        mesh.MarkDynamic();

        int[] indices = { 0, 1, 2, 2, 3, 0 };
        Vector3[] vertices =
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
        };

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        for (int i = 0; i < mesh.normals.Length; i++)
        {
            Debug.Log(mesh.normals[ i ]);
        }

        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

        mesh.RecalculateNormals();

        for (int i = 0; i < mesh.normals.Length; i++)
        {
            Debug.Log(mesh.normals[ i ]);
        }

        mesh.normals = new[]{
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0)
        };

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
