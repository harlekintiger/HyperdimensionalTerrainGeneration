using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class TestScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Test", EditorStyles.miniButton))
        {
            ((TestScript)target).Test();

            Undo.RecordObject(target, "test");
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }
}