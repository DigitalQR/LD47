using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTile : MonoBehaviour
{
	private Vector2Int m_Coord;
	private int m_TeamIndex;
	private bool m_IsBeingCosidered;

	private GameObject m_Content = null;

	internal void InitArenaTile(Vector2Int coord, int teamIndex)
	{
		m_Coord = coord;
		m_TeamIndex = teamIndex;
		m_IsBeingCosidered = false;
	}

	public Vector2Int Coord
	{
		get => m_Coord;
	}
	
	public int TeamIndex
	{
		get => m_TeamIndex;
	}

	public bool IsBeingCosidered
	{
		get => m_IsBeingCosidered;
		set => m_IsBeingCosidered = value;
	}

	public GameObject Content
	{
		get => m_Content;
		set
		{
			Assert.Format(m_Content == null, "Setting content for tile already containing '{0}'", m_Content);
			m_Content = value;
		}
	}

	public void SelectTile()
	{
		Assert.Format(m_IsBeingCosidered, "Tile '{0}' isn't being considered, but was selected", m_Coord);

		if(m_IsBeingCosidered)
			ArenaCoordinator.Instance.BroadcastMessage("OnTileSelected", this);
	}
}
