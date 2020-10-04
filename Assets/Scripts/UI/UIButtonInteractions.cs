using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonInteractions : MonoBehaviour
{
	[SerializeField]
	private UIAttackInteractions m_AttackInteractions = null;

	[Header("Buttons")]
	[SerializeField]
	private GameObject[] m_AttackGroup = null;

	[SerializeField]
	private GameObject[] m_MovementGroup = null;

	[SerializeField]
	private GameObject[] m_ContinueGroup = null;

	[SerializeField]
	private GameObject[] m_DefeatGroup = null;

	private PlayerCoordinator m_CurrentCoordinator = null;

	private void Event_OnCoordinatorTurnBegin(TurnCoordinator coordinator)
	{
		m_CurrentCoordinator = coordinator as PlayerCoordinator;

	}

	private void Event_OnCoordinatorTurnEnd(TurnCoordinator coordinator)
	{
		m_CurrentCoordinator = null;
		m_AttackInteractions.CloseMenu();
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
		if (state == TurnState.Attacking)
			ViewButtonGroup(m_AttackGroup);
		else if(state == TurnState.Movement)
			ViewButtonGroup(m_MovementGroup);
	}

	private void Event_OnTeamCoordinatorDefeat(TeamTurnCoordinator coordinator)
	{
		if (coordinator is PlayerCoordinator)
			ViewButtonGroup(m_DefeatGroup);
		else
			ViewButtonGroup(m_ContinueGroup);
	}

	public void Button_Attack()
	{
		if (m_CurrentCoordinator && m_CurrentCoordinator.PreviousKnownState == TurnState.Attacking)
		{
			m_AttackInteractions.OpenMenu(m_CurrentCoordinator);
		}
	}
	
	public void Button_Bag()
	{
		m_AttackInteractions.CloseMenu();
	}

	public void Button_Pass()
	{
		m_AttackInteractions.CloseMenu();

		if (m_CurrentCoordinator && m_CurrentCoordinator.PreviousKnownState == TurnState.Attacking)
			m_CurrentCoordinator.NextDecisionPawn();
	}

	public void Button_Continue()
	{
		m_AttackInteractions.CloseMenu();

		if (m_CurrentCoordinator)
			m_CurrentCoordinator.FlagPassTurn();
	}

	public void Button_MainMenu()
	{
		m_AttackInteractions.CloseMenu();
	}
}
