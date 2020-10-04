using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttackInteractions : MonoBehaviour
{
	[SerializeField]
	private UIAttackButton m_DefaultButton = null;
	
	private PlayerCoordinator m_RecentCoordinator = null;

	private void Start()
	{
		m_DefaultButton.gameObject.SetActive(false);
	}

	public void ToggleOpen(PlayerCoordinator coordinator)
	{
		if (m_RecentCoordinator == coordinator)
			CloseMenu(coordinator);
		else
			OpenMenu(coordinator);
	}

	public void OpenMenu(PlayerCoordinator coordinator)
	{
		if (m_RecentCoordinator != coordinator)
		{
			m_RecentCoordinator = coordinator;
			DestroyExistingOptions();

			// Build options
			var attackOptions = coordinator.CurrentAttackActions();

			for (int i = 0; i < attackOptions.Length; ++i)
			{
				UIAttackButton newButton = Instantiate(m_DefaultButton, transform);
				newButton.PopulateContent(coordinator, attackOptions[i], i);
				newButton.gameObject.SetActive(true);
			}
		}
	}

	public void CloseMenu(PlayerCoordinator coordinator, bool clearAction = true)
	{
		if (m_RecentCoordinator == coordinator)
		{
			m_RecentCoordinator = null;
			if (clearAction && coordinator != null && coordinator.PreviousKnownState == TurnState.Attacking)
				coordinator.SetCurrentAttackAction(-1);

			DestroyExistingOptions();
		}
	}

	private void DestroyExistingOptions()
	{
		foreach (var button in GetComponentsInChildren<UIAttackButton>())
		{
			if (button != m_DefaultButton)
				Destroy(button.gameObject);
		}
	}

	private void Event_OnTileSelected(ArenaTile tile)
	{
		if (m_RecentCoordinator != null)
		{
			if (m_RecentCoordinator.PreviousKnownState == TurnState.Attacking)
			{
				CloseMenu(m_RecentCoordinator, false);
			}
		}
	}
}
