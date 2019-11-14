using UnityEngine;

public class ComputeTerrain : MonoBehaviour
{
    public bool doNormalRecalculation = true;
    public bool doBoundRecalculation = true;

    public ComputeShader computeShader;

    private MeshFilter meshFilter;

    ComputeBuffer vertBuffer;

    private void Start()
    {
        Allocate();
        GenerateTerrain();
    }

    private void Allocate()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
    }

    public void GenerateTerrain()
    {
        //Debug.Log(meshFilter.sharedMesh.vertices[ 0 ]);

        int kernelHandle = computeShader.FindKernel("TerVertGen");

        vertBuffer = new ComputeBuffer(meshFilter.mesh.vertexCount, 12);
        vertBuffer.SetData(meshFilter.sharedMesh.vertices);

        int threadGroupX = meshFilter.mesh.vertexCount / 128;
        threadGroupX++;

        computeShader.SetBuffer(kernelHandle, "mesh", vertBuffer);

        computeShader.SetMatrix("localToWorldMatrix", transform.localToWorldMatrix);

        computeShader.Dispatch(kernelHandle, threadGroupX, 1, 1);

        Vector3[] vecArr = new Vector3[ meshFilter.sharedMesh.vertexCount ];
        vertBuffer.GetData(vecArr);

        Mesh mesh = meshFilter.mesh;
        mesh.vertices = vecArr;
        if (doNormalRecalculation)
            mesh.RecalculateNormals(100);
        if (doBoundRecalculation)
            mesh.RecalculateBounds();

        meshFilter.mesh = mesh;


        vertBuffer.Release();
    }

    public void RecalculateNormals()
    {
        GetComponentInChildren<MeshFilter>().mesh.RecalculateNormals(100);
    }
}