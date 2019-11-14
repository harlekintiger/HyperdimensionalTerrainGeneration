using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AssignVectorToShader : MonoBehaviour
{
    [SerializeField]
    private Material terrainMaterial;

    private void Update()
    {
        terrainMaterial.SetVector("_CurrentUp", new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 100);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 100);
    }

}
