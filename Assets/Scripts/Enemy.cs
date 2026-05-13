/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Enemy handles non-player-Unit Movement(), Shoot() & Die() calls. Performs
 *          enemy (aka Player) detection and SFX.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {

    public float lookDistance; // Enemy detection raycast distance.
    public float raycastHeight; // For casting enemy detection raycast and instantiating projectiles.
    public float shotDelayTimer; // Time between shots (so enemies don't shoot 60/sec).
    public int health;
    public AudioClip detectionSFX;

    private bool shotFired; // Used to keep Enemy from firing at 60 shots/sec.
    private bool enemyDetected = false;
    private bool firstDetection = true; // Used to keep detectionSFX from firing 60/sec.

    void FixedUpdate()
    {
        // Only run the rest of the FixedUpdate function if the unit is alive & game is unpaused.
        if (!isAlive || GameManager.gamePaused)
            return;
        else if (health <= 0)
        {
            Die(gameObject.name);
            return;
        }

        // recievedDamage is set by Projectile. Damaged() plays damagedSFX.
        if (recievedDamage)
            Damaged();

        // Change direction if there's no more platform in front of the unit.
        if (!IsGrounded(-groundedRaycastXOffset))
            direction = 1;
        else if (!IsGrounded(groundedRaycastXOffset))
            direction = -1;

        // If enemyDetected, stop moving and shoot.
        if (enemyDetected)
        {
            firstDetection = false; // Used in SearchForEnemy() to play detectionSFX.
            Movement(0, false, true);
            if (!shotFired)
            {
                Shoot(raycastHeight);
                shotFired = true;
                StartCoroutine(DelayShot()); // Sets shotFired to false after shotDelayTimer secs.
            }
        }

        // If no enemy detected, continue moving. 
        else if (!enemyDetected)
        {
            shotFired = false;
            firstDetection = true;
            Movement(direction, false, true);
        }

        // Reset enemyDetected boolean.
        enemyDetected = false;

        // Create new V3 based on new position and adjust it's values accordingly.
        Vector3 origin = transform.position;
        origin.y += raycastHeight; // Raise the raycaster off the floor.

        // Search for enemy within sight. Sets the enemyDetected bool depending on result.
        SearchForEnemy(origin);

        // Check if Enemy fell below the death Y position.
        if (rigidBody.transform.position.y < GameManager.deathYPos)
            Die(gameObject.name);
    }

    // Draw a raycaster and see if it hits a gameObject with a MasterChef script.
    void SearchForEnemy(Vector3 origin)
    {
        // Draw the V2 to the right, multiply it by direction, which flips it if negative.
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, Vector2.right * direction, lookDistance);

        // If the collider isn't null and hit object with MasterChef script, an enemy was detected.
        if (hitInfo.collider != null && hitInfo.collider.GetComponent<Player>() != null)
        {
            enemyDetected = true;
            if (firstDetection)
                audioSource.PlayOneShot(detectionSFX);
        }
    }

    // This is the coroutine for the Enemy to pause between shots.
    IEnumerator DelayShot()
    {
        yield return new WaitForSeconds(shotDelayTimer);
        // Check to see if its true first, otherwise if player goes out of detection range and
        // then reenters, this will prematurely reset the delay. 
        if (shotFired)
            shotFired = false;
    }
}
