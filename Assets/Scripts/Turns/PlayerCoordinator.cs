using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileContentCursor))]
public class PlayerCoordinator : TeamTurnCoordinator
{
	private struct AttackData
	{
		public enum State
		{
			Setup,
			ViewSelected,
			AwaitingSelection
		}

		public State CurrentState;
		public AttackAction[] AttackOptions;
		public int m_SelectedAttackIndex;
		public ArenaTile m_SelectedTile;

	};

	private TileContentCursor m_TileCursor;

	public bool m_TESTEndTurn = false;

	private int m_CurrentPawnIndex = 0;
	private AttackData m_TempAttackData = default;

	protected override void Start()
	{
		base.Start();

		m_TileCursor = GetComponent<TileContentCursor>();
		m_TileCursor.SetPaused(true);
	}

	public override bool RequiresRealtimeInput
	{
		get => true;
	}

	public Pawn GetCurrentDecisionPawn()
	{
		return GetPawn(m_CurrentPawnIndex);
	}

	public void NextDecisionPawn()
	{
		ClearConsideredTiles();
		m_CurrentPawnIndex++;
		m_TempAttackData = default;
	}

	protected override DecisionState GenerateDecisionsInternal(TurnState turnState)
	{
		if (turnState != PreviousKnownState)
		{
			SetupForState(turnState);
			return DecisionState.Pending;
		}

		bool endTurn = false;

		if (turnState == TurnState.Attacking)
		{
			Pawn currentPawn = GetCurrentDecisionPawn();
			if (currentPawn && currentPawn.HasAttackActions)
			{
				switch (m_TempAttackData.CurrentState)
				{
					case AttackData.State.Setup:
						{
							m_TempAttackData.AttackOptions = currentPawn.AttackActions.ToArray();
							m_TempAttackData.CurrentState = AttackData.State.ViewSelected;
							break;
						}

					case AttackData.State.ViewSelected:
						{
							ClearConsideredTiles();
							var attackAction = m_TempAttackData.AttackOptions[m_TempAttackData.m_SelectedAttackIndex];

							foreach (var tile in attackAction.GatherConsideredTiles(currentPawn))
								tile.MarkAsConsidered();

							m_TempAttackData.CurrentState = AttackData.State.AwaitingSelection;
							break;
						}

					case AttackData.State.AwaitingSelection:
						{
							if (m_TempAttackData.m_SelectedTile)
							{
								// Attack selected
								Pawn caster = currentPawn;
								ArenaTile target = m_TempAttackData.m_SelectedTile;
								AttackAction attack = m_TempAttackData.AttackOptions[m_TempAttackData.m_SelectedAttackIndex];
								QueueAction(0, (int count) => Action_ExecuteAttack(count, currentPawn, target, attack));

								NextDecisionPawn();
							}
							break;
						}
				}
			}
			else
				endTurn = true;
		}
		else if (turnState == TurnState.Movement)
		{
			// All handle through tile cursor, so just wait here
		}

		if (m_TESTEndTurn)
		{
			endTurn = true;
			m_TESTEndTurn = false;
		}

		if (endTurn)
		{
			m_TileCursor.SetPaused(true);
			ClearConsideredTiles();
			return DecisionState.Finished;
		}

		return DecisionState.Pending;
	}

	private void SetupForState(TurnState turnState)
	{
		// Reset
		ClearConsideredTiles();
		m_CurrentPawnIndex = 0;
		m_TempAttackData = default;

		if (turnState == TurnState.Movement)
		{
			SelectMovementTiles();
			m_TileCursor.SetPaused(false);
		}
	}

	private void Event_OnTileCursorContentChanged(TileContentCursor cursor)
	{
		if (PreviousKnownState == TurnState.Movement && cursor == m_TileCursor)
			SelectMovementTiles();
	}

	private void Event_OnTileSelected(ArenaTile tile)
	{
		if (m_PreviousKnownState == TurnState.Attacking && m_TempAttackData.CurrentState == AttackData.State.AwaitingSelection)
			m_TempAttackData.m_SelectedTile = tile;
	}

	private void SelectMovementTiles()
	{
		ClearConsideredTiles();

		var tiles = GetTeamTiles();

		if (m_TileCursor.HasContent)
			tiles = tiles.Where((t) => !t.HasContent);
		else
			tiles = tiles.Where((t) => t.HasContent);

		foreach (var tile in tiles)
			tile.MarkAsConsidered();
	}
}
