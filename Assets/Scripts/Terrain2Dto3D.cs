using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Terrain2Dto3D : MonoBehaviour
{
    public int length = 10;
    public int initialWidth = 1;
    public float rowDistance = 1;
    public float heightScale = 1;
    public float noiseScale = 1;
    public bool isDoubleSided = true;
    public IndexFormat meshIndexFormat = IndexFormat.UInt16;

    Mesh mesh;
    MeshFilter meshFilter;

    int width = 0;

    private void Awake()
    {
        Initialize();

        Generate2DTerrainLine( initialWidth );
    }

    private void Initialize()
    {
        mesh = new Mesh();
        mesh.name = "Generated 2D Terrain";
        mesh.indexFormat = meshIndexFormat;
        mesh.subMeshCount = 2;
        mesh.vertices = new Vector3[ 0 ];
        mesh.MarkDynamic();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private void Generate2DTerrainLine(int amountToAdd = 1)
    {
        Vector3[] vertices = new Vector3[ length * amountToAdd ];
        int[] indices = new int[ (length - 1) * 2 * amountToAdd ];
        int indicesIndex = 0;

        for (int rowCountToAdd = 0; rowCountToAdd < amountToAdd; rowCountToAdd++)
        {
            int rowOffsetVert = rowCountToAdd * length;
            int rowOffsetInd = (length - 1) * 2 * rowCountToAdd;

            float zOffset = rowDistance * width++;
            vertices[ rowOffsetVert ].z += zOffset;

            for (int i = 1; i < length; i++)
            {
                vertices[ rowOffsetVert + i ] = vertices[ rowOffsetVert + i - 1 ] + new Vector3(1, 0, 0);

                indices[ indicesIndex++ ] = rowOffsetVert + i - 1;
                indices[ indicesIndex++ ] = rowOffsetVert + i;
            }
        }

        for (int i = 0; i < indices.Length; i++)
        {
            indices[ i ] += mesh.vertices.Length;
        }

        mesh.vertices = JoinArrays(mesh.vertices, vertices);
        //mesh.SetIndices( new int[0], MeshTopology.Triangles, 0);
        mesh.SetIndices( JoinArrays(mesh.GetIndices(1), indices), MeshTopology.Lines, 1);

        Synch();
    }

    public void Add2DStrip()
    {
        Generate2DTerrainLine();
    }

    public void AddFaces()
    {
        { // Quads instead of tris
            /*
            int[] indices = new int[ (length - 1) * 4];
            for (int quadCount = 0, indicesCount = 0; quadCount < length -1; quadCount++)
            {
                indices[ indicesCount++ ] = quadCount;
                indices[ indicesCount++ ] = quadCount + 1;

                indices[ indicesCount++ ] = quadCount + 1 + length;
                indices[ indicesCount++ ] = quadCount     + length;
            }

            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            */
        }
        int[] indices = new int[ (length - 1) * (3 * 2) * (width - 1) ];
        int indicesCount = 0;
        for (int rowCount = 0; rowCount < width - 1; rowCount++)
        {
            int rowOffset = rowCount * length;
            for (int trisCount = 0; trisCount < length - 1; trisCount++)
            {   // foreach quad / 2 tris:
                indices[ indicesCount++ ] = rowOffset + trisCount;
                indices[ indicesCount++ ] = rowOffset + trisCount + 1 + length;
                indices[ indicesCount++ ] = rowOffset + trisCount + 1;

                indices[ indicesCount++ ] = rowOffset + trisCount;
                indices[ indicesCount++ ] = rowOffset + trisCount + length;
                indices[ indicesCount++ ] = rowOffset + trisCount + 1 + length;
            }
            Debug.Log("now finsihed row " + rowCount);
        }

        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.subMeshCount = 1;
        mesh.RecalculateNormals(100);

        mesh.normals = new Vector3[ mesh.vertexCount ].Populate(new Vector3(0, 1, 0));
        mesh.RecalculateNormals();

        Synch();
    }

    private void PreventFrustumCulling()
    {
        /* 
            int e = 0;
            // following code from http://allenwp.com/blog/2013/12/19/disabling-frustum-culling-on-a-   game-object-in-unity/

            // boundsTarget is the center of the camera's frustum, in world coordinates:
            Vector3 camPosition = Camera.main.transform.position;
            Vector3 normCamForward = Vector3.Normalize(Camera.main.transform.forward);
            float boundsDistance = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2 +     Camera.main.nearClipPlane;
            Vector3 boundsTarget = camPosition + (normCamForward * boundsDistance);

            // The game object's transform will be applied to the mesh's bounds for frustum culling     checking.
            // We need to "undo" this transform by making the boundsTarget relative to the game     object's transform:
            Vector3 realtiveBoundsTarget = this.transform.InverseTransformPoint(boundsTarget);

            // Set the bounds of the mesh to be a 1x1x1 cube (actually doesn't matter what the size     is)
            mesh.bounds = new Bounds(realtiveBoundsTarget, Vector3.one);
        */

        Synch();
    }

    private void Synch()
    {
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public static T[] JoinArrays<T>(T[] array1, T[] array2)
    {
        T[] newArray = new T[ array1.Length + array2.Length ];
        Array.Copy(array1, newArray, array1.Length);
        Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
        return newArray;
    }

    [Space]
    public bool drawNormals = false;
    public bool reGetMesh = false;

    Vector3[] vertices;
    Vector3[] normals;

    private void OnDrawGizmos()
    {
        if (reGetMesh)
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;

            normals = mesh.normals;
            vertices = mesh.vertices;

            reGetMesh = false;
            return;
        }

        if (drawNormals == false)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Gizmos.DrawLine(vertices[ i ], vertices[ i ] + normals[ i ]);
            Gizmos.DrawSphere(vertices[ i ], 0.1f);
        }
    }
}
