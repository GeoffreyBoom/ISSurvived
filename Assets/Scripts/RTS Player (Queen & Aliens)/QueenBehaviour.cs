using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


public class QueenBehaviour : Photon.MonoBehaviour
{
    [SerializeField]
    public int numOfEggs = 1;
    RTSInterface inter;
    /*
     *
     *if the queen is spawning eggs, the line circonference should be ON and eggs have to be within the circle if they want to bloom. 
     * If the line renderer is ON, then the Queen cannot move. 
     * Otherwise if off.  
     * 
     */
    void Start()
    {
        inter = FindObjectOfType<RTSInterface>();
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            //If line renderer was drawn for the Queen's egg spawning circonference:
            if (GetComponent<selectAlien>().isOn)
            {
                //if the user presses space then it instantiates 4 eggs within its radius: 
                if (Input.GetKeyDown(KeyCode.Space) && inter.trySpawningEggs())
                {
                    //spawn 4-5 eggs within the radius: 
                    for (int i = 0; i < numOfEggs; i++)
                    {
                        Vector3 position = transform.position + new Vector3(Random.Range(-GetComponent<selectAlien>().radius, GetComponent<selectAlien>().radius), 0, Random.Range(-GetComponent<selectAlien>().radius, GetComponent<selectAlien>().radius));
                        GameObject egg = PhotonNetwork.Instantiate("Egg", position, this.transform.rotation, 0);
                        egg.GetComponentInChildren<TextMesh>().text = "8";
                        egg.transform.GetChild(0).transform.LookAt(Camera.main.transform);
                    }
                }
            }

            if (GetComponent<selectAlien>().isMoving == true)
            {
                GetComponent<EnemyBehaviour>().moving();
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            // Network player, receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }
}
