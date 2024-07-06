using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainBehaviour : MonoBehaviour
{
    /// <summary>
    /// The animatior for the train
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Is true when the train is moving
    /// </summary>
    [SerializeField]
    private bool move = false;

    /// <summary>
    /// Is true when the train has rotated in the station
    /// </summary>
    private bool hasRotated = true;

    /// <summary>
    /// A list of all NPCs sat in the fancy carriage
    /// </summary>
    [SerializeField]
    private List<GameObject> fCarrage;

    /// <summary>
    /// A list of all NPCs sat in the not fancy carriage
    /// </summary>
    [SerializeField]
    private List<GameObject> nfCarrage;

    /// <summary>
    /// The trains sprite renderer
    /// </summary>
    [SerializeField]
    private SpriteRenderer trainRenderer;

    /// <summary>
    /// The 2nd class carriage attached to the train
    /// </summary>
    [SerializeField]
    private SpriteRenderer carrageOne;

    /// <summary>
    /// The 3rd class carriage attached to the train
    /// </summary>
    [SerializeField]
    private SpriteRenderer carrageTwo;

    /// <summary>
    /// The camera on board the train
    /// </summary>
    [SerializeField]
    private GameObject onBoardCamera;

    /// <summary>
    /// The data manager for taking NPCs across scenes
    /// </summary>
    private GameObject dataManager;

    /// <summary>
    /// The types of NPC in the game
    /// </summary>
    [SerializeField]
    private List<GameObject> npcTypes;

    /// <summary>
    /// The prompt text for interacting with the train
    /// </summary>
    [SerializeField]
    private TMP_Text promptText;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GameObject.FindGameObjectWithTag("Data");

        if (move)
        {
            animator.SetTrigger("Arriving");
        }

        //Gets the NPCs in the 2nd class carriage
        List<string> fData = dataManager.GetComponent<DataManagement>().ApplyFData();

        //Gets the NPCs in the 3rd class carriage
        List<string> nfData = dataManager.GetComponent<DataManagement>().ApplyNFData();

        //If there is passengers in the 2nd class carriage when the scene changes the NPCs are spawned back into their seats
        if (fData.Count > 0)
        {
            foreach (string npc in fData)
            {
                foreach (GameObject type in npcTypes)
                {
                    foreach (GameObject seat in fCarrage)
                    {
                        if (type.GetComponent<Animator>().runtimeAnimatorController.name == npc && seat.GetComponentInChildren<SpriteRenderer>() == null)
                        {
                            GameObject seatTaken = Instantiate(type);
                            seatTaken.GetComponent<NPC>().targetCarrage = "";
                            seatTaken.transform.SetParent(seat.transform);
                            seatTaken.transform.position = seat.transform.position + new Vector3(0,0,1);

                            //Sets the interactable and prompt elements of the NPC to not be visible
                            seatTaken.transform.GetChild(0).gameObject.SetActive(false);
                            seatTaken.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                        }
                    }
                }
            }
        }
        //Clears the passenger storage of spawned NPCs
        fData.Clear();

        //If there is passengers in the 3rd class carriage when the scene changes the NPCs are spawned back into their seats
        if (nfData.Count > 0)
        {
            foreach (string npc in nfData)
            {
                foreach (GameObject type in npcTypes)
                {
                    foreach (GameObject seat in nfCarrage)
                    {
                        if (type.GetComponent<Animator>().runtimeAnimatorController.name == npc && seat.GetComponentInChildren<SpriteRenderer>() == null)
                        {
                            GameObject seatTaken = Instantiate(type);
                            seatTaken.GetComponent<NPC>().targetCarrage = "";
                            seatTaken.transform.SetParent(seat.transform);
                            seatTaken.transform.position = seat.transform.position;

                            //Sets the interactable and prompt elements of the NPC to not be visible
                            seatTaken.transform.GetChild(0).gameObject.SetActive(false);
                            seatTaken.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                        }
                    }
                }
            }
        }
        //Clears the passenger storage of spawned NPCs
        nfData.Clear();
    }

    private void LateUpdate()
    {
        if (move)
        {
            transform.Translate(new Vector2(-1, 0) * Time.deltaTime * 5);
        }

        //If the train is not visible on the screen and is facing the wrong way for departure the train will rotate
        if (!trainRenderer.isVisible && !carrageOne.isVisible && !carrageTwo.isVisible && !hasRotated)
        {
            //Rotates the screen to the correct orientation for Wadebridge
            if(SceneManager.GetActiveScene().name == "Waybridge")
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                hasRotated = true;
                transform.position = new Vector3(0, 0, 0);

                //Rotates all the carriages
                foreach (Transform pos in GetComponentsInChildren<Transform>())
                {
                    if (pos.GetComponent<SpriteRenderer>() != null)
                    {
                        pos.position = new Vector3(pos.position.x, pos.position.y, 1);
                    }
                }
                //Rotates the onboard camera and onboard sprite
                onBoardCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);
                onBoardCamera.transform.localPosition = new Vector3(0, 1, 10);
                onBoardCamera.transform.parent.position = new Vector3(onBoardCamera.transform.parent.position.x, onBoardCamera.transform.parent.position.y, 2);

                //Allows the train to be interacted with again
                GetComponent<BoxCollider2D>().enabled = true;
                promptText.transform.position = new Vector3(promptText.transform.position.x, promptText.transform.position.y, -1);
                promptText.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else //Rotates the train for Bodmin
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                hasRotated = true;
                transform.position = new Vector3(0, 0, 0);

                //Rotates all the carriages
                foreach (Transform pos in GetComponentsInChildren<Transform>())
                {
                    if(pos.GetComponent<SpriteRenderer>() != null)
                    {
                        pos.position = new Vector3(pos.position.x, pos.position.y, 1);
                    }
                }
                //Rotates the onboard camera and onboard sprite
                onBoardCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                onBoardCamera.transform.localPosition = new Vector3(0, 1, -10);
                onBoardCamera.transform.parent.position = new Vector3(onBoardCamera.transform.parent.position.x, onBoardCamera.transform.parent.position.y, 2);
                //Allows the train to be interacted with again
                GetComponent<BoxCollider2D>().enabled = true;
                promptText.transform.position = new Vector3(promptText.transform.position.x, promptText.transform.position.y, 1);
                promptText.rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    /// <summary>
    /// Assignes an NPC to a seat
    /// </summary>
    /// <param name="npc"> The NPC gameobject that needs to be assigned a seat</param>
    public void AssignSeat(GameObject npc)
    {
        switch (npc.GetComponent<NPC>().targetCarrage)
        {
            //Takes a seat in the 2nd class carriage
            case "Fancy":
                TakeFSeat(npc);
                break;
            //Takes a seat in the 3rd class carriage
            case "NotFancy":
                TakeNFSeat(npc);
                break;
            case "N/A":
                break;
            case "":
                break;
            default:
                Debug.LogError("Invalid carrage type");
                break;
        }
    }

    /// <summary>
    /// Assigns and moves the NPC to their seat
    /// </summary>
    /// <param name="npc">The NPC gameobject that needs to be assigned a seat</param>
    private void TakeFSeat(GameObject npc)
    {
        foreach (GameObject seat in fCarrage)
        {
            //Checks if the seat is empty and if the NPC is in front of the seat
            if (seat.GetComponentInChildren<SpriteRenderer>() == null && Mathf.RoundToInt(npc.transform.position.x) == Mathf.RoundToInt(seat.transform.position.x))
            {
                npc.transform.position = seat.transform.position + new Vector3(0, 0, 1);
                npc.GetComponent<NPC>().SeatTaken();
                npc.transform.SetParent(seat.transform);
                dataManager.GetComponent<DataManagement>().SetFData(npc.GetComponent<Animator>().runtimeAnimatorController.name);
                break;
            }
        }
    }

    /// <summary>
    /// Assigns and moves the NPC to their seat
    /// </summary>
    /// <param name="npc">The NPC gameobject that needs to be assigned a seat</param>
    private void TakeNFSeat(GameObject npc)
    {
        foreach (GameObject seat in nfCarrage)
        {
            //Checks if the seat is empty and if the NPC is in front of the seat
            if (seat.GetComponentInChildren<SpriteRenderer>() == null && Mathf.RoundToInt(npc.transform.position.x) == Mathf.RoundToInt(seat.transform.position.x))
            {
                npc.transform.position = seat.transform.position + new Vector3(0, 0, 1);
                npc.GetComponent<NPC>().SeatTaken();
                npc.transform.SetParent(seat.transform);
                dataManager.GetComponent<DataManagement>().SetNFData(npc.GetComponent<Animator>().runtimeAnimatorController.name);
                break;
            }
        }
    }

    /// <summary>
    /// Runs when the train arrives at the station
    /// </summary>
    /// <param name="collider">The station collider</param>
    private void ArriveAtStation(Collider2D collider)
    {
        //Sets the train to stationarry
        move = false;
        animator.SetTrigger("Arrived");

        //Enables the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponentInChildren<AudioListener>().enabled = true;
        player.GetComponentInChildren<CapsuleCollider2D>().enabled = true;
        player.transform.position = new Vector3(onBoardCamera.transform.parent.position.x, 0, 0);

        //Disables the onboard camera and audio listener
        onBoardCamera.GetComponent<Camera>().enabled = false;
        onBoardCamera.GetComponent<AudioListener>().enabled = false;
        hasRotated = false;

        //Disables the station collider
        collider.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        //Disembarks all the NPCs in the fancy carriage
        foreach(GameObject seat in fCarrage)
        {
            SpriteRenderer npc = seat.GetComponentInChildren<SpriteRenderer>();
            if (npc != null)
            {
                npc.gameObject.GetComponent<NPC>().Disembarked();
            }
        }

        //Disembarks all the NPCs in the fancy carriage
        foreach (GameObject seat in nfCarrage)
        {
            SpriteRenderer npc = seat.GetComponentInChildren<SpriteRenderer>();
            if (npc != null)
            {
                npc.gameObject.GetComponent<NPC>().Disembarked();
            }
        }
        //Plays the train whistle sound effect
        GetComponent<AudioSource>().Play();
    }

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if(SceneManager.GetActiveScene().name == "Bodminton")
        {
            //Allows the player to enter the train in Bodmin
            if (collisionObject.tag == "Player" && collisionObject.transform.position.x < 4)
            {
                promptText.enabled = true;
                PlayerInput input = collisionObject.GetComponent<PlayerInput>();
                InputAction interaction = input.actions.FindAction("Interact");

                //Puts the player in the train and starts the train moving
                if (interaction.IsPressed())
                {
                    animator.SetTrigger("Arriving");
                    onBoardCamera.GetComponent<Camera>().enabled = true;
                    onBoardCamera.GetComponent<AudioListener>().enabled = true;
                    GameObject.FindGameObjectWithTag("Leaving").GetComponent<CapsuleCollider2D>().enabled = true;
                    Destroy(collisionObject);
                    StartCoroutine(MoveDelay());
                }
            }
            else
            {
                //The prompt text is disabled when the player walks out of range
                promptText.enabled = false;
            }
        }
        else
        {
            //Allows the player to enter the train in Wadebridge
            if (collisionObject.tag == "Player" && collisionObject.transform.position.x > -5)
            {
                promptText.enabled = true;
                PlayerInput input = collisionObject.GetComponent<PlayerInput>();
                InputAction interaction = input.actions.FindAction("Interact");

                //Puts the player in the train and starts the train moving
                if (interaction.IsPressed())
                {
                    animator.SetTrigger("Arriving");
                    onBoardCamera.GetComponent<Camera>().enabled = true;
                    onBoardCamera.GetComponent<AudioListener>().enabled = true;
                    GameObject.FindGameObjectWithTag("Leaving").GetComponent<CapsuleCollider2D>().enabled = true;
                    Destroy(collisionObject);
                    StartCoroutine(MoveDelay());
                }
            }
            else
            {
                //The prompt text is disabled when the player walks out of range
                promptText.enabled = false;
            }
        }

        //Sets up the NPC to take a seat
        if (collisionObject.tag == "NPC")
        {
            AssignSeat(collisionObject);
        }

        //Stops the train if at a station
        if (collisionObject.tag == "Station")
        {
            ArriveAtStation(collision);
        }

        //Loads the new scene when the train leaves
        if (collisionObject.tag == "Leaving")
        {
            if(SceneManager.GetActiveScene().name == "Waybridge")
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadScene(3);
            }
        }
    }

    //Cited from https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
    //Author Unity Technologies
    //Date 28/03/2023
    IEnumerator MoveDelay()
    {
        //Plays the train whistle sound effect
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.3f);
        //End of cited code
        move = true;
    }
}
