using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurnCoordinator : MonoBehaviour
{
	public enum DecisionState
	{
		Pending,
		Finished
	}

	protected TurnState m_PreviousKnownState = TurnState.Inactive;

	protected virtual void Start()
	{
		TurnManager.Instance.RegisterCoordinator(this);
	}

	protected virtual void OnDestroy()
	{
		if(TurnManager.IsValid)
			TurnManager.Instance.UnregisterCoordinator(this);
	}

	public abstract bool RequiresRealtimeInput
	{
		get;
	}

	public TurnState PreviousKnownState
	{
		get => m_PreviousKnownState;
	}

	public virtual DecisionState GenerateDecisions(TurnState turnState)
	{
		DecisionState decisionState = DecisionState.Finished;
		if (turnState != TurnState.Inactive)
			decisionState = GenerateDecisionsInternal(turnState);

		m_PreviousKnownState = turnState;
		return decisionState;
	}
	
	protected abstract DecisionState GenerateDecisionsInternal(TurnState turnState);

	public void QueueAction(int priority, TurnAction.DecisionCallback callback)
	{
		QueueAction(new TurnAction { Priority = priority, Action = callback });
	}

	public void QueueAction(TurnAction action)
	{
		TurnManager.Instance.QueueAction(action);
	}

	public void ClearConsideredTiles()
	{
		ArenaBoard.Instance.ClearConsideredTiles();
	}
}
