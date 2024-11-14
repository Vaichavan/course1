using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        [Tooltip("The current player's health.")]
        public float health;

        [Tooltip("The maximum health the player can have.")]
        public float maxHealth;

        [Tooltip("How fast the player runs.")]
        public float speed;

        [Tooltip("How high the player jumps.")]
        public float jumpForce;

        [Tooltip("Whether the player is allowed to move or not.")]
        public bool canMove;

        [Tooltip("When the player is allowed to jump or not.")]
        public bool canJump;
    }

    public Stats playerStats;

    [Tooltip("The number of coins the player has collected.")]
    public int coinsCollected = 0;

    [Tooltip("The script that will play the player's sound effects.")]
    public SoundManager soundManager;

    [Tooltip("Which layer allows the player to jump.")]
    public LayerMask groundLayer;

    [Tooltip("The transform that detects what layer the player is on.")]
    public Transform groundCheckL, groundCheckR;

    [Tooltip("The transform that the player's directional movement will be based upon.")]
    public Transform mainCamera;

    private float moveX, moveY;
    private float facing;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Set maxHealth to the player's initial health
        playerStats.maxHealth = playerStats.health;
    }

    // Function to restore health, ensuring it does not exceed maxHealth
    public void RestoreHealth(float amount)
    {
        playerStats.health = Mathf.Min(playerStats.health + amount, playerStats.maxHealth);
        print($"Player's health: {playerStats.health}");
    }

    // Function to handle coin collection
    public void CollectCoin()
    {
        coinsCollected++;

        // Check if the player has collected 10 coins
        if (coinsCollected >= 10)
        {
            RestoreHealth(1); // Restore one health point
            coinsCollected = 0; // Reset coin count
        }
    }

    private void Update()
    {
        if (playerStats.canMove == true)
        {

            // maps movement onto WASD keys and arrow keys
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");  

            // creates linecasts that check for the ground layer, allowing the player to jump
            bool hitL = Physics.Linecast(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, groundLayer);
            bool hitR = Physics.Linecast(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, groundLayer);
            Debug.DrawLine(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, Color.red);
            Debug.DrawLine(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, Color.red);

            if (hitL || hitR)
            {
                playerStats.canJump = true;
            }
            else
            {
                playerStats.canJump = false;
            }

            if (playerStats.canJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerStats.canMove == true)
        {
            
            // directional movement is wrapped around which way the camera is facing
            Vector3 movement = ((mainCamera.right * moveX) * playerStats.speed) + ((mainCamera.forward * moveY) * playerStats.speed);
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            // player faces the direction they are moving towards
            if (movement.x != 0 && movement.z != 0)
            {
                facing = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            }
            rb.rotation = Quaternion.Euler(0, facing, 0);  
        }
    }
    
    private void Jump()
    {
        playerStats.canJump = false;
        soundManager.PlayJumpSound();
        rb.AddForce(Vector3.up * playerStats.jumpForce);
    }

    public void ChangeHealth(float amount)
    {
        playerStats.health += amount;
        print($"Player's health: {playerStats.health}");
    }

}
