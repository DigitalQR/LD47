using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaBoard : MonoBehaviour
{
	[SerializeField]
	private Vector2Int m_TeamAreaSize = new Vector2Int(3, 3);

	[SerializeField]
	private Vector2 m_TileScale = Vector2.one;

	[SerializeField]
	private ArenaTile m_TilePrefab = null;

	[SerializeField]
	private Transform[] m_TeamAreaBeacons = null;
		
	private Dictionary<Vector2Int, ArenaTile> m_ArenaTiles = null;
	private GameObject m_ActiveContainer = null;

	private Vector3 m_PlayAreaCentre;
	private Vector3[] m_TeamDirections;
	
    private void Start()
    {
		RegenerateBoard();
	}

	public int MaxTeamCount
	{
		get => m_TeamAreaBeacons.Length;
	}

	public IEnumerable<ArenaTile> AllArenaTiles
	{
		get => m_ArenaTiles.Values;
	}

	public void RegenerateBoard()
	{
		// Create container to contain every effect on this board
		if (m_ActiveContainer != null)
			Destroy(m_ActiveContainer);

		m_ActiveContainer = new GameObject();
		m_ActiveContainer.name = "Container";
		m_ActiveContainer.transform.parent = transform;


		// Reset values
		m_TeamDirections = new Vector3[MaxTeamCount];
		m_ArenaTiles = new Dictionary<Vector2Int, ArenaTile>();
		
		// Recalc centre
		m_PlayAreaCentre = Vector3.zero;

		foreach (var beacon in m_TeamAreaBeacons)
			m_PlayAreaCentre += beacon.position;
		m_PlayAreaCentre /= m_TeamAreaBeacons.Length;


		for (int i = 0; i < MaxTeamCount; ++i)
			RegenerateTeamArea(i);
	}

	private void RegenerateTeamArea(int teamIndex)
	{
		Vector3 teamCentre = m_TeamAreaBeacons[teamIndex].position;
		Vector2Int centreOffset = new Vector2Int(Mathf.FloorToInt(m_TeamAreaSize.x / 2.0f), Mathf.FloorToInt(m_TeamAreaSize.y / 2.0f));

		m_TeamDirections[teamIndex] = (m_PlayAreaCentre - teamCentre).normalized;

		for (int x = 0; x < m_TeamAreaSize.x; ++x)
			for (int z = 0; z < m_TeamAreaSize.y; ++z)
			{
				Vector2Int coord = new Vector2Int(m_TeamAreaSize.x * teamIndex + x, z);
				Vector3 position = teamCentre + new Vector3(m_TileScale.x * (x - centreOffset.x), 0, m_TileScale.y * (z - centreOffset.y));

				ArenaTile tile = Instantiate(m_TilePrefab, position, Quaternion.identity, m_ActiveContainer.transform);
				tile.InitArenaTile(coord, teamIndex);
				m_ArenaTiles.Add(coord, tile);
			}
	}

	public void ClearConsideredTiles()
	{
		foreach (var tile in AllArenaTiles)
			tile.IsBeingCosidered = false;
	}

	public IEnumerable<ArenaTile> GetTilesForTeam(int teamIndex)
	{
		return AllArenaTiles.Where((tile) => tile.TeamIndex == teamIndex);
	}

	public bool TryGetTile(Vector2Int coord, out ArenaTile tile)
	{
		return m_ArenaTiles.TryGetValue(coord, out tile);
	}
}
