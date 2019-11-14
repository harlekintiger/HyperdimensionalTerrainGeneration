using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Player3D : MonoBehaviour
{
    [SerializeField]
    private bool invertHorizontalAxis = false;
    [SerializeField]
    private bool invertVerticalAxis = false;

    private int horizontalInvert = 1;
    private int verticalInvert = 1;

    private void OnValidate()
    {
        if (invertHorizontalAxis)
            horizontalInvert = -1;
        else
            horizontalInvert = 1;


        if (invertVerticalAxis)
            verticalInvert = -1;
        else
            verticalInvert = 1;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) == false)
            return;

        Debug.Log("SPACE !11");

        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate(Input.GetAxis("Horizontal") * horizontalInvert, 0, 0, Space.Self);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(0, 0, Input.GetAxis("Vertical") * verticalInvert, Space.Self);
        }
    }
}
