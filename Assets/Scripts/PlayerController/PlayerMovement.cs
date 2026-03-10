using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private PlayerSkateController MyController;
    public PlayerSwing playerSwing;
    
    public float jumpForce = 5f;
    public float moveForce = 5f;
    public float maxSpeed = 10f;
    

    // Animation : stephen
    public Animator animator;
    private Rigidbody2D _rb;
    private bool _isGrounded = false; // Variable to check if the player is grounded : stephen

    // Start is called before the first frame update
    void Start()
    {
        // Get or Add Player Controller
        MyController = GetComponent<PlayerSkateController>();
        if (!MyController)
        {
            Debug.Log($"Object {this.name}| Needs PlayerController Component to function properly.");
            this.AddComponent<PlayerSkateController>();
        }
        
        _rb = GetComponent<Rigidbody2D>(); // Get the rigidbody component : stephen
    }
    // Update is called once per frame : stephen
   void Update()
{
    if (MyController.Jump.pressed && !playerSwing.isSwinging)
    {
        _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        // Start a coroutine that will wait 0.1 seconds before checking for swingable object
        StartCoroutine(DelayedSwingCheck());
    }

    if (_isGrounded)
    {
        Debug.Log("Grounded");
        HandleWalking();
    }

    if (playerSwing.isSwinging)
    {
        HandleSwinging();
        Debug.Log("Swinging");
    }

    if(!playerSwing.isSwinging)
    {
       transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    if (_rb.velocity.x == 0 || !_isGrounded)
    {
        animator.SetFloat("speed", 0f);
    }
}

IEnumerator DelayedSwingCheck()
{
    yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
    playerSwing.AttachToSwingable(); // Then check for swingable object
}

void HandleWalking() // Function to handle walking animations and movement : stephen
{
    float movementDirection = MyController.moveVector.x;
    float movementSpeed = Mathf.Abs(_rb.velocity.x);

   
    if (movementSpeed < maxSpeed)
    {
        float movementForce = movementDirection * moveForce;
        Vector2 force = new Vector2(movementForce, 0);
        _rb.AddForce(force); // Apply the force to the Rigidbody2D
    }

    // Set the sprite's facing direction based on the move vector.
    if (movementDirection != 0)
    {
        transform.localScale = new Vector3(movementDirection < 0 ? -1 : 1, 1, 1);
    }

    // Set animation speed if grounded : stephen
    if (_isGrounded)
    {
        animator.SetBool("swing", false);
        animator.SetFloat("speed", movementSpeed > 0 ? 1f : 0f);
    }
    else
    {
        animator.SetFloat("speed", 0f);
    }
}



// This needs to be modified so it uses the player controller instead of keyboard input
   void HandleSwinging()
{
    float torque = 0f;
    animator.SetBool("swing", true);

    // Get the horizontal axis value from the player controller
    float horizontalAxis = MyController.moveVector.x;

    // Get the vertical axis value from the player controller
    float verticalAxis = MyController.moveVector.y;

    if (horizontalAxis < 0)
    {
        Debug.Log("Left");
        torque = -5f; // Counterclockwise torque
        transform.localScale = new Vector3(-1, 1, 1);
    }
    else if (horizontalAxis > 0)
    {
        Debug.Log("Right");
        torque = 5f; // Clockwise torque
        transform.localScale = new Vector3(1, 1, 1);
    }
    else if (verticalAxis > 0)
    {
       playerSwing.ExtendRope();
    }
    else if (verticalAxis < 0)
    {
        Debug.Log("Down");
        playerSwing.ShortenRope();
    }
    else if(MyController.Jump.pressed)
    {
     
      playerSwing.DetachFromSwingable();
      //playerSwing.isSwinging = false;
     
    }
   
    _rb.AddTorque(torque);
}

    void OnCollisionEnter2D(Collision2D collision) // Check collision with the ground : stephen
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _rb.freezeRotation = true; // rotation freeze : stephen
            // Ensure the character is standing 0,0,0 rotation
            transform.rotation = Quaternion.Euler(0, 0, 0);
            playerSwing.DetachFromSwingable();
        }
    }
    void OnCollisionExit2D(Collision2D collision) // Check exit collision with the ground : stephen
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
            _rb.freezeRotation = false; // rotation unfreeze : stephen
        }
    }
}