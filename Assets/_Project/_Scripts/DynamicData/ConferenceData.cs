using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConferenceData", menuName = "ScriptableObjects/ConferenceData")]
public class ConferenceData : ScriptableObject
{
	[SerializeField] private string conferenceId;

	public string ConferenceId { get => conferenceId; set => conferenceId = value; }
}
