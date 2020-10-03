using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	[Header("Random Variation")]
	[SerializeField]
	private TintVariationCollection m_TintVariation = default;

	private PawnAnimator m_Animator = null;

	private ArenaTile m_CurrentTile = null;

	private void Start()
	{
		m_Animator = GetComponent<PawnAnimator>();
	}

	private void Event_OnTileContentChanged(ArenaTile changeTile)
	{
		if (m_CurrentTile)
		{
			// This item has been moved
			if (changeTile == m_CurrentTile && changeTile.Content != gameObject)
				m_CurrentTile = null;
		}

		if (changeTile.Content == gameObject)
			m_CurrentTile = changeTile;
	}

	public void MoveToTile(ArenaTile target, bool shouldAnimate)
	{
		Vector3 startPosition = transform.position;
		target.Content = gameObject;

		if (shouldAnimate && m_Animator != null)
		{
			transform.position = startPosition;
			m_Animator.StartWalkAnimation(target.transform.position);
		}
	}

	public void SetFacingDirection(Vector2Int facingDir)
	{
		// Gets called really early, so make sure grabbed the component
		if (!m_Animator)
			m_Animator = GetComponent<PawnAnimator>();

		if (m_Animator)
			m_Animator.SetFacingDirection(facingDir);
	}

	public ArenaTile CurrentTile
	{
		get => m_CurrentTile;
	}

	public Vector2Int CurrentCoords
	{
		get
		{
			Assert.Message(m_CurrentTile, "Coords aren't known");
			return m_CurrentTile.Coord;
		}
	}

	public void ApplyRandomVariantion()
	{
		m_TintVariation.ApplyVariationTo(gameObject);
	}

	public bool InBlockingAnimating
	{
		get => m_Animator && m_Animator.InBlockingAnimating;
	}
}
