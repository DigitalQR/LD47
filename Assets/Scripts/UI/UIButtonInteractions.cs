﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonInteractions : MonoBehaviour
{
	[SerializeField]
	private GameObject m_WorldSpacePointer = null;

	[SerializeField]
	private UIAttackInteractions m_AttackInteractions = null;

	[SerializeField]
	private UIInventory m_InventoryInteractions = null;

	[Header("Buttons")]
	[SerializeField]
	private GameObject[] m_AttackGroup = null;

	[SerializeField]
	private GameObject[] m_MovementGroup = null;

	[SerializeField]
	private GameObject[] m_EndEncounterGroup = null;

	[SerializeField]
	private GameObject[] m_DefeatGroup = null;

	private PlayerCoordinator m_CurrentCoordinator = null;

	private void Start()
	{
		m_WorldSpacePointer.SetActive(false);
	}

	private void Update()
	{
		bool showState = false;

		if (m_CurrentCoordinator && m_CurrentCoordinator.PreviousKnownState == TurnState.Attacking)
		{
			Pawn pawn = m_CurrentCoordinator.GetCurrentDecisionPawn();
			if (pawn)
			{
				m_WorldSpacePointer.transform.position = pawn.transform.position;
				showState = true;
			}
		}

		m_WorldSpacePointer.SetActive(showState);
	}

	private void Event_OnCoordinatorTurnBegin(TurnCoordinator coordinator)
	{
		m_CurrentCoordinator = coordinator as PlayerCoordinator;
	}

	private void Event_OnCoordinatorTurnEnd(TurnCoordinator coordinator)
	{
		m_AttackInteractions.CloseMenu(m_CurrentCoordinator);
		m_CurrentCoordinator = null;
	}

	private void ViewButtonGroup(GameObject[] group)
	{
		// Disable everything
		for (int i = 0; i < transform.childCount; ++i)
			transform.GetChild(i).gameObject.SetActive(false);
		
		foreach (var obj in group)
			obj.gameObject.SetActive(true);
	}

	private void Event_OnTurnStateChange(TurnState state)
	{
		CloseAnything();

		if (state == TurnState.Attacking)
			ViewButtonGroup(m_AttackGroup);
		else if(state == TurnState.Movement)
			ViewButtonGroup(m_MovementGroup);
	}

	private void Event_OnGameOver()
	{
		PopupManager.Instance.CreateHeadingPopup3D("Game Over", (EncounterManager.Instance.BadEncounterCount - 1) + " encounters cleared", EncounterManager.Instance.CurrentEncounterContainer.transform.position, 1.5f, 30.0f);

		CloseAnything();
		ViewButtonGroup(m_DefeatGroup);
	}

	private void Event_OnEncounterBegin(EncounterType type)
	{
	}

	private void Event_OnEncounterEnd(EncounterType type)
	{
		ViewButtonGroup(m_EndEncounterGroup);
	}
	
	private void Event_OnTileHover(ArenaTile tile)
	{
		if (tile.HasContent && tile.Content.TryGetComponent(out Pawn pawn))
			pawn.ShowInfoPanel();
		else
			InfoPanelManager.Instance.Close();
	}

	private void CloseAnything()
	{
		m_AttackInteractions.CloseMenu(m_CurrentCoordinator);
		m_InventoryInteractions.Close();
	}


	public void Button_Attack()
	{
		m_InventoryInteractions.Close();

		if (m_CurrentCoordinator && m_CurrentCoordinator.PreviousKnownState == TurnState.Attacking)
		{
			m_AttackInteractions.ToggleOpen(m_CurrentCoordinator);
		}
	}
	
	public void Button_Bag()
	{
		m_AttackInteractions.CloseMenu(m_CurrentCoordinator);
		m_InventoryInteractions.ToggleOpen(m_CurrentCoordinator);
	}

	public void Button_Pass()
	{
		CloseAnything();

		if (m_CurrentCoordinator)
		{
			if (m_CurrentCoordinator.PreviousKnownState == TurnState.Attacking)
				m_CurrentCoordinator.NextDecisionPawn();
			else
				m_CurrentCoordinator.FlagPassTurn();
		}
	}

	public void Button_Continue()
	{
		CloseAnything();
		EncounterManager.Instance.SpawnNextEncounter();
	}

	public void Button_MainMenu()
	{
		CloseAnything();

		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.buildIndex);
	}

	public void Button_Quit()
	{
		Application.Quit();
	}
}
