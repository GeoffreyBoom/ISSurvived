using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RTSInterface : MonoBehaviour
{

    List<EnemyBehaviour> enemies;
    Camera_RTS cam;
    bool patrol = false;
    int index = 0;
    string moveOrPatrolString = "MOVE";
    EnemyBehaviour currentlyControlledEnemy;
    Vector3 target;

    // Use this for initialization
    void Start()
    {
        //Initialize the num of resources
        Text crystalText = this.GetComponentsInChildren<Text>()[0];
        int resNum = int.Parse(crystalText.text) + 10;
        string temp = resNum + "";
        crystalText.text = temp;
        //add a reference to the camera
        cam = FindObjectOfType<Camera_RTS>();
    }

    // Update is called once per frame
    void Update()
    {
        refreshEnemyList();
        getKeyboardInputs();
        getMouseInputs();

    }

    void refreshEnemyList()
    {
        //get the list of npcs
        enemies = new List<EnemyBehaviour>();
        enemies.AddRange(FindObjectsOfType<EnemyBehaviour>());
    }

    void getKeyboardInputs()
    {
        //check for keyboard inputs
        if (Input.GetKeyDown(KeyCode.P))
        {
            MoveOrPatrolBtnClicked();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            FindNextClicked();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //the npc goes to its idle state
            currentlyControlledEnemy.clearAllFutur();
            currentlyControlledEnemy.currentState = EnemyBehaviour.State.Idle;
        }
    }
    void getMouseInputs()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //no npc is selected
            if (currentlyControlledEnemy != null && currentlyControlledEnemy.currentState == EnemyBehaviour.State.ReceiveInput)
            {
				if (currentlyControlledEnemy.gameObject.GetComponent<selectAlien>().isOn)
                {
                    currentlyControlledEnemy.gameObject.GetComponent<selectAlien>().isOn = true;
                    currentlyControlledEnemy.gameObject.GetComponent<selectAlien>().drawPoints = true;
					currentlyControlledEnemy.currentState = EnemyBehaviour.State.Idle;
				}
            }
            currentlyControlledEnemy = null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //do a raycast to see what was hit
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag.Equals("Alien"))
                {
                    //try selecting npc

                    wasClickedOn(hit.collider.GetComponentInParent<EnemyBehaviour>());
                    
                }
                else if (hit.collider.gameObject.tag == "Floor")
                {
                    //try giving currently controlled npc a command
                    target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    giveCommand();
                }
            }
        }
    }

    public void wasClickedOn(EnemyBehaviour e)
    {
        //if npc was already selected, deselect, else select

        if (e.gameObject.GetComponent<selectAlien>().isOn == false)
        {
            //npc goes to the receive input state
            if (e.currentState != EnemyBehaviour.State.Idle)
            {
                e.storeState(e.currentState, e.target);
            }
            e.currentState = EnemyBehaviour.State.ReceiveInput;
            currentlyControlledEnemy = e;
        }
        else{
            //npc goes back to the idle state
            if(currentlyControlledEnemy.currentState == EnemyBehaviour.State.ReceiveInput)
            {
                currentlyControlledEnemy.currentState = EnemyBehaviour.State.Idle;
            }
            currentlyControlledEnemy = null;

        }
    }

    void giveCommand()
    {
        //if an npc is selected
        if (currentlyControlledEnemy != null)
        {
            currentlyControlledEnemy.clearAllFutur();
            //if the npc is told to patrol
            if (patrol)
            {
                currentlyControlledEnemy.currentState = EnemyBehaviour.State.Patrol;
                currentlyControlledEnemy.assignTarget(currentlyControlledEnemy.transform.position);
                currentlyControlledEnemy.assignPatrolTarget(target);
            }
            else
            {
                //else it moves to the designated position
                currentlyControlledEnemy.currentState = EnemyBehaviour.State.MoveTo;
                currentlyControlledEnemy.assignTarget(target);
            }
        }
    }

    public void MoveOrPatrolBtnClicked()
    {
        //changes whether next command will be to patrol or to move
        patrol = !patrol;
        Text mpText = this.GetComponentsInChildren<Text>()[1];
        string temp = mpText.text;
        mpText.text = moveOrPatrolString;
        moveOrPatrolString = temp;

    }

    public void FindNextClicked()
    {
        //searches the list of enemy for the next enemy that is in its idle state
        int prevIndex = index;
        while ((++index) % enemies.Count != prevIndex)
        {
            index = index % enemies.Count;
            if (enemies[index].currentState == EnemyBehaviour.State.Idle)
            {
                //moves the camera over the npc
                cam.changeCameraTarget(enemies[index].gameObject);
                break;
            }
        }
    }

    public void GotResource()
    {
        //updates the number of resources
        Text crystalText = this.GetComponentsInChildren<Text>()[0];
        int resNum = int.Parse(crystalText.text) + 1;
        string temp = resNum + "";
        crystalText.text = temp;
    }

    public bool trySpawningEggs()
    {
        //checks whether or not there are enough resources to spawn an egg
        Text crystalText = this.GetComponentsInChildren<Text>()[0];
        int resNum = int.Parse(crystalText.text);
        if (resNum >= 2)
        {
            //if there is, update the number of resources
            resNum -= 1;
            string temp = resNum + "";
            crystalText.text = temp;
            return true;
        }
        return false;
    }
}