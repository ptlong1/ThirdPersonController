using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Assets._Project._Scripts.Screen;

namespace Assets._Project._Scripts.ChangeData
{
	public class ChangeDataController : MonoBehaviour
	{
		public CinemachineVirtualCamera virtualCam;
		ConferenceObjectData[] conferenceObjects;
		private int currentObjectIdx;
		public UpdateDataWindow updateDataWindow;
		public ConferenceIdList conferenceIdList;

		public int CurrentObjectIdx { 
			get => currentObjectIdx; 
			set {
				int temp = value;
				if (temp < 0)
					temp += conferenceObjects.Length;
				currentObjectIdx = temp % conferenceObjects.Length; 
				UpdateInfo(conferenceObjects[currentObjectIdx]);
			}
		}

		private void Awake() {
			conferenceObjects = FindObjectsOfType<ConferenceObjectData>();
			CurrentObjectIdx = 0;
			for (int i = 0; i < conferenceObjects.Length; ++i)
			{
				ConferenceIdButton btn = conferenceIdList.AddButton(conferenceObjects[i].id, i);
				btn.TransferToId += () => {
					CurrentObjectIdx = btn.Idx;
				};
			}
		}
		// Start is called before the first frame update
		void Start()
		{

		}
		public void NextIdx(int value)
		{
			CurrentObjectIdx += value;
		}

		void UpdateInfo(ConferenceObjectData data)
		{
			updateDataWindow.Id = data.id;
			updateDataWindow.currentConferenceObject = data;
			updateDataWindow.currentType.text = data.GetComponent<ConferenceScreen>().screenType.ToString();
			updateDataWindow.currentUrl.text = data.GetComponent<ConferenceScreen>().urlContent;
			CameraView cameraView = data.GetComponent<CameraView>();
			UpdateCamera(cameraView.cameraTransform);
		}
		void UpdateCamera(Transform trans)
		{
			// virtualCam.transform.SetPositionAndRotation(trans.position, trans.rotation);
			// virtualCam.ForceCameraPosition(trans.position, trans.rotation);
			virtualCam.transform.DOMove(trans.position, 1f);
			virtualCam.transform.DORotateQuaternion(trans.rotation, 1f);
		}
	}
}