using DQR.Debug;
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

	private bool m_IsPaused = false;

	public bool HasContent
	{
		get => m_Content != null;
	}
	
	public GameObject Content
	{
		get => m_Content;
		set
		{
			if (m_Content != null)
				Assert.Format(value == null, "Setting content '{0}' for cursor already containing '{1}'", value, m_Content);

			m_Content = value;

			if (m_Content != null)
				m_Content.transform.position = transform.position;

			EventHandler.Invoke("OnTileCursorContentChanged", this);
		}
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
		if (!m_IsPaused) 
		{           
			// Swap contents
			var oldContent = m_Content;
			var newContent = tile.Content;

			m_Content = newContent;
			tile.Content = oldContent;
			EventHandler.Invoke("OnTileCursorContentChanged", this);
		}
	}

	public void SetPaused(bool paused)
	{
		if (m_IsPaused != paused)
		{
			m_IsPaused = paused;

			if (paused)
				Assert.Format(!HasContent, "Still contain content '{0}' when movement manager is being disabled", m_Content);
		}
	}
}
