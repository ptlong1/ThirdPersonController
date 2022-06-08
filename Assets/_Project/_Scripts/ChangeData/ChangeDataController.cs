using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Assets._Project._Scripts.Screen;
using Assets._Project._Scripts.DynamicData;

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
				TurnOffWatching(CurrentObjectIdx);
				int temp = value;
				if (temp < 0)
					temp += conferenceObjects.Length;
				currentObjectIdx = temp % conferenceObjects.Length; 
				UpdateInfo(conferenceObjects[currentObjectIdx]);
			}
		}


		private void Awake()
		{
			// UpdateObjectDataList();
		}

		public void UpdateObjectDataList()
		{
			conferenceObjects = FindObjectsOfType<ConferenceObjectData>();
			CurrentObjectIdx = 0;
			for (int i = 0; i < conferenceObjects.Length; ++i)
			{
				ConferenceIdButton btn = conferenceIdList.AddButton(conferenceObjects[i].id, i);
				btn.TransferToId += () =>
				{
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
			ConferenceScreen screen = data.GetComponent<ConferenceScreen>();
			updateDataWindow.currentType.text = screen.screenType.ToString();
			updateDataWindow.currentUrl.text = screen.urlContent;
			// screen.TriggerWatching();
			CameraView cameraView = data.GetComponent<CameraView>();
			UpdateCamera(cameraView.cameraTransform);
		}

		public void CurrentScreenTriggerWatching()
		{
			ConferenceScreen screen = conferenceObjects[CurrentObjectIdx].GetComponent<ConferenceScreen>();
			screen.TriggerWatching();
		}
		void UpdateCamera(Transform trans)
		{
			// virtualCam.transform.SetPositionAndRotation(trans.position, trans.rotation);
			// virtualCam.ForceCameraPosition(trans.position, trans.rotation);
			virtualCam.transform.DOMove(trans.position, 1f);
			virtualCam.transform.DORotateQuaternion(trans.rotation, 1f);
		}

		public void TurnOffWatching(int idx)
		{
			if (idx >= conferenceObjects.Length) 
			{
				Debug.Log("Index out of array bound");
				return;
			}
			ConferenceScreen screen = conferenceObjects[idx].GetComponent<ConferenceScreen>();
			screen.TurnOffWatching();
		}
	}
}