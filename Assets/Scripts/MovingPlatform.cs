/* Name: Ross A. Metcalf
 * Email: pg09ross@vfs.com
 * Date: 20170219
 * Summary: Moving platforms are blue and move back and forth.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 endPosition;
    public float duration; // Larger = moves slower.

    void Start()
    {
        Tweener moveTween = transform.DOMove(endPosition, duration);
        moveTween.SetRelative(true);
        moveTween.SetLoops(-1, LoopType.Yoyo); // -1 loop count = infinite.
        moveTween.SetEase(Ease.InOutSine);
    }
}