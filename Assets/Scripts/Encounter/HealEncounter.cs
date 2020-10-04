using DQR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEncounter : MonoBehaviour
{
	[SerializeField]
	private int m_HealAmount = 20;
		
	private void Event_OnEncounterInCameraView(FocusCamera camera)
	{
		foreach (var health in FindObjectsOfType<PawnHealth>())
			health.ApplyHeal(m_HealAmount);

		gameObject.SetActive(false);
		EncounterManager.Instance.EndCurrentEncounter();
	}
}
