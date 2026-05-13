/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Player & Enemy inherit from Unit. Unit has Movement(), Shoot() & Die() which
 *          are used often. Has most of the AudioClips.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour {

    public float moveSpeed;
    public float groundedRaycastXOffset; // X offset used to check if there is ground to the left/right of unit.
    public float airSpeedFactor; // Modifier percent like 0.9. Used to slightly slow horizontal speed while airborne.
    public float shootXOffset; // X offset to place new projectiles outside self collider.
    public float deathReloadDelay; // Delay before reloading the scene.
    public Projectile projectilePrefab;
    public AudioClip shootSFX;
    public AudioClip deathSFX;
    public AudioClip damagedSFX;
    public float deathXVelocity; // deathX/YVelocities used when characters die to give them a jump.
    public float deathYVelocity;
    public bool recievedDamage;

    protected float direction = 1; // -1 = left & 1 = right
    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected CapsuleCollider2D capCollider; // Used when adjusting collider size.
    protected AudioSource audioSource;
    protected bool isAlive = true;

    private SpriteRenderer spriteRenderer;
    private float groundedRaycastDistance = 0.05f;
    private float groundedRaycastYOffset = 0.01f; // Lower raycaster so its beneath self collider.

    void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        capCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
    // Flip sprites according to direction.
    protected float Movement(float amount, bool crouching, bool grounded)
    {
        // Get the sprite animation facing the correct direction.
        if (amount > 0)
        {
            spriteRenderer.flipX = false;
            direction = 1;
        }
        else if (amount < 0)
        {
            spriteRenderer.flipX = true;
            direction = -1;
        }
         
        animator.SetFloat("Movement", Mathf.Abs(amount));

        // If crouching and grounded, stop moving horizontally.
        if (crouching && grounded)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
        // If crouching and not grounded, continue moving at reduced horizontal speed.
        // Move faster running than jumping. airSpeedFactor should be a percent like 0.9.
        else if (crouching && !grounded || !crouching && !grounded)
        {
            rigidBody.velocity = new Vector2(amount * (moveSpeed * airSpeedFactor), rigidBody.velocity.y);
        }
        // If not crouching and grounded, continue moving as normal.
        else
        {
            rigidBody.velocity = new Vector2(amount * moveSpeed, rigidBody.velocity.y);
        }
        return direction;
    }

    // Called by a unit to see if a downward raycast drawn at the given X offset hits a platform.
    protected bool IsGrounded(float offsetX)
    {
        // Create a V3 and adjust its X by the given offset. 
        Vector3 origin = transform.position;
        origin.x += offsetX;
        origin.y -= groundedRaycastYOffset; // Lower Y position to avoid unit's own collider.

        RaycastHit2D hitInfo = Physics2D.Raycast(origin, Vector2.down, groundedRaycastDistance);
        if (hitInfo.collider == null)
        {
            transform.SetParent(null);
            return false;
        }
        else if (hitInfo.collider.GetComponent<MovingPlatform>() != null || hitInfo.collider.GetComponent<WeakPlatform>() != null)
        {
            transform.SetParent(hitInfo.transform);
        }
        
        return true;
    }

    // Called by any unit when shooting.
    protected void Shoot(float height)
    {
        // Create V3 and adjust it's Y to the right height and X to be outside it's own collider.
        Vector3 origin = transform.position;
        origin.y += height;
        if (direction > 0)
            origin.x += shootXOffset;
        else
            origin.x -= shootXOffset;

        // Instantiate a Projectile gameObject.
        Projectile projectileClone = Instantiate(projectilePrefab, origin, transform.rotation);
        projectileClone.owner = this.gameObject;
        
        // If the unit is facing left, flip the sprite.
        if (direction < 0)
            projectileClone.GetComponent<SpriteRenderer>().flipX = true;

        audioSource.PlayOneShot(shootSFX);
    }

    // Plays the appropriate damaged SFX for the Unit.
    protected void Damaged ()
    {
        audioSource.PlayOneShot(damagedSFX);
        recievedDamage = false;
    }

    // Called by any unit when dying.
    public void Die(string name)
    {
        // Avoid playing multiple deathSound effects while waiting for unit to die.
        if (!isAlive)
            return;

        // If Player dies, kill music, detatch camera, set 'falling' animation.
        if (name == "MasterChef")
        {
            GameObject backgroundMusic = GameObject.FindGameObjectWithTag("BackgroundMusic");
            Destroy(backgroundMusic);
            Camera.main.transform.SetParent(null); // Detach camera, parent about to be destroyed.
            animator.SetBool("IsAirborne", true);
            animator.SetBool("IsCrouching", false);
            // Wait a moment before displaying the death menu so players can see how they died.
            StartCoroutine(DelayGameEnd());
        }
        // If Enemy dies, increment score counter.
        else
        {
            GameManager.ovenantDestroyedCounter++;
            Destroy(gameObject, deathReloadDelay);
        }
            
        // Perform all shared Die actions between Enemy & Player: deathSFX, destroy collider &
        // eventually destroy GameObject.  
        isAlive = false;
        audioSource.PlayOneShot(deathSFX);
        Destroy(gameObject.GetComponent<CapsuleCollider2D>());
        rigidBody.velocity = new Vector2(deathXVelocity, deathYVelocity);
    }

    // This function delays the death menu from appearing immediately upon Player death.
    IEnumerator DelayGameEnd()
    {
        yield return new WaitForSeconds(deathReloadDelay);
        GameManager.gameOver = true;
        Destroy(gameObject);
    }
}
