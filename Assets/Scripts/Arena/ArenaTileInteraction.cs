using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArenaTile))]
public class ArenaTileInteraction : MonoBehaviour
{
	private static ArenaTile s_DudTile = null;

	[SerializeField]
	private GameObject m_HoverObject = null;

	[SerializeField]
	private GameObject m_DefaultObject = null;

	private ArenaTile m_Tile = null;
	private bool m_IsHovering = false;
	private bool m_JustHovered = false;
	
	private void Start()
	{
		m_Tile = GetComponent<ArenaTile>();

		if (s_DudTile == null)
		{
			var obj = new GameObject();
			obj.SetActive(false);
			obj.name = "Tile dud";
			s_DudTile = obj.AddComponent<ArenaTile>();
		}
	}

	private void Update()
	{
		if (m_IsHovering && !m_JustHovered)
		{
			EventHandler.Invoke("OnTileHover", s_DudTile);
		}

		m_IsHovering = false;
		if (m_JustHovered)
		{
			m_IsHovering = true;
			m_JustHovered = false;
		}

		if (m_Tile.IsBeingCosidered)
		{
			if (m_HoverObject != null)
				m_HoverObject.SetActive(m_IsHovering);

			if (m_DefaultObject != null)
				m_DefaultObject.SetActive(!m_IsHovering);
		}
		else
		{
			if (m_HoverObject != null)
				m_HoverObject.SetActive(false);

			if (m_DefaultObject != null)
				m_DefaultObject.SetActive(false);
		}
	}

	private void OnMouseOver()
	{
		if (m_Tile.IsBeingCosidered)
			m_JustHovered = true;

		EventHandler.Invoke("OnTileHover", m_Tile);
	}
	
	private void OnMouseUp()
	{
		if(m_Tile.IsBeingCosidered)
			m_Tile.SelectTile();
	}
}
