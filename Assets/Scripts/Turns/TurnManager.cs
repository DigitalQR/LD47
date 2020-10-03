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
	public delegate TurnActionState DecisionCallback();

	public int Priority;
	public DecisionCallback Action;
}

public class TurnManager : SingletonBehaviour<TurnManager>
{
	[SerializeField]
	private TurnCoordinator[] m_Coordinators = null;
	
	private TurnState m_CurrentState = TurnState.Inactive;
	
	private List<TurnAction> m_ActionQueue = new List<TurnAction>();
	private Queue<TurnCoordinator> m_CoordinatorQueue = new Queue<TurnCoordinator>();

	protected override void SingletonInit()
	{
	}
	
	private void Update()
    {
		if (m_Coordinators == null || m_Coordinators.Length == 0)
			return;
		
		// Update decisions
		while (m_CoordinatorQueue.Count != 0)
		{
			var coordinator = m_CoordinatorQueue.Peek();

			// Only update this decision at a frame, if pending
			if (coordinator.GenerateDecisions(m_CurrentState) == TurnCoordinator.DecisionState.Pending)
				return;

			m_CoordinatorQueue.Dequeue();
		}

		// Update actions
		while (m_ActionQueue.Count != 0)
		{
			var action = m_ActionQueue[0];

			if (action.Action() == TurnActionState.Pending)
				return;

			m_ActionQueue.RemoveAt(0);
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
				m_CurrentState = TurnState.Attacking;
				break;

			case TurnState.Inactive:
			case TurnState.Attacking:
				m_CurrentState = TurnState.Movement;
				break;
		}

		UnityEngine.Debug.Log($"Next phase '{m_CurrentState}'");
		EventHandler.Invoke("OnTurnStateChange", m_CurrentState);

		// Update coordinator queue (If requires input, prioritise)
		foreach (var coordinator in m_Coordinators.OrderBy((c) => c.RequiresRealtimeInput ? 0 : 1))
			m_CoordinatorQueue.Enqueue(coordinator);
	}

	public void QueueAction(TurnAction action)
	{
		m_ActionQueue.Add(action);
		m_ActionQueue.Sort((a, b) => a.Priority - b.Priority);
	}
}
