using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridCustom : MonoBehaviour {

    public int xSize, zSize;
    public int multiplicator = 10;
    public float scale = 1;
    public float exponent = 1;
    [Space]
    public float heightPreferenceNoiseScale = 1;

    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[ (xSize + 1) * (zSize + 1) ];
        Vector2[] uv = new Vector2[ vertices.Length ];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                uv[ i ] = new Vector2((float)x / xSize, (float)z / zSize);
                vertices[ i ] = new Vector3(x, CalculateHeight(uv[i] * scale), z);
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

    private float CalculateHeight(Vector2 uv)
    {
        return CalculateVariantedNoise(uv) * multiplicator * HeightPreference(uv.x, uv.y);
        return
            (CalculateVariantedNoise(uv) 
            + CalculateVariantedNoise(new Vector2(uv.y, uv.x))
            + CalculateVariantedNoise(-uv)
            + CalculateVariantedNoise(- new Vector2(uv.y, uv.x)))
                / 4;
    }

    private float CalculateVariantedNoise(Vector2 uv)
    {
        return Mathf.Pow(
                (
                  CalulateNoiseVariant(uv, 1)
                + CalulateNoiseVariant(uv, 2)
                + CalulateNoiseVariant(uv, 4)
                + CalulateNoiseVariant(uv, 8)
                + CalulateNoiseVariant(uv, 16)
                + CalulateNoiseVariant(uv, 32)
                + CalulateNoiseVariant(uv, 64)
                + CalulateNoiseVariant(uv, 128))
                / 2f,
            exponent);
    }

    private float CalulateNoiseVariant(Vector2 uv, float freq)
    {
        return (1/freq) * Mathf.PerlinNoise(uv.x * freq, uv.y * freq);
    }

    private float HeightPreference(float x, float y)
    {
        return Mathf.PerlinNoise(x * heightPreferenceNoiseScale, y * heightPreferenceNoiseScale) * 3; //Noise value between 0 and 2
    }
}
