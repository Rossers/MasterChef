/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Projectile is instantiated by Unit.Shoot().
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int projectileDamage;
    public float projectileSpeed;
    public float projectileLifetime; // Destroyed after this many seconds.
    public GameObject owner; // Used for differentiating the shooter from the target.

    private float direction = 1;

	void Start () {
        // Countdown and destroy the projectile.
        Destroy(gameObject, projectileLifetime);

        // Unit.Shoot() sets correct SpriteRenderer direction. Need to store 
        // direction for movement.
        if (this.GetComponent<SpriteRenderer>().flipX)
            direction = -1;
	}
	
	void Update () {
        // Translate and multiply by Time.deltaTime so pauseMenu freezes projectiles.
        transform.Translate(Vector3.right * Time.deltaTime * projectileSpeed * direction);
    }

    void OnTriggerEnter2D (Collider2D otherCollider)
    {
        string name = otherCollider.name;
        GameObject target = otherCollider.gameObject;
        // Check the name against the owner otherwise when Projectile is instantiated,
        // it will immediately hit its owner and destroy itself.
        if (name.Contains("MasterChef") && target != owner)
        {
            Player player = target.GetComponent<Player>();
            GameManager.playerHealth = GameManager.playerHealth - projectileDamage;
            player.recievedDamage = true; // Unit.Player will use this to play audio.
            Destroy(gameObject);
        }
        // DutchOvenant and MicroOvenant both contain Ovenant.
        else if (name.Contains("Ovenant") && target != owner)
        {
            Enemy enemy = otherCollider.GetComponent<Enemy>();
            enemy.health = enemy.health - projectileDamage;
            enemy.recievedDamage = true;
            Destroy(gameObject);
        }
        else if (name.Contains("Platform"))
            Destroy(gameObject);
        // Can't put Destroy() outside of else if or it Destroys immediately.
    }
}
