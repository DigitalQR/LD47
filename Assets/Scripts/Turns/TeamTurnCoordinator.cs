using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TeamTurnCoordinator : TurnCoordinator
{
	[SerializeField]
	private int m_TeamIndex = 0;
	
	public IEnumerable<ArenaTile> GetTeamTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex == m_TeamIndex);
	}

	public IEnumerable<ArenaTile> GetEnemyTiles()
	{
		return ArenaBoard.Instance.AllArenaTiles.Where((t) => t.TeamIndex != m_TeamIndex);
	}

	public GameObject InstantiatePawn(GameObject prefab)
	{
		foreach (var tile in GetTeamTiles())
		{
			if (!tile.HasContent)
			{
				var newObj = Instantiate(prefab, transform);
				tile.Content = newObj;
				return newObj;
			}
		}

		Assert.FailFormat("Couldn't find empty tile for new pawn '{0}'", prefab);
		return null;
	}
}
