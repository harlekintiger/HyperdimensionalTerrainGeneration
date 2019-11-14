using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ComputeTerrain))]
public class ComputeTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Regenerate Terrain", EditorStyles.miniButton))
        {
            ((ComputeTerrain)target).GenerateTerrain();

            Undo.RecordObject(target, "Regenerated terrain");
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("RecalculateNormals", EditorStyles.miniButton))
        {
            ((ComputeTerrain)target).RecalculateNormals();

            Undo.RecordObject(target, "RecalculateNormals");
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }
}