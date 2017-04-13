using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class RTSInterface : MonoBehaviour
{
    [SerializeField]
    FlockBehaviour flockPrefab;

    List<EnemyBehaviour> enemies;

    List<EnemyBehaviour> newFlock;
    List<FlockBehaviour> flocks;

    Camera_RTS cam;

    bool patrol = false;
    bool multipleEnemiesSelected;

    int index = 0;

    string moveOrPatrolString = "MOVE";

    // EnemyBehaviour currentlyControlledEnemy;
    Vector3 target;
    // Use this for initialization
    void Start()
    {
        Text crystalText = this.GetComponentsInChildren<Text>()[0];
        int resNum = int.Parse(crystalText.text) + 10;
        string temp = resNum + "";
        crystalText.text = temp;

        enemies = new List<EnemyBehaviour>();
        enemies.AddRange(FindObjectsOfType<EnemyBehaviour>());
        flocks = new List<FlockBehaviour>();
        newFlock = new List<EnemyBehaviour>();
        cam = FindObjectOfType<Camera_RTS>();
    }

    // Update is called once per frame
    void Update()
    {
        getKeyboardInputs();
        getMouseInputs();

    }
    void getKeyboardInputs()
    {
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
            if (multipleEnemiesSelected)
            {
                createNewFlock();
                foreach (EnemyBehaviour en in flocks[flocks.Count - 1].enemies)
                {
                    en.clearAllFutur();
                    en.currentState = EnemyBehaviour.State.Idle;
                }

                /*else if (currentlyControlledEnemy != null)
                {
                    currentlyControlledEnemy.clearAllFutur();
                    currentlyControlledEnemy.currentState = EnemyBehaviour.State.Idle;
                }*/
            }
            /* else
             {
                 if (currentlyControlledEnemy != null)
                 {
                     currentlyControlledEnemy.clearAllFutur();
                     currentlyControlledEnemy.currentState = EnemyBehaviour.State.Idle;
                 }
             }*/
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            multipleEnemiesSelected = true;
            //currentlyControlledEnemy = null;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            multipleEnemiesSelected = false;
        }
    }

    void createNewFlock()
    {
        if (newFlock.Count > 0)
        {
            foreach (FlockBehaviour fl in flocks)
            {
                foreach (EnemyBehaviour en in newFlock)
                {
                    fl.enemies.Remove(en);
                }
            }
            for (int i = 0; i < flocks.Count; i++)
            {
                if (flocks[i].enemies.Count < 1)
                {
                    FlockBehaviour temp = flocks[i];
                    flocks.RemoveAt(i);
                    Destroy(temp);

                }
            }
            //FlockBehaviour flock = new FlockBehaviour();
            FlockBehaviour tempFlock = Instantiate(flockPrefab, Vector3.zero, Quaternion.identity) as FlockBehaviour;
            flocks.Add(tempFlock);
            foreach (EnemyBehaviour en in newFlock)
            {
                flocks[flocks.Count - 1].enemies.Add(en);
            }
            Debug.Log("Aliens in flock: " + flocks[flocks.Count - 1].enemies.Count);
        }/*else if (newFlock.Count == 1)
        {
            multipleEnemiesSelected = false;
            currentlyControlledEnemy = newFlock[0];
        }*/
        newFlock = new List<EnemyBehaviour>();
    }

    void getMouseInputs()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //currentlyControlledEnemy = null;

            createNewFlock();
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject.tag.Equals("Alien"))
                {
                    if (multipleEnemiesSelected)
                    {
                        newFlock.Add(hit.collider.GetComponentInParent<EnemyBehaviour>());
                    }
                    else
                    {
                        //wasClickedOn(hit.collider.GetComponentInParent<EnemyBehaviour>());
                        createNewFlock();
                        newFlock.Add(hit.collider.GetComponentInParent<EnemyBehaviour>());
                    }

                }
                else if (hit.collider.gameObject.tag == "Floor")
                {

                    target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    //if (multipleEnemiesSelected)
                    // {
                    // createNewFlock();
                    // if (multipleEnemiesSelected )
                    // {
                    giveCommandToFlock(flocks.Count - 1);
                    //}
                    // else
                    // {
                    //     giveCommand();
                    //  }
                    //  }
                    // else
                    // {
                    //      giveCommand();
                    // }
                }
            }
        }
    }
    /*
    public void wasClickedOn(EnemyBehaviour e)
    {
        Debug.Log("Has e");
        currentlyControlledEnemy = e;
    }
    */
    /* void giveCommand()
     {
         if(currentlyControlledEnemy != null)
         {
             currentlyControlledEnemy.clearAllFutur();
             Debug.Log(target);
             if (patrol)
             {
                 Debug.Log("Patrol");
                 currentlyControlledEnemy.currentState = EnemyBehaviour.State.Patrol;
                 currentlyControlledEnemy.assignTarget(currentlyControlledEnemy.transform.position);
                 currentlyControlledEnemy.assignPatrolTarget(target);
             }
             else
             {
                 Debug.Log("Move To");
                 currentlyControlledEnemy.currentState = EnemyBehaviour.State.MoveTo;
                 currentlyControlledEnemy.assignTarget(target);
             }
         }
     }*/

    void giveCommandToFlock(int index)
    {
        if (index < flocks.Count && index >= 0)
        {
            foreach (EnemyBehaviour en in flocks[index].enemies)
            {
                en.clearAllFutur();
                Debug.Log(target);
                if (patrol)
                {
                    Debug.Log("Patrol");
                    en.currentState = EnemyBehaviour.State.Patrol;
                    en.assignTarget(en.transform.position);
                    en.assignPatrolTarget(target);
                }
                else
                {
                    Debug.Log("Move To");
                    en.currentState = EnemyBehaviour.State.MoveTo;
                    en.assignTarget(target);
                }
            }
        }
    }

    public void MoveOrPatrolBtnClicked()
    {
        patrol = !patrol;
        Text mpText = this.GetComponentsInChildren<Text>()[2];
        string temp = mpText.text;
        mpText.text = moveOrPatrolString;
        moveOrPatrolString = temp;

    }

    public void FindNextClicked()
    {
        int prevIndex = index;

        while ((++index) % enemies.Count != prevIndex)
        {
            index = index % enemies.Count;
            if (enemies[index].currentState == EnemyBehaviour.State.Idle)
            {
                cam.changeCameraTarget(enemies[index].gameObject);
                break;
            }
        }
    }

    public void GotResource()
    {
        Text crystalText = this.GetComponentsInChildren<Text>()[1];
        int resNum = int.Parse(crystalText.text) + 1;
        string temp = resNum + "";
        crystalText.text = temp;
    }

    public bool trySpawningEggs()
    {
        Text crystalText = this.GetComponentsInChildren<Text>()[0];
        int resNum = int.Parse(crystalText.text);
        if (resNum >= 2)
        {
            resNum -= 2;
            string temp = resNum + "";
            crystalText.text = temp;
            return true;
        }
        return false;
    }
}