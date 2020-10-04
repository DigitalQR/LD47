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
		m_PreviousCoordinator = null;

		switch (m_CurrentState)
		{
			case TurnState.Movement:
				m_CurrentState = TurnState.Attacking;
				break;

			case TurnState.Inactive:
			case TurnState.Attacking:
				m_CurrentState = TurnState.Movement;
				break;
		}

		//UnityEngine.Debug.Log($"Next phase '{m_CurrentState}'");
		EventHandler.Invoke("OnTurnStateChange", m_CurrentState);
		
		// Update coordinator queue (If requires input, prioritise)
		foreach (var coordinator in m_Coordinators.OrderBy((c) => c.RequiresRealtimeInput ? 1 : 0))
			m_CoordinatorQueue.Enqueue(coordinator);
	}

	public void QueueAction(TurnAction action)
	{
		m_ActionQueue.Add(action);
		m_ActionQueue.Sort((a, b) => a.Priority - b.Priority);
	}
}
