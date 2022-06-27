using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FloatingIcon : MonoBehaviour
{
	public Transform target;
	public float deltaY;
	public float deltaScale;
	public float duration;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMoveY(deltaY, duration).SetLoops(-1, LoopType.Yoyo);
		// target.DOScale(Vector3.one*deltaScale, duration).SetLoops(-1, LoopType.Yoyo);
    }

}
