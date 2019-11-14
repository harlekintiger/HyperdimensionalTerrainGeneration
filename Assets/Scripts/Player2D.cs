using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Player2D : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 1;
    [SerializeField]
    private bool invertAxis = false;

    [Space]
    [SerializeField]
    private float rotationSpeed = 1;
    [SerializeField]
    private bool invertRotation = false;

    private int axisInvert = 1;
    private int rotationInvert = 1;

    private void OnValidate()
    {
        if (invertAxis)
            axisInvert = -1;
        else
            axisInvert = 1;

        if (invertRotation)
            rotationInvert = -1;
        else
            rotationInvert = 1;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) == false)
            return;

        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate(Input.GetAxis("Horizontal") * axisInvert * movementSpeed, 0, 0, Space.Self);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            transform.Rotate(transform.forward, Input.GetAxis("Vertical") * rotationInvert * rotationSpeed, Space.Self);
        }
    }
}
