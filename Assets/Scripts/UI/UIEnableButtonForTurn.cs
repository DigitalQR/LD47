using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnableButtonForTurn : MonoBehaviour
{
	[SerializeField]
	private Button[] m_Targets = null;

	private PlayerCoordinator m_PrevCoordinator = null;

	private void Event_OnCoordinatorTurnBegin(TurnCoordinator coordinator)
	{
		m_PrevCoordinator = coordinator as PlayerCoordinator;
		bool enableState = m_PrevCoordinator != null;

		foreach (var button in m_Targets)
			button.interactable = enableState;

	}

	private void Update()
	{
		if (m_PrevCoordinator)
		{
			bool enableState = !m_PrevCoordinator.TileCursor.HasContent;

			foreach (var button in m_Targets)
				button.interactable = enableState;
		}
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
