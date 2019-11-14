using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Terrain2Dto3D))]
public class Terrain2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Add 2D Terrain row", EditorStyles.miniButton))
        {
            ((Terrain2Dto3D)target).Add2DStrip();

            Undo.RecordObject(target, "Add 2D Terrain row");
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("AddFaces", EditorStyles.miniButton))
        {
            ((Terrain2Dto3D)target).AddFaces();

            Undo.RecordObject(target, "AddFaces");
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }
}