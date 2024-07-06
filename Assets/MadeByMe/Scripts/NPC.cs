using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    /// <summary>
    /// Returns true if the player can interact with the NPC
    /// </summary>
    private bool interact = false;

    /// <summary>
    /// The input action component on the player
    /// </summary>
    private InputAction playerInteraction = null;

    /// <summary>
    /// The player gameobject
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Is true when the NPC is talking
    /// </summary>
    private bool isRunning = false;

    /// <summary>
    /// Is true when the NPC is moving
    /// </summary>
    private bool moving = false;

    /// <summary>
    /// Prompt text for the player to interact with the NPC
    /// </summary>
    private TMP_Text promptText;

    /// <summary>
    /// The NPCs animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The carriage the NPC wants a ticket for
    /// </summary>
    [SerializeField]
    public string targetCarrage;

    /// <summary>
    /// The current velocity of the NPC
    /// </summary>
    private float speed = 0;

    /// <summary>
    /// Changes the NPCs velocity randomly
    /// </summary>
    private bool settingSpeed = false;

    /// <summary>
    /// A reference to the talk canvas
    /// </summary>
    private GameObject talkCanvas;

    /// <summary>
    /// All the possible NPC states
    /// </summary>
    private enum State
    {
        IDLE,
        MOVING,
        TALK
    }

    /// <summary>
    /// The current state of the NPC
    /// </summary>
    [SerializeField]
    private State state = State.IDLE;

    private void Awake()
    {       
        promptText = gameObject.GetComponentInChildren<TMP_Text>();
        animator = GetComponent<Animator>();
        talkCanvas = GameObject.Find("TalkCanvas");
        talkCanvas.GetComponent<TalkCanvas>().finished.AddListener(FinishedTalk);
    }

    private void Start()
    {
        //Adds the one to the prompt telling the player how many NPCs they can find
        if (targetCarrage != "N/A" && targetCarrage != "")
        {
            GameObject.Find("MarkText").GetComponent<SetPromptText>().AddAmount();
        }
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case State.IDLE:
                IdleState();
                break;
            case State.MOVING:
                MoveState();
                break;
            case State.TALK:
                TalkState();
                break;
        }
    }

    /// <summary>
    /// Handles the movement of the NPC
    /// </summary>
    private void MoveState()
    {
        //If the NPC has no carriage they move randomly when not talking
        if(targetCarrage == "N/A")
        {
            //Checks if the player has interacted with the NPC
            if (interact && playerInteraction != null && playerInteraction.IsPressed())
            {
                state = State.TALK;
                return;
            }

            if (!settingSpeed)
            {
                StartCoroutine(SetSpeed());
            }
            //Randomly moves the NPC according to their velocity
            transform.Translate(new Vector2(speed, 0) * Time.deltaTime);
            animator.SetFloat("Speed", speed);
        }
        else
        {
            //Checks if the NPC has just disembarked and sets the movement vector accordingly
            if(targetCarrage != "")
            {
                //Moves the NPC towards the train based on the current scene
                if (SceneManager.GetActiveScene().name == "Waybridge")
                {
                    transform.Translate(new Vector2(1, 0) * Time.deltaTime * 5);
                    animator.SetFloat("Speed", 1);
                }
                else
                {
                    transform.Translate(new Vector2(-1, 0) * Time.deltaTime * 5);
                    animator.SetFloat("Speed", -1);
                }
            }
            else if (targetCarrage == "")
            {
                //Moves the NPC towards the train based on the current scene
                if (SceneManager.GetActiveScene().name == "Waybridge")
                {
                    transform.Translate(new Vector2(-1, 0) * Time.deltaTime * 5);
                    animator.SetFloat("Speed", -1);
                }
                else
                {
                    transform.Translate(new Vector2(1, 0) * Time.deltaTime * 5);
                    animator.SetFloat("Speed", 1);
                }
            }


        }
    }

    /// <summary>
    /// The idle state method
    /// </summary>
    private void IdleState()
    {
        //Sets the random movement if the NPC doesn't have a target carriage
        if (targetCarrage == "N/A")
        {
            moving = true;
        }

        if (moving)
        {
            state = State.MOVING;
        }

        //Checks if the player has interacted with the NPC
        if (interact && playerInteraction != null && playerInteraction.IsPressed())
        {
            state = State.TALK;
        }
    }

    /// <summary>
    /// The method that manages the talk state
    /// </summary>
    private void TalkState()
    {
        if (!isRunning)
        {
            talkCanvas.GetComponent<TalkCanvas>().TStart(gameObject);
            interact = false;
            isRunning = true;
            player.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    /// <summary>
    /// The method that runs when the NPC has finished talking
    /// </summary>
    private void FinishedTalk()
    {
        if (isRunning)
        {
            isRunning = false;
            player.GetComponent<PlayerMovement>().enabled = true;
            interact = true;
            promptText.enabled = false;

            //If the NPC has a target carriage it starts making its way towards the train
            if(targetCarrage != "N/A")
            {
                Embarking();
            }
            state = State.IDLE;
        }
    }

    /// <summary>
    /// Sets the NPC up to take the seat in the train
    /// </summary>
    public void SeatTaken()
    {
        state = State.IDLE;
        animator.SetFloat("Speed", 0);
        moving = false;
        GetComponent<Collider2D>().enabled = false;
    }

    /// <summary>
    /// Set up the NPC to start moving towards the train
    /// </summary>
    private void Embarking()
    {
        //Checks if the player is in interact range
        if (interact)
        {
            moving = true;

            //Removes one from the prompt amount of NPCs to find
            GameObject.Find("MarkText").GetComponent<SetPromptText>().RemoveAmount();
            GetComponentsInChildren<SpriteRenderer>()[1].enabled = false;
        }
    }

    /// <summary>
    /// The method that runs when the NPC disembarkes from the train
    /// </summary>
    public void Disembarked()
    {
        //Gets the NPC off the train and starts them moving
        transform.SetParent(null);
        transform.position = new Vector3(transform.position.x, 0, 0);
        GetComponent<Collider2D>().enabled = false;
        state = State.MOVING;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Enables the interactions between the player and the NPC
        if (collision.gameObject.tag == "Player")
        {
            promptText.enabled = true;
            player = collision.gameObject;
            interact = true;

            if (playerInteraction == null)
            {
                PlayerInput playerInput = collision.gameObject.GetComponent<PlayerInput>();
                playerInteraction = playerInput.actions.FindAction("Interact");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Checks if the player has left the NPC's collider then disables all prompt text
        if (collision.gameObject.tag == "Player")
        {
            interact = false;
            promptText.enabled = false;
        }
    }

    //Cited from https://docs.unity3d.com/ScriptReference/WaitForSeconds.html
    //Author Unity Technologies
    //Date 28/03/2023
    IEnumerator SetSpeed()
    {
        settingSpeed = true;
        yield return new WaitForSeconds(5f);

        speed = Random.Range(-1, 2);
        settingSpeed = false;
    }
}
