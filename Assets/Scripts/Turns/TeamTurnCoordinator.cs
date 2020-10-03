using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TeamTurnCoordinator : TurnCoordinator
{
	[SerializeField]
	private int m_TeamIndex = 0;

	private List<Pawn> m_Pawns = new List<Pawn>();

	// TESTING
	public Pawn m_TESTPrefab;
	public int m_TESTCount;

	protected override void Start()
	{
		base.Start();
		InstantiatePawns(m_TESTPrefab, m_TESTCount);
	}

	private void LateUpdate()
	{
		// Clear out dead
		for (int i = 0; i < m_Pawns.Count; ++i)
		{
			if (m_Pawns[i] == null)
				m_Pawns.RemoveAt(i--);
		}
	}

	public override DecisionState GenerateDecisions(TurnState turnState)
	{
		if (OwnedPawnCount == 0)
		{
			Debug.Log("Decide with 0 pawns huh?");
			m_PreviousKnownState = turnState;
			return DecisionState.Finished;
		}
		
		return base.GenerateDecisions(turnState);
	}

	public IEnumerable<ArenaTile> GetTeamTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex == m_TeamIndex);
	}

	public IEnumerable<ArenaTile> GetEnemyTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex != m_TeamIndex);
	}

	public IEnumerable<Pawn> OwnedPawns
	{
		get => m_Pawns;
	}

	public int OwnedPawnCount
	{
		get => m_Pawns.Count;
	}

	public Pawn GetPawn(int index)
	{
		return index < m_Pawns.Count ? m_Pawns[index] : null;
	}

	public IEnumerable<Pawn> InstantiatePawns(Pawn prefab, int count)
	{
		List<Pawn> objs = new List<Pawn>();
		for (int i = 0; i < count; ++i)
			objs.Add(InstantiatePawn(prefab));

		return objs;
	}

	public Pawn InstantiatePawn(Pawn prefab)
	{
		foreach (var tile in GetTeamTiles())
		{
			if (!tile.HasContent)
			{
				Pawn newPawn = Instantiate(prefab, transform);
				newPawn.TeamIndex = m_TeamIndex;
				newPawn.SetFacingDirection(GetAttackCoordForward());
				newPawn.ApplyRandomVariantion();
				tile.Content = newPawn.gameObject;

				m_Pawns.Add(newPawn);
				return newPawn;
			}
		}

		Assert.FailFormat("Couldn't find empty tile for new pawn '{0}'", prefab);
		return null;
	}

	public Vector3 GetAttackDirection()
	{
		return ArenaBoard.Instance.GetTeamAttackDirection(m_TeamIndex);
	}

	public Vector2Int GetAttackCoordForward()
	{
		Vector3 dir = GetAttackDirection();

		return new Vector2Int(
			Mathf.Abs(dir.x) > 0.4f ? Mathf.RoundToInt(Mathf.Sign(dir.x)) : 0,
			Mathf.Abs(dir.z) > 0.4f ? Mathf.RoundToInt(Mathf.Sign(dir.z)) : 0
		);
	}

	public Vector2Int GetAttackCoordUp()
	{
		Vector2Int dir = GetAttackCoordForward();

		return new Vector2Int(dir.y, dir.x);
	}

	protected TurnActionState Action_WaitOnPawnAnimations(int count)
	{
		foreach (var pawn in OwnedPawns)
		{
			if (pawn.InBlockingAnimating)
				return TurnActionState.Pending;
		}

		return TurnActionState.Finished;
	}

	protected TurnActionState Action_ExecuteAttack(int count, Pawn caster, ArenaTile target, AttackAction attack)
	{
		if (count == 0)
			attack.BeginAttack(caster, target);

		if (attack.InBlockingAnimating)
			return TurnActionState.Pending;

		return TurnActionState.Finished;
	}
}
