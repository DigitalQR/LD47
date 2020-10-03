using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TileContentCursor))]
public class PlayerCoordinator : TeamTurnCoordinator
{
	private TileContentCursor m_TileCursor;

	public GameObject m_TESTPrefab;
	public int m_TESTCount;

	protected override void Start()
	{
		base.Start();

		m_TileCursor = GetComponent<TileContentCursor>();
		m_TileCursor.enabled = false;

		for (int i = 0; i < m_TESTCount; ++i)
			InstantiatePawn(m_TESTPrefab);
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

		//m_MovementManager.enabled = false;
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
