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
	}

	public GameObject Content
	{
		get => m_Content;
		set
		{
			if(m_Content != null)
				Assert.Format(value == null, "Setting content '{0}' for tile already containing '{1}'", value, m_Content);

			m_Content = value;

			if (m_Content != null)
				m_Content.transform.position = transform.position;

			EventHandler.Invoke("OnTileContentChanged", this);
		}
	}

	public bool HasContent
	{
		get => m_Content != null;
	}

	public void MarkAsConsidered()
	{
		m_IsBeingCosidered = true;
	}

	public void ClearConsideration()
	{
		m_IsBeingCosidered = false;
	}

	public static int CoordDistance(Vector2Int a, Vector2Int b)
	{
		Vector2Int delta = a - b;
		int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
		return distance;
	}

	public int GetCoordDistance(ArenaTile tile)
	{
		return CoordDistance(m_Coord, tile.m_Coord);
	}

	public int GetCoordDistance(Vector2Int coord)
	{
		return CoordDistance(m_Coord, coord);
	}

	public void SelectTile()
	{
		Assert.Format(m_IsBeingCosidered, "Tile '{0}' isn't being considered, but was selected", m_Coord);

		if(m_IsBeingCosidered)
			EventHandler.Invoke("OnTileSelected", this);
	}
}
