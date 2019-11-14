using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DisplayObject : MonoBehaviour
{
    [SerializeField]
    private float minDistanceToDisplay = 3;

    [Space]
    private GameObject objectToDisplay;

    private bool wasDisplayedLastFrame = false;

    private void Update()
    {
        float distanceToPlane = Vector3.Distance(transform.position,
            Orient.playerExistancePlane.ClosestPointOnPlane(transform.position));
        bool inDisplayDistance = distanceToPlane < minDistanceToDisplay;

        if (wasDisplayedLastFrame != inDisplayDistance)
            SwitchDisplayObject();
    }

    private void SwitchDisplayObject()
    {
        //if (wasDisplayedLastFrame)
        //{
        //    Instantiate(objectToDisplay, transform, );
        //}
        //
        //    wasDisplayedLastFrame = !wasDisplayedLastFrame;
    }
}

