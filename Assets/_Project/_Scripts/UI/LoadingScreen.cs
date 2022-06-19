using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LoadingScreen : MonoBehaviour
{
	public Transform logo;
	public float maxRotationSpeed;
	public float duration;
	public Ease easeType;
	public float minWaitTime;
	float currentRotationSpeed;
	float currentWaitTime;
	Tween myTween;

	private void OnEnable() {
		logo.rotation = Quaternion.identity;
		currentRotationSpeed = 0f;
		currentWaitTime = 0f;
		myTween = DOTween.To(()=>currentRotationSpeed, x => currentRotationSpeed = x, maxRotationSpeed, duration)
			.SetEase(easeType)
			.OnUpdate(() => UpdateRotation())
			.SetLoops(-1, LoopType.Yoyo);
		logo.GetComponent<RawImage>().DOFade(1f, 0.5f).From(0f);
	}

	private void OnDisable() {
		myTween.Pause();
	}

	void UpdateRotation()
	{
		//logo.eulerAngles += new Vector3(0f,currentRotationSpeed*Time.deltaTime, 0f);
		logo.eulerAngles += new Vector3(0f, 0f,-currentRotationSpeed*Time.deltaTime);
	}
}
