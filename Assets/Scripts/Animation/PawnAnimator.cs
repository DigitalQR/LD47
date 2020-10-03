using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject m_VisualsRoot = null;

	[Header("Movement")]
	[SerializeField]
	private float m_WalkSpeed = 1.0f;

	[SerializeField]
	private float m_TooCloseDistance = 0.1f;

	private Pawn m_Pawn;

	private bool m_InWalkAnimation = false;
	private Vector3 m_WalkTarget;

	private void Start()
	{
		m_Pawn = GetComponent<Pawn>();
	}

	private void Update()
	{
		if (m_InWalkAnimation)
		{
			Vector3 toTarget = m_WalkTarget - transform.position;
			float distance = toTarget.magnitude;

			// End animation
			if (distance < m_TooCloseDistance)
			{
				transform.position = m_WalkTarget;
				m_WalkTarget = Vector3.zero;
				m_InWalkAnimation = false;
				return;
			}

			Vector3 step = Vector3.ClampMagnitude(toTarget.normalized * Time.deltaTime * m_WalkSpeed, distance);
			transform.position += step;
		}
	}

	public void StartWalkAnimation(Vector3 target)
	{
		m_InWalkAnimation = true;
		m_WalkTarget = target;
	}

	public void SetFacingDirection(Vector2Int facingDir)
	{
		if (m_VisualsRoot)
			m_VisualsRoot.transform.localScale = new Vector3(facingDir.x, 1, 1);
	}

	public bool InBlockingAnimating
	{
		get => m_InWalkAnimation;
	}
}
