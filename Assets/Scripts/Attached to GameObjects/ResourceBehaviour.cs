using UnityEngine;
using System.Collections;

public class ResourceBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit");
        if (col.gameObject.name.Equals("Player"))
        {
            Debug.Log("Hit by player");
            Destroy(this.gameObject);
        }
    }
}
