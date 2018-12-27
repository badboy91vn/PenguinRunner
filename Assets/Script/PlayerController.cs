using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Const
    private const float LAND_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;
    private const float SLIDING_TIME = 1.0f;
    // - Land
    private const int LEFT = 0;
    private const int MID = 1;
    private const int RIGHT = 2;
    // - Animation
    private const string JUMP = "Jump";
    private const string SLIDING = "Sliding";
    private const string GROUNDED = "Grounded";
    private const string DEATH = "Death";
    private const string STARTRUNNING = "StartRunning";

    // Gameplay
    private Animator anim;
    private bool isRunning = false;

    // Movement
    private CharacterController controller;
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLand = 1; // 0: Left, 1: Mid, 2: Right

    // Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedInscreaseLastTick = 7.0f;
    private float speedInscreaseTime = 2.0f;
    private float speedInscreaseAmount = 0.1f;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        speed = originalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        if (Time.time - speedInscreaseLastTick > speedInscreaseTime)
        {
            speedInscreaseLastTick = Time.time;
            speed += speedInscreaseAmount;

            // Update UI Speed
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

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
        // -- Calculate X
        float x = Mathf.Clamp(transform.position.x, -LAND_DISTANCE, LAND_DISTANCE);
        if (desiredLand == MID && x >= -0.2 && x <= 0.2) { x = 0; }
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        // -- Calculate Y
        bool isGrounded = controller.isGrounded; //IsGround();
        anim.SetBool(GROUNDED, isGrounded);
        if (isGrounded) // Grounded
        {
            verticalVelocity = 0;//-0.1f;

            //if (Input.GetKeyDown(KeyCode.Space))
            if (MobileInput.Instance.SwipeUp)
            {
                anim.SetTrigger(JUMP);
                verticalVelocity = jumpForce; // jump
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                StartSliding(); // Sliding
                Invoke("StopSliding", SLIDING_TIME);
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
        moveVector.y = verticalVelocity;
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

    private void StartSliding()
    {
        anim.SetBool(SLIDING, true);
        controller.height = controller.height * 0.5f;
        controller.center = new Vector3(controller.center.x, controller.center.y * 0.5f, controller.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool(SLIDING, false);
        controller.height = controller.height * 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void MoveLand(bool direc)
    {
        desiredLand += !direc ? -1 : 1;
        desiredLand = Mathf.Clamp(desiredLand, LEFT, RIGHT);
    }

    private void Dead()
    {
        isRunning = false;
        anim.SetTrigger(DEATH);
        GameManager.Instance.IsDead = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                print("Obstacle");
                Dead();
                break;

        }
    }

    private bool IsGround()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 0.5f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartStopGame()
    {
        isRunning = true;
        anim.SetTrigger(STARTRUNNING);
    }
}
