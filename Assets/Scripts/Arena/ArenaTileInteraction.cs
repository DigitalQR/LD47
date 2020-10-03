﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArenaTile))]
public class ArenaTileInteraction : MonoBehaviour
{
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
	}

	private void Update()
	{
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
	}

	private void OnMouseUp()
	{
		if(m_Tile.IsBeingCosidered)
			m_Tile.SelectTile();
	}
}
