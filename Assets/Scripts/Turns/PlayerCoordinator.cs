using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileContentCursor))]
public class PlayerCoordinator : TeamTurnCoordinator
{
	private TileContentCursor m_TileCursor;

	public bool m_TESTEndTurn = false;
	
	protected override void Start()
	{
		base.Start();

		m_TileCursor = GetComponent<TileContentCursor>();
		m_TileCursor.enabled = false;
	}

	public override bool RequiresRealtimeInput
	{
		get => true;
	}

	protected override DecisionState GenerateDecisionsInternal(TurnState turnState)
	{
		if (turnState != PreviousKnownState)
		{
			SetupForState(turnState);
			return DecisionState.Pending;
		}


		if (m_TESTEndTurn)
		{
			m_TESTEndTurn = false;
			m_TileCursor.enabled = false;
			ClearConsideredTiles();
			return DecisionState.Finished;
		}

		return DecisionState.Pending;
	}

	private void SetupForState(TurnState turnState)
	{
		ClearConsideredTiles();

		if (turnState == TurnState.Movement)
		{
			SelectMovementTiles();
			m_TileCursor.enabled = true;
		}
	}

	private void Event_OnTileCursorContentChanged(TileContentCursor cursor)
	{
		if (PreviousKnownState == TurnState.Movement && cursor == m_TileCursor)
			SelectMovementTiles();
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
