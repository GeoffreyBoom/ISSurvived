using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eggs : MonoBehaviour
{

    GameObject queen;
    float timer = 8;
    static public int counter = 0;

    // Use this for initialization
    void Start()
    {
        queen = GameObject.FindGameObjectWithTag("Queen");
    }

    // Update is called once per frame
    void Update()
    {

        timer -= Time.deltaTime;
        this.transform.GetChild(0).GetComponent<TextMesh>().text = Mathf.RoundToInt(timer).ToString();     

        //When the timer is done, spawn aliens:
        if (timer < 0)
        {
            GameObject alien = PhotonNetwork.Instantiate("Aliens", this.transform.position, this.transform.rotation, 0);
            alien.name = alien.name + counter;
            Destroy(this.gameObject);
            counter++;
        }

    }
}
