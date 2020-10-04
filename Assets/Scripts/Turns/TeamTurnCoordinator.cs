using DQR.Debug;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TeamTurnCoordinator : TurnCoordinator
{
	[System.Serializable]
	public class PawnSpawnSettings
	{
		[Range(1, 9)]
		public int MinPawnSpawns = 1;

		[Range(1, 9)]
		public int MaxPawnSpawns = 3;
		public WeightedCollection<Pawn> PawnOptions = null;

		public int MinEquipmentSpawns = 0;
		public int MaxEquipmentSpawns = 3;
		public WeightedCollection<EquipableItem> EquipmentOptions = null;
		public bool ApplyVariationToWeapons = true;
	}

	[SerializeField]
	private int m_TeamIndex = 0;

	[SerializeField]
	private int m_MaxMoveDistance = 1;

	private List<Pawn> m_Pawns = new List<Pawn>();
	private Dictionary<Pawn, IEnumerable<ArenaTile>> m_PawnMovementOption = new Dictionary<Pawn, IEnumerable<ArenaTile>>();
	
	protected override void Start()
	{
		base.Start();
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
			EventHandler.Invoke("OnTeamCoordinatorDefeat", this);

			m_PreviousKnownState = turnState;
			return DecisionState.Finished;
		}

		if (m_PreviousKnownState != turnState && turnState == TurnState.Movement)
		{
			m_PawnMovementOption.Clear();

			// Generate movement for each pawn
			var teamTiles = GetTeamTiles();

			foreach (var pawn in m_Pawns)
			{
				var pawnTiles = teamTiles.Where((t) => CalcMoveDistance(t, pawn.CurrentTile) <= m_MaxMoveDistance);
				m_PawnMovementOption.Add(pawn, pawnTiles.ToArray());
			}
		}

		return base.GenerateDecisions(turnState);
	}

	private int CalcMoveDistance(ArenaTile a, ArenaTile b)
	{
		Vector2Int delta = a.Coord - b.Coord;
		return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
	}
	
	public IEnumerable<ArenaTile> GetTeamTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex == m_TeamIndex);
	}

	public IEnumerable<ArenaTile> GetEnemyTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex != m_TeamIndex);
	}

	public IEnumerable<ArenaTile> GetMovementTilesFor(Pawn pawn)
	{
		if (m_PawnMovementOption.TryGetValue(pawn, out var tiles))
			return tiles;

		return new ArenaTile[0];
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

	public bool AnyPawnsInBlockingAnimation
	{
		get => m_Pawns.Where((p) => p.InBlockingAnimating).Any();
	}

	public void SpawnPawns(PawnSpawnSettings settings)
	{
		int pawnCount = Random.Range(settings.MinPawnSpawns, settings.MaxPawnSpawns + 1);
		int equipCount = Random.Range(settings.MinEquipmentSpawns, settings.MaxEquipmentSpawns + 1);

		for (int i = 0; i < pawnCount; ++i)
		{
			Pawn prefab = settings.PawnOptions.SelectRandom();
			InstantiatePawn(prefab);
		}

		for (int i = 0; i < equipCount; ++i)
		{
			EquipableItem prefab = settings.EquipmentOptions.SelectRandom();
			if (prefab)
			{
				EquipableItem newItem = Instantiate(prefab);

				if(settings.ApplyVariationToWeapons)
					newItem.ApplyVariantion();

				bool hasBeenEquiped = false;

				for (int n = 0; n < 10; ++n)
				{
					int randomIndex = Random.Range(0, OwnedPawnCount);
					Pawn pawn = GetPawn(randomIndex);
					EquipableTarget target = pawn.GetComponent<EquipableTarget>();

					if (target.TryEquipItem(newItem))
					{
						hasBeenEquiped = true;
						break;
					}
				}

				if (!hasBeenEquiped)
					Destroy(newItem.gameObject);
			}
		}
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
		if (prefab == null)
			return null;

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

	protected void QueueAttack(Pawn caster, ArenaTile target, AttackAction attack)
	{
		// TODO - Attack priority?
		int priority = Mathf.RoundToInt(caster.CurrentStats.Speed * 100);
		QueueAction(priority, (int count) => Action_ExecuteAttack(count, caster, target, attack));
	}

	private TurnActionState Action_ExecuteAttack(int count, Pawn caster, ArenaTile target, AttackAction attack)
	{
		if (caster == null || caster.IsPendingDestroy)
			return TurnActionState.Finished;

		if (count == 0)
			attack.BeginAttack(caster, target);

		if (attack.InBlockingAnimating)
			return TurnActionState.Pending;

		return TurnActionState.Finished;
	}
}
