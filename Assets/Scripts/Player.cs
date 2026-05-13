/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Player gets input for character actions, checks colliders & sets animations.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public float jumpHeight; 
    public float shootHeight; // Vertical offset so Player doesn't shoot on the floor.
    public float altShootHeight; // Vertical offset when crouched
    public float crouchHeight; // Collider height when crouched.
    
    // Store Player-specific sound effects.
    public AudioClip jumpSFX;
    public AudioClip emptySFX;
    public AudioClip reloadSFX;
    public AudioClip healthSFX;

    private bool isGrounded;
    private bool isCrouched;
    private bool wasStanding = true;

    // Default width & height for collider.
    private const float colliderDefaultWidth = 0.44f;
    private const float colliderDefaultHeight = 0.56f;
    
	void Update () {
        // Only run the rest of the Update function if alive & unpaused.
		if (!isAlive || GameManager.gamePaused)
            return;

        // If player was hit, damage them.
        if (recievedDamage)
            Damaged();

        // Get left/right input from user.
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Set isGrounded boolean based on downward facing raycasters to the left, right or middle.
        isGrounded = (IsGrounded(0) || IsGrounded(-groundedRaycastXOffset) || IsGrounded(groundedRaycastXOffset));

        // Set isCrouched boolean if the 'S' key is pressed (continuous).
        isCrouched = Input.GetKey(KeyCode.S);

        AnimatePlayer(isCrouched, isGrounded);
        Movement(horizontalInput, isCrouched, isGrounded);

        // If the player clicks the LMB, fire a shot.
        if (Input.GetKeyDown(KeyCode.Mouse0) && GameManager.ammoCount > 0)
        {
            if (isCrouched)
                Shoot(altShootHeight);
            else
                Shoot(shootHeight);
            GameManager.ammoCount--;
            GameManager.mealsServedCounter++;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
            audioSource.PlayOneShot(emptySFX); 

        // If the player is grounded and not crouching, allow them to jump.
        if (isGrounded && !isCrouched)
        {
           if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHeight);
                audioSource.PlayOneShot(jumpSFX);
            }
        }

        // Kill Player if they fall below death Y position.
        if (rigidBody.transform.position.y < GameManager.deathYPos)
        {
            Die(gameObject.name);
        }  
	}

    // Sets all the appropriate animator values.
    void AnimatePlayer (bool crouching, bool grounded)
    {
        if (crouching)
        {
            animator.SetBool("IsCrouching", true);
            // Crouching is also the reload mechanic.
            if (wasStanding)
            {
                // Only reload if they were standing. Avoids infinite ammo when crouched.
                GameManager.ammoCount = GameManager.maxPlayerAmmo;
                wasStanding = false;
                audioSource.PlayOneShot(reloadSFX);
            }
            capCollider.size = new Vector2(colliderDefaultWidth, crouchHeight);
        }
        else
        {
            animator.SetBool("IsCrouching", false);
            wasStanding = true;
            capCollider.size = new Vector2(colliderDefaultWidth, colliderDefaultHeight);
        }
        if (grounded)
            animator.SetBool("IsAirborne", false);
        else
            animator.SetBool("IsAirborne", true);
    }

    // Handle Player colliding with Pickups and Enemies (DutchOvenants specifically).
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // Destroy the health pickup when Player collides with it.
        if (otherCollider.GetComponent<PickUp>() != null)
        {
            Destroy(otherCollider.gameObject);
            audioSource.PlayOneShot(healthSFX);
            GameManager.playerHealth = GameManager.maxPlayerHealth;
            GameManager.cheatMealsCounter++;
        }
        // Kill Player if it collides with an Enemy.
        else if (otherCollider.GetComponent<Enemy>() != null)
            Die(gameObject.name);
    }
}
