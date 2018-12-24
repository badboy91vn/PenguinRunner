using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;

    // Movement
    private CharacterController controller;
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private readonly float speed = 7.0f;
    private int desiredLand = 1; // 0: Left, 1: Mid, 2: Right

    // Const
    private const float LAND_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.05f;
    // - Land
    private const int LEFT = 0;
    private const int MID = 1;
    private const int RIGHT = 2;
    // - Animation
    private const string JUMP = "Jump";
    private const string GROUNDED = "Grounded";

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { MoveLand(false); }
        //if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { MoveLand(true); }

        if (MobileInput.Instance.SwipeLeft) MoveLand(false);
        if (MobileInput.Instance.SwipeRight) MoveLand(true);

        // Calculate where we should be in furture
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLand == LEFT)
        {
            targetPosition += Vector3.left * LAND_DISTANCE;
        }
        else if (desiredLand == RIGHT)
        {
            targetPosition += Vector3.right * LAND_DISTANCE;
        }

        // Calculate move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        // Calculate Y
        bool isGrounded = controller.isGrounded;
        anim.SetBool(GROUNDED, isGrounded);
        if (isGrounded) // Grounded
        {
            verticalVelocity = -.1f;

            //if (Input.GetKeyDown(KeyCode.Space))
            if (MobileInput.Instance.SwipeUp)
            {
                anim.SetTrigger(JUMP);
                verticalVelocity = jumpForce; // jump                
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            //if (Input.GetKeyDown(KeyCode.Space))
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce; // jump
            }
        }
        moveVector.y = verticalVelocity;// -0.1f;
        moveVector.z = speed;

        // Move Player
        controller.Move(moveVector * Time.deltaTime);

        // Rotate Player
        Vector3 direc = controller.velocity;
        if (direc != Vector3.zero)
        {
            direc.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direc, TURN_SPEED);
        }
    }

    void MoveLand(bool direc)
    {
        desiredLand += !direc ? -1 : 1;
        desiredLand = Mathf.Clamp(desiredLand, LEFT, 2);
    }
}
