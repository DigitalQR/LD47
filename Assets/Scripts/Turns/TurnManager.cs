using DQR.Debug;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TurnState
{
	Inactive,
	Movement,
	Attacking
}

public enum TurnActionState
{
	Pending,
	Finished
}

public struct TurnAction
{
	public delegate TurnActionState DecisionCallback(int counter);

	public int Priority;
	public DecisionCallback Action;
}

public class TurnManager : SingletonBehaviour<TurnManager>
{
	private List<TurnCoordinator> m_Coordinators = new List<TurnCoordinator>();
	private TurnState m_CurrentState = TurnState.Inactive;
	
	private List<TurnAction> m_ActionQueue = new List<TurnAction>();
	private Queue<TurnCoordinator> m_CoordinatorQueue = new Queue<TurnCoordinator>();
	private TurnCoordinator m_PreviousCoordinator = null;
	private int m_DecisionCounter = 0;

	protected override void SingletonInit()
	{
	}

	public void RegisterCoordinator(TurnCoordinator coordinator)
	{
		Assert.Condition(!m_Coordinators.Contains(coordinator));
		m_Coordinators.Add(coordinator);
	}

	public void UnregisterCoordinator(TurnCoordinator coordinator)
	{
		Assert.Condition(m_Coordinators.Contains(coordinator));
		m_Coordinators.Remove(coordinator);
	}

	private void Update()
    {
		if (m_Coordinators == null || m_Coordinators.Count == 0)
			return;
		
		// Update decisions
		while (m_CoordinatorQueue.Count != 0)
		{
			var coordinator = m_CoordinatorQueue.Peek();

			if (coordinator != null)
			{
				if (m_PreviousCoordinator != coordinator)
				{
					EventHandler.Invoke("OnCoordinatorTurnBegin", coordinator);
					m_PreviousCoordinator = coordinator;
				}

				// Only update this decision at a frame, if pending
				if (coordinator.GenerateDecisions(m_CurrentState) == TurnCoordinator.DecisionState.Pending)
					return;
				
				EventHandler.Invoke("OnCoordinatorTurnEnd", coordinator);
			}

			m_CoordinatorQueue.Dequeue();
		}

		// Update actions
		while (m_ActionQueue.Count != 0)
		{
			var action = m_ActionQueue[0];

			if (action.Action(m_DecisionCounter++) == TurnActionState.Pending)
				return;

			m_ActionQueue.RemoveAt(0);
			m_DecisionCounter = 0;
		}

		NextPhase();
    }

	private void NextPhase()
	{
		Assert.Message(m_CoordinatorQueue.Count == 0, "Coordinator queue is not empty");
		Assert.Message(m_ActionQueue.Count == 0, "Action queue is not empty");

		switch (m_CurrentState)
		{
			case TurnState.Movement:
				SetCurrentState(TurnState.Attacking);
				break;

			case TurnState.Inactive:
			case TurnState.Attacking:
				SetCurrentState(TurnState.Movement);
				break;
		}

		if (EncounterManager.Instance.IsEncounterActive && EncounterManager.Instance.EncounterCount == 1)
		{
			Vector3 popupLocation = EncounterManager.Instance.CurrentEncounterContainer.transform.position;
			if (m_CurrentState == TurnState.Movement)
				PopupManager.Instance.CreateHeadingPopup3D("Movement", "Move closer to attack or fall back to defend", popupLocation, 2.0f);
			else if (m_CurrentState == TurnState.Attacking)
				PopupManager.Instance.CreateHeadingPopup3D("Attack", "!", popupLocation, 2.0f);
		}
	}

	private void SetCurrentState(TurnState state)
	{
		m_CoordinatorQueue.Clear();
		m_ActionQueue.Clear();
		m_PreviousCoordinator = null;

		//UnityEngine.Debug.Log($"Next phase '{m_CurrentState}'");
		m_CurrentState = state;
		EventHandler.Invoke("OnTurnStateChange", state);
		
		// Update coordinator queue (If requires input, prioritise)
		foreach (var coordinator in m_Coordinators.OrderBy((c) => c.RequiresRealtimeInput ? 1 : 0))
			m_CoordinatorQueue.Enqueue(coordinator);
	}

	public void QueueAction(TurnAction action)
	{
		m_ActionQueue.Add(action);
		m_ActionQueue.Sort((a, b) => a.Priority - b.Priority);
	}

	private void Event_OnEncounterBegin(EncounterType type)
	{
		SetCurrentState(TurnState.Inactive);
	}
}
