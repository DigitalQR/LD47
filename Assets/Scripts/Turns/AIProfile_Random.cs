using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIProfile_Random : AIProfile
{
	public override float CalculateMovementScore(AICoordinator coordinator, Dictionary<Pawn, ArenaTile> config)
	{
		return Random.value;
	}

	public override bool ChooseAttackAction(AICoordinator coordinator, Pawn caster, out AttackAction attack, out ArenaTile target)
	{
		AttackAction[] actions = caster.AttackActions.ToArray();
		int rand = Random.Range(0, actions.Length - 1);

		attack = actions[rand];

		var tiles = attack.GatherConsideredTiles(caster).Where((t) => t.HasContent).ToArray();
		if (tiles.Any())
		{
			rand = Random.Range(0, tiles.Length - 1);
			target = tiles[rand];
		}
		else
		{
			target = null;
			return false;
		}

		return true;
	}
}
