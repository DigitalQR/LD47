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

	private TurnActionState Action_MovePawns(int count)
	{
		var moveConfig = SelectConfiguration();

		if (moveConfig != null)
		{
			foreach (var pawn in OwnedPawns)
				pawn.CurrentTile.Content = null;

			foreach (var pawn in OwnedPawns)
				pawn.MoveToTile(moveConfig[pawn], true);
		}

		return TurnActionState.Finished;
	}

	private Dictionary<Pawn, ArenaTile> GenerateRandomConfiguration()
	{
		Dictionary<Pawn, ArenaTile> targets = new Dictionary<Pawn, ArenaTile>();
		HashSet<ArenaTile> lockedTiles = new HashSet<ArenaTile>();
		
		foreach (var pawn in OwnedPawns)
		{
			var validTiles = GetMovementTilesFor(pawn).Where((t) => !lockedTiles.Contains(t)).ToArray();

			// Pawn had nowhere to go
			if (validTiles.Length == 0)
				return null;

			int rand = Random.Range(0, validTiles.Length);

			var tile = validTiles[rand];
			lockedTiles.Add(tile);
			targets.Add(pawn, tile);
		}

		return targets;
	}

	private float CaclulateMovementScore(Dictionary<Pawn, ArenaTile> config)
	{
		return 1.0f;
	}

	private Dictionary<Pawn, ArenaTile> SelectConfiguration(int countToTest = 10)
	{
		Dictionary<Pawn, ArenaTile> currBest = null;
		float bestScore = 0.0f;

		for (int i = 0; i < countToTest; ++i)
		{
			var test = GenerateRandomConfiguration();
			if (test != null)
			{
				float score = CaclulateMovementScore(test);

				if (currBest == null || score > bestScore)
				{
					currBest = test;
					bestScore = score;
				}
			}
		}

		return currBest;
	}
}
