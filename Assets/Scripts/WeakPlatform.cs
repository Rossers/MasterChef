/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Weak Platforms are red platforms that- when stepped- shake a bit, 
 *          then fall and are destroyed.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class WeakPlatform : MonoBehaviour
{
    public float standDuration; // Time before platform falls.
    public float shakeStrength; // Magnitude of turbulance platform will shake with.
    public float fallDuration; // Larger = slower. Time it takes to fall.

    private bool isTriggered = false; // Used to keep platform from shaking 60 times/second.

    void Update()
    {
        // When the platform falls low enough that its no longer important to gameplay, 
        // destroy it.
        if (transform.position.y < GameManager.deathYPos)
        {
            transform.DetachChildren();
            Destroy(gameObject);
        }
            
        // Since in Unit.Movement(), units are made a child of a platform they're standing
        // on, we know something is standing on the weak platform when its child count > 0. 
        if (transform.childCount > 0 && isTriggered == false)
        {
            isTriggered = true;
            Tweener shakeTween = transform.DOShakePosition(standDuration, shakeStrength);
            shakeTween.SetRelative(true);
            shakeTween.SetEase(Ease.OutQuint);
            StartCoroutine(DropPlatform()); // When the shaking stops, it begins to fall.
        }
    }

    IEnumerator DropPlatform()
    {
        yield return new WaitForSeconds(standDuration);
        // Send the platform on a path as if its falling.
        Vector3 fall = new Vector3(transform.position.x, transform.position.y - 100);
        Tweener fallTween = transform.DOMove(fall, fallDuration);
        fallTween.SetRelative(true);
        fallTween.SetEase(Ease.InCirc);
    }
}
