using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The players animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The input component on the player
    /// </summary>
    private PlayerInput input;

    /// <summary>
    /// The move action defined by the player input
    /// </summary>
    private InputAction moveAction;

    /// <summary>
    /// The players acceleration
    /// </summary>
    [SerializeField]
    private float acceleration = 5f;

    /// <summary>
    /// The states the player can have
    /// </summary>
    enum State
    {
        IDLE,
        MOVE
    }

    /// <summary>
    /// The current state of the player
    /// </summary>
    private State state = State.IDLE;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        moveAction = input.actions.FindAction("Movement");
    }

    private void LateUpdate()
    {
        switch (state)
        {
            case (State.IDLE):
                IdleState();
                break;
            case (State.MOVE):
                MoveState();
                break;
        }
    }

    /// <summary>
    /// The move state of the player
    /// </summary>
    private void MoveState()
    {
        //Checks if the user has input a movement action then moves the player according the the value of move action
        if(moveAction.ReadValue<float>() != 0)
        {
            float xMovement = moveAction.ReadValue<float>();
            float yMovement = 0;
            Vector2 moveVector = new Vector2(xMovement, yMovement) * Time.deltaTime * acceleration;
            transform.Translate(moveVector);
            animator.SetFloat("Speed", xMovement);
        }
        else
        {
            state = State.IDLE;
        }
    }

    /// <summary>
    /// Controls the idle state of the player
    /// </summary>
    private void IdleState()
    {
        if (moveAction.ReadValue<float>() != 0)
        {
            state = State.MOVE;
            return;
        }
        animator.SetFloat("Speed", 0);
    }
}
