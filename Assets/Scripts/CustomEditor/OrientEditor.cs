using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Orient))]
//[CanEditMultipleObjects] // only if you handle it properly

public class OrientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Orient To Anchors", EditorStyles.miniButton))
        {
            ((Orient)target).OrientToAnchors();

            Undo.RecordObject(target, "OrientedToAnchors");
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Calculate Anchors", EditorStyles.miniButton))
        {
            ((Orient)target).CalculateAnchors();

            Undo.RecordObject(target, "calculate anchors");
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }
}