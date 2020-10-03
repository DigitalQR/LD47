using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICoordinator : TeamTurnCoordinator
{
	public override bool RequiresRealtimeInput
	{
		get => false;
	}

	protected override DecisionState GenerateDecisionsInternal(TurnState turnState)
	{
		if (turnState == TurnState.Movement)
		{
			QueueAction(0, Action_MovePawns);
			QueueAction(0, Action_WaitOnPawnAnimations);
		}
		return DecisionState.Finished;
	}

	private TurnActionState Action_MovePawns()
	{
		foreach (var pawn in OwnedPawns)
			MovePawnToRandomTile(pawn);

		return TurnActionState.Finished;
	}

	protected void MovePawnToRandomTile(Pawn pawn)
	{
		var tiles = GetTeamTiles().ToArray();

		// Try for a bit but give up after a while
		for (int i = 0; i < 10; ++i)
		{
			int index = Random.Range(0, tiles.Length);
			var tile = tiles[index];

			if (tile.HasContent)
			{
				// Have decided to place where we already are
				if (tile.Content == pawn)
					return;
			}
			else
			{
				// Found an empty tile
				pawn.MoveToTile(tile, true);
				return;
			}
		}
	}
}
