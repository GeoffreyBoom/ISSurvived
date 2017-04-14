using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class selectAlien : Photon.MonoBehaviour
{


    [SerializeField]
    public float radius = 1f;
    [SerializeField]
    public int numOfPoints = 100;

    private float increment = 0.2f;
    private bool drawPoints = false;
    private float speed = 4.0f;
    LineRenderer line;

    public bool isMoving = false;
    public bool isOn = false;


    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.isMine)
        {
            if (drawPoints == false)
            {
                selectQueenOrAlien();
            }
            else
            {
                if (isOn == false)
                {
                    if (increment <= radius)
                    {

                        CreatePoints(increment);
                        increment += increment * Time.deltaTime * speed;
                    }
                    else
                    {
                        isOn = true;
                        drawPoints = false;

                    }
                }
                else
                {
                    if (increment >= 0.2f)
                    {
                        CreatePoints(increment);
                        increment -= increment * Time.deltaTime * speed;
                    }
                    else
                    {
                        isOn = false;
                        isMoving = false;
                        drawPoints = false;
                    }
                }

            }
        }


    }


    void selectQueenOrAlien()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (line.enabled == false)
            {
                line.enabled = true;
            }

            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    if (hit.rigidbody.gameObject.name == this.name)
                    {
                        Debug.Log(hit.rigidbody.gameObject.name);
                        drawPoints = true;
                    }
                }
                else
                {
                    if (isOn && hit.collider.gameObject.tag == "Floor" && this.gameObject.tag == "Queen")
                    {
                        Vector3 target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        GetComponent<EnemyBehaviour>().assignTarget(target);
                        isMoving = true;
                    }
                }
            }
        }
    }

    public void CreatePoints(float increment)
    {
        line.positionCount = (numOfPoints + 1);
        line.useWorldSpace = false;
        float x;
        float z;

        float angle = 0f;

        for (int i = 0; i < (numOfPoints + 1); i++)
        {

            x = Mathf.Sin(Mathf.Deg2Rad * angle) * increment;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * increment;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / numOfPoints);
        }
    }
}
