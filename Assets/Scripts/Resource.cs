using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Photon.MonoBehaviour {
	
	void Start ()
    {
		



	}
	
	void Update () {

		transform.RotateAround(transform.position, transform.up, 1f);
	}
	
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Queen" || collision.gameObject.tag == "Alien")
		{
			photonView.RPC("consume", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void consume ()
	{
		Destroy(gameObject);
	}
}
