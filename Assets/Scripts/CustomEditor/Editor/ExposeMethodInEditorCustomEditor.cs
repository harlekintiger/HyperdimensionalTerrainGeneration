using UnityEngine;
using UnityEditor;
using System.Reflection;

[CanEditMultipleObjects] 
[CustomEditor(typeof(MonoBehaviour), true)] 
public class ExposeMethodInEditorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            var type = target.GetType();

            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = method.GetCustomAttributes(typeof(ExposeMethodInEditorAttribute), true);
                if (attributes.Length > 0)
                {
                    if (GUILayout.Button("Run: " + method.Name))
                        ((MonoBehaviour)target).Invoke(method.Name, 0f);
                }
            }
        }
    }
}
