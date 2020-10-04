using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AIProfile : MonoBehaviour
{
	public abstract bool ChooseAttackAction(AICoordinator coordinator, Pawn caster, out AttackAction attack, out ArenaTile target);
	public abstract float CalculateMovementScore(AICoordinator coordinator, Dictionary<Pawn, ArenaTile> config);
}
