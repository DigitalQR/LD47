using DQR.Debug;
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
		public int SelectedAttackIndex;
		public ArenaTile SelectedTile;

	};

	[SerializeField]
	private PawnSpawnSettings m_InitSettings = null;

	private TileContentCursor m_TileCursor;
	private bool m_PassTurnFlag = false;
	
	private int m_CurrentPawnIndex = 0;
	private AttackData m_TempAttackData = default;

	protected override void Start()
	{
		base.Start();

		m_TileCursor = GetComponent<TileContentCursor>();
		m_TileCursor.SetPaused(true);

		SpawnPawns(m_InitSettings);
	}

	public override bool RequiresRealtimeInput
	{
		get => true;
	}

	public TileContentCursor TileCursor
	{
		get => m_TileCursor;
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
		m_TempAttackData.SelectedAttackIndex = -1;
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
			if (currentPawn)
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

							if (m_TempAttackData.SelectedAttackIndex >= 0)
							{
								var attackAction = m_TempAttackData.AttackOptions[m_TempAttackData.SelectedAttackIndex];

								foreach (var tile in attackAction.GatherConsideredTiles(currentPawn))
									tile.MarkAsConsidered();

								m_TempAttackData.CurrentState = AttackData.State.AwaitingSelection;
							}
							break;
						}

					case AttackData.State.AwaitingSelection:
						{
							if (m_TempAttackData.SelectedTile)
							{
								// Attack selected
								Pawn caster = currentPawn;
								ArenaTile target = m_TempAttackData.SelectedTile;
								AttackAction attack = m_TempAttackData.AttackOptions[m_TempAttackData.SelectedAttackIndex];
								QueueAttack(caster, target, attack);
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
			// All handle through tile cursor, so just setup and wait
			if (AnyPawnsInBlockingAnimation)
			{
				m_TileCursor.SetPaused(true);
				ClearConsideredTiles();
			}
			else
			{
				if (m_TileCursor.SetPaused(false))
					SelectMovementTiles();
			}
		}

		if (m_PassTurnFlag)
		{
			endTurn = true;
			m_PassTurnFlag = false;
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
		m_TempAttackData.SelectedAttackIndex = -1;
	}

	private void Event_OnTileCursorContentChanged(TileContentCursor cursor)
	{
		if (PreviousKnownState == TurnState.Movement && cursor == m_TileCursor)
			SelectMovementTiles();
	}

	private void Event_OnTileSelected(ArenaTile tile)
	{
		if (m_PreviousKnownState == TurnState.Attacking && m_TempAttackData.CurrentState == AttackData.State.AwaitingSelection)
			m_TempAttackData.SelectedTile = tile;
	}

	private void SelectMovementTiles()
	{
		ClearConsideredTiles();

		var tiles = GetTeamTiles();

		if (m_TileCursor.HasContent)
		{
			Pawn currPawn = m_TileCursor.Content.GetComponent<Pawn>();
			tiles = GetMovementTilesFor(currPawn).Where((t) => !t.HasContent);
		}
		else
			tiles = tiles.Where((t) => t.HasContent);

		foreach (var tile in tiles)
			tile.MarkAsConsidered();
	}

	public void FlagPassTurn()
	{
		m_PassTurnFlag = true;
	}

	public AttackAction[] CurrentAttackActions()
	{
		Assert.Message(PreviousKnownState == TurnState.Attacking, "Expected to be attacking");
		return m_TempAttackData.AttackOptions;
	}

	public void SetCurrentAttackAction(int i)
	{
		Assert.Message(PreviousKnownState == TurnState.Attacking, "Expected to be attacking");
		m_TempAttackData.SelectedAttackIndex = i;
		m_TempAttackData.CurrentState = AttackData.State.ViewSelected;
	}
}
