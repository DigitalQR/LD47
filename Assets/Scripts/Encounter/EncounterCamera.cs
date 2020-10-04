using DQR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCamera : MonoBehaviour
{
	private FocusCamera m_CurrCamera = null;

	private void Event_OnEncounterBegin(EncounterType type)
	{
		if (Camera.main.TryGetComponent(out m_CurrCamera))
		{
			m_CurrCamera.CurrentFocus = EncounterManager.Instance.CurrentEncounterContainer.transform;
			m_CurrCamera.OnTargetReached.AddListener(OnCameraFocusReached);
		}
	}

	private void OnCameraFocusReached()
	{
		if (m_CurrCamera != null)
		{
			EventHandler.Invoke("OnEncounterInCameraView", m_CurrCamera);

			m_CurrCamera.OnTargetReached.RemoveListener(OnCameraFocusReached);
			m_CurrCamera = null;
		}
	}
}
