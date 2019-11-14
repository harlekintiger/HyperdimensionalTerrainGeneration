using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color4DTerrain : MonoBehaviour
{
    [SerializeField]
    private int xSize, ySize, zSize;


    private void Start()
    {
        GetComponent<MeshFilter>().mesh = Generate();
    }

    private Mesh Generate()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Cubegrid";
        Vector3[] vertices;


        vertices = new Vector3[ (xSize + 1) * (ySize + 1) * (zSize + 1) ];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    vertices[ i++ ] = new Vector3(x, y, z);
                }
            }
        }
        mesh.vertices = vertices;


        //int[] quadIndices = new int[];
        //int index = 0;
        //for (int z = 0; z < zSize - 1; z++)
        //{
        //    for (int y = 0; y < ySize - 1; y++)
        //    {
        //        for (int x = 0; x < xSize - 1; x++)
        //        {
        //            quadIndices[ index++ ] = x;
        //            quadIndices[ index++ ] = x;
        //            quadIndices[ index++ ] = x;
        //            quadIndices[ index++ ] = x;
        //        }
        //    }
        //}


        return mesh;
    }
}
