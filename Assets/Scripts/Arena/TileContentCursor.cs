﻿using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContentCursor : MonoBehaviour
{
	[SerializeField]
	private Transform m_CursorTarget = null;

	[SerializeField]
	private Transform m_ContentTarget = null;

	[SerializeField]
	private LayerMask m_CursorPositionMask = new LayerMask();
	
	private GameObject m_Content = null;

	public bool HasContent
	{
		get => m_Content != null;
	}

	private void LateUpdate()
	{
		if (m_ContentTarget != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, m_CursorPositionMask))
				m_CursorTarget.position = hit.point;
		}

		// Animation
		if(m_Content != null && m_ContentTarget != null)
		{
			m_Content.transform.position = m_ContentTarget.transform.position;
		}
	}

	private void Event_OnTileSelected(ArenaTile tile)
	{
		// Swap contents
		var oldContent = m_Content;
		var newContent = tile.Content;
		
		// Make sure tile.Content is set 2nd as it will fire of a event
		m_Content = newContent;
		tile.Content = oldContent;
		EventHandler.Invoke("OnTileCursorContentChanged", this);
	}

	public void SetEnabled(bool state)
	{
		if (enabled != state)
		{
			enabled = state;

			if (state)
				Assert.Format(HasContent, "Still contain content '{0}' when movement manager is being disabled", m_Content);
		}
	}
}