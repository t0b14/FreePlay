using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    [SerializeField] Transform cam;

    float xRotation;
    float yRotation;


    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)){
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;

            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f,90f);
            //Debug.Log(xRotation);
            //Debug.Log("y " + yRotation.ToString());
            orientation.rotation = Quaternion.Euler(xRotation,0, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);


        }

    }
}
