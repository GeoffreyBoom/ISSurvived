using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_RTS : MonoBehaviour
{

    float zoomSpeed;
    float moveSpeed;
    float mouseMargin;
    float rotationSpeed;
    bool rotating;

    void Start()
    {

        zoomSpeed = 1.0f;
        moveSpeed = 1.0f;
        mouseMargin = 1.0f;
        rotationSpeed = 2.0f;
        rotating = false;

        transform.position = new Vector3(0.0f, 5.0f);
        Vector3 rotation = new Vector3(0.5f, -0.5f);
        transform.rotation = Quaternion.LookRotation(rotation);
    }

    void Update()
    {


        //Zoom
        transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        //Mouse & keyboard movement
        if ((Input.mousePosition.x <= mouseMargin && !rotating) || Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        else if ((Input.mousePosition.x >= Screen.width - mouseMargin && !rotating) || Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }

        if ((Input.mousePosition.y <= mouseMargin && !rotating) || Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(transform.forward.x, 0.0f, transform.forward.z) * moveSpeed * Time.deltaTime;
        }
        else if ((Input.mousePosition.y >= Screen.height - mouseMargin && !rotating) || Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(transform.forward.x, 0.0f, transform.forward.z) * moveSpeed * Time.deltaTime;
        }

        //Mouse rotation
        if (Input.GetKey(KeyCode.Mouse2))
        {
            rotating = true;
            transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed);
        }
        else
        {
            rotating = false;
        }
    }

    public void changeCameraTarget(GameObject target)
    {

        transform.position = target.transform.position + Vector3.up * 10;
		Vector3 rotation = new Vector3(0.5f, -0.5f);
		transform.rotation = Quaternion.LookRotation(rotation);
		transform.forward = (target.transform.position - transform.position).normalized;
    }
}
