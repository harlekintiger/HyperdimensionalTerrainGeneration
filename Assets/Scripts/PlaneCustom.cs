using UnityEngine;

[ExecuteInEditMode]
public class PlaneCustom : MonoBehaviour
{
    [SerializeField]
    private int xSize, zSize;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        Generate();
        Allocate();
    }

    private void Allocate()
    {
        for (int part = 0; part < transform.childCount; part++)
        {
            MeshFilter meshFilter = transform.GetChild(part).GetComponent<MeshFilter>();
            if (meshFilter != null)
                meshFilter.mesh = mesh;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            if (hit.distance == 2)
                return;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit2))
        {
            if (hit2.distance == 2)
                return;
        }
    }

    private void Generate()
    {
        mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[ (xSize + 1) * (zSize + 1) ];
        Vector2[] uv = new Vector2[ vertices.Length ];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                uv[ i ] = new Vector2((float)x / xSize, (float)z / zSize);
                vertices[ i ] = new Vector3(x, 0, z);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] triangles = new int[ xSize * zSize * 6 ];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ ti ] = vi;
                triangles[ ti + 3 ] = triangles[ ti + 2 ] = vi + 1;
                triangles[ ti + 4 ] = triangles[ ti + 1 ] = vi + xSize + 1;
                triangles[ ti + 5 ] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}