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

	private TurnState m_PreviousState = TurnState.Inactive;

	protected virtual void Start()
	{
		
	}

	public abstract bool RequiresRealtimeInput
	{
		get;
	}

	public TurnState PreviousKnownState
	{
		get => m_PreviousState;
	}

	public DecisionState GenerateDecisions(TurnState turnState)
	{
		var decisionState = GenerateDecisionsInternal(turnState);
		m_PreviousState = turnState;
		return decisionState;
	}
	
	protected abstract DecisionState GenerateDecisionsInternal(TurnState turnState);

	public void QueueAction(TurnAction action)
	{
		TurnManager.Instance.QueueAction(action);
	}

	protected void ClearConsideredTiles()
	{
		ArenaBoard.Instance.ClearConsideredTiles();
	}
}
