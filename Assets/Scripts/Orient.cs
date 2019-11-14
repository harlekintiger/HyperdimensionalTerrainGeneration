using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Orient : MonoBehaviour
{
    public static Plane playerExistancePlane = new Plane();

    [SerializeField]
    private bool continuousOrientationCalulcation = false;

    [Space]
    [SerializeField]
    private Vector3 anchor2;
    [SerializeField]
    private Vector3 anchor3;

    [Space]
    [SerializeField]
    private Transform objectToSetAnchor2To;
    [SerializeField]
    private Transform objectToSetAnchor3To;
    [SerializeField]
    private Transform objectToSetPlayerTo;

    private void OnValidate()
    {
        if (objectToSetAnchor2To != null)
        {
            anchor2 = objectToSetAnchor2To.position;
            objectToSetAnchor2To = null;
        }

        if (objectToSetAnchor3To != null)
        {
            anchor3 = objectToSetAnchor3To.position;
            objectToSetAnchor3To = null;
        }

        if (objectToSetPlayerTo != null)
        {
            transform.position = objectToSetPlayerTo.position;
            objectToSetPlayerTo = null;
        }
    }

    private void Update()
    {
        playerExistancePlane.normal = transform.up;

        if (continuousOrientationCalulcation)
            OrientToAnchors();
    }

    public void OrientToAnchors()
    {
        transform.up = Vector3.Cross(
            anchor2 - transform.position, anchor3 - transform.position);
    }

    public void CalculateAnchors()
    {
        anchor2 = transform.position + transform.right;

        anchor3 = transform.position + transform.forward;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);
        //Gizmos.DrawLine(transform.position, transform.position + transform.right * 100);

        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.DrawWireSphere(anchor2,            0.25f);
        Gizmos.DrawWireSphere(anchor3,            0.25f);

    }
}
