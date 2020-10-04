using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnableButtonForTurn : MonoBehaviour
{
	[SerializeField]
	private Button[] m_Targets = null;
	
	private void Event_OnCoordinatorTurnBegin(TurnCoordinator coordinator)
	{
		bool enableState = coordinator is PlayerCoordinator;

		foreach (var button in m_Targets)
			button.interactable = enableState;

	}

	private void Event_OnCoordinatorTurnEnd(TurnCoordinator coordinator)
	{
		if (coordinator is PlayerCoordinator)
		{
			foreach (var button in m_Targets)
				button.interactable = false;
		}
	}
}
