using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttackCategory
{
	None,
	Melee,
	Ranged,
	Magic
}

public enum AttackArea
{
	Circle,
	Row,
	Column,
}

[System.Flags]
public enum AttackCastTarget
{
	None = 0,
	Self = 1,
	Ally = 2,
	Enemy = 4,

	AnyTeamTile = Self | Ally,
	AnyEnemyTile = Enemy,
}

[System.Flags]
public enum AttackAffectedTargets
{
	None = 0,
	Self = 1,
	Ally = 2,
	Enemy = 4,
}

public class DamageEvent
{
	public Pawn Caster;
	public Pawn Target;
	public AttackCategory Category;
	public int DamageAmount;
	public float Accuracy;
}

public class AttackAction : MonoBehaviour
{
	public enum AttackState
	{
		Pending,
		Failed,
		Finished
	}

	[SerializeField]
	private int m_CastCost = 1;

	[SerializeField]
	private int m_MinDamageAmount = 1;

	[SerializeField]
	private int m_MaxDamageAmount = 1;

	[SerializeField]
	private float m_Accuracy = 1.0f;

	[SerializeField]
	private AttackCategory m_Category = AttackCategory.None;

	[SerializeField]
	private int m_AttackCastBreadth = 1;

	[SerializeField]
	private int m_AttackCastRange = 1;

	[SerializeField]
	private int m_AttackAffectRange = 0;

	[SerializeField]
	private AttackArea m_AttackAffectArea = AttackArea.Circle;

	[SerializeField]
	private AttackCastTarget m_CastTarget = AttackCastTarget.Enemy;

	[SerializeField]
	private AttackAffectedTargets m_AffectedTargets = AttackAffectedTargets.Enemy;

	private AttackAnimator m_Animator = null;
	private Pawn m_CurrentCaster = null;
	private ArenaTile m_CurrentTarget = null;
	private List<Pawn> m_DamagedPawns = null;

	private void Start()
	{
		m_Animator = GetComponent<AttackAnimator>();
	}

	public int CastCost
	{
		get => Mathf.Max(0, m_CastCost);
	}

	public IEnumerable<ArenaTile> GatherConsideredTiles(Pawn caster)
	{
		List<ArenaTile> tiles = new List<ArenaTile>();
				
		if (m_CastTarget == AttackCastTarget.Self)
		{
			tiles.Add(caster.CurrentTile);
		}
		if ((m_CastTarget & AttackCastTarget.AnyTeamTile) != 0)
		{
			var teamTiles = ArenaBoard.Instance.GetTilesForTeam(caster.TeamIndex);

			if (m_CastTarget.HasFlag(AttackCastTarget.Self))
				tiles.AddRange(teamTiles);
			else
				tiles.AddRange(teamTiles.Where((t) => t != caster.CurrentTile));
		}

		if ((m_CastTarget & AttackCastTarget.AnyEnemyTile) != 0)
		{
			void WalkLOS(Vector2Int baseCoord, int offset)
			{
				for (int x = offset; x <= m_AttackCastRange; ++x)
				{
					Vector2Int coord = baseCoord + Vector2Int.Scale(caster.FacingCoordDir, new Vector2Int(x, 0));

					if (ArenaBoard.Instance.TryGetTile(coord, out ArenaTile tile))
					{
						if (tile != caster.CurrentTile)
						{
							if (tile.TeamIndex != caster.TeamIndex)
								tiles.Add(tile);

							if (tile.HasContent)
								return;
						}
					}
				}
			};

			// Do LOS checks for enemy tiles
			for (int y = -m_AttackCastBreadth; y <= m_AttackCastBreadth; ++y)
			{
				int offset = 0;

				// If not in current row, start for 1 infront to not get blocked by alies in the same row
				if (y != 0)
					offset = 1;

				WalkLOS(caster.CurrentCoords + new Vector2Int(0, y), offset);
			}
		}
		
		return tiles.Where((t) => InCastRange(caster, t));
	}

	private bool InCastRange(Pawn caster, ArenaTile tile)
	{
		if (m_AttackCastRange < 0)
			return true;
		
		return ArenaTile.CoordDistance(caster.CurrentCoords, tile.Coord) <= m_AttackCastRange;
	}

	public IEnumerable<ArenaTile> GetAffectedTiles(ArenaTile target)
	{
		// Filter team indices here
		return ArenaBoard.Instance.AllArenaTiles.Where((t) =>
		{
			if (t.GetCoordDistance(target) <= m_AttackAffectRange && t.TeamIndex == target.TeamIndex)
			{
				if (m_AttackAffectArea == AttackArea.Row)
					return t.Coord.y == target.Coord.y;

				if (m_AttackAffectArea == AttackArea.Column)
					return t.Coord.x == target.Coord.x;

				return true;
			}

			return false;
		});
	}

	public IEnumerable<Pawn> GetAffectedPawns(Pawn caster, ArenaTile target)
	{
		List<Pawn> pawns = new List<Pawn>();
		foreach (var tile in GetAffectedTiles(target))
		{
			if (tile.HasContent)
			{
				Pawn currPawn = tile.Content.GetComponent<Pawn>();
				if (currPawn)
				{
					if ((m_AffectedTargets.HasFlag(AttackAffectedTargets.Self) && currPawn == caster) ||
						(m_AffectedTargets.HasFlag(AttackAffectedTargets.Ally) && currPawn.TeamIndex == caster.TeamIndex) ||
						(m_AffectedTargets.HasFlag(AttackAffectedTargets.Enemy) && currPawn.TeamIndex != caster.TeamIndex)
						)
					{
						pawns.Add(currPawn);
					}
				}
			}
		}

		return pawns;
	}

	public void BeginAttack(Pawn caster, ArenaTile target)
	{
		Assert.Message(m_CurrentCaster == null, "Attack is already executing");
		m_CurrentCaster = caster;
		m_CurrentTarget = target;
		m_DamagedPawns = null;

		if (m_Animator)
			m_Animator.BeginAttack(caster, target);
		else
			ApplyCurrentAttack();
	}

	public void ApplyCurrentAttack()
	{
		Assert.Message(m_CurrentCaster != null, "Attack isn't executing");
		if (m_CurrentCaster != null)
		{
			m_DamagedPawns = new List<Pawn>();

			foreach (var targetPawn in GetAffectedPawns(m_CurrentCaster, m_CurrentTarget))
			{
				DamageEvent damageEvent = new DamageEvent();
				damageEvent.Caster = m_CurrentCaster;
				damageEvent.Category = m_Category;
				damageEvent.DamageAmount = Random.Range(m_MinDamageAmount, m_MaxDamageAmount + 1);
				damageEvent.Target = targetPawn;
				damageEvent.Accuracy = m_Accuracy;

				m_DamagedPawns.Add(targetPawn);
				targetPawn.ReceiveDamage(damageEvent);
			}
		}

		m_CurrentCaster = null;
		m_CurrentTarget = null;
	}

	public bool InBlockingAnimating
	{
		get => (m_Animator && m_Animator.InBlockingAnimating) || (m_DamagedPawns != null && m_DamagedPawns.Where((p) => p.InBlockingAnimating).Any());
	}
}
