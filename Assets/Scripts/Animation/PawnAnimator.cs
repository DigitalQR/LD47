using DQR.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
[RequireComponent(typeof(PawnHealth))]
public class PawnAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject m_VisualsRoot = null;

	[Header("Movement")]
	[SerializeField]
	private float m_WalkSpeed = 1.0f;

	[SerializeField]
	private float m_TooCloseDistance = 0.1f;

	[Header("Health")]
	[SerializeField]
	private GameObject m_HealthBarRoot = null;

	[SerializeField]
	private GameObject m_HealthBarScaled = null;
	
	[SerializeField]
	private float m_HealthAnimSpeed = 1.0f;

	private Pawn m_Pawn;
	private PawnHealth m_Health;

	private bool m_InWalkAnimation = false;
	private Vector3 m_WalkTarget;
	private float m_AnimationSpeedScale;

	private float m_AnimHealth = 1.0f;
	private bool m_HealthDefaultShow = false;

	private void Start()
	{
		m_Pawn = GetComponent<Pawn>();
		m_Health = GetComponent<PawnHealth>();
	}

	private void Update()
	{
		if (m_InWalkAnimation)
			UpdateWalkAnimation();

		if (ShouldHealthAnimate)
		{
			m_HealthBarRoot.SetActive(true);
			UpdateHealthAnimation();
		}
		else
		{
			m_HealthBarRoot.SetActive(m_HealthDefaultShow);
		}
	}
	
	private void Event_OnTileHover(ArenaTile tile)
	{
		m_HealthDefaultShow = (m_Pawn.CurrentTile == tile);
	}

	private void UpdateWalkAnimation()
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

		Vector3 step = Vector3.ClampMagnitude(toTarget.normalized * Time.deltaTime * m_WalkSpeed * m_AnimationSpeedScale, distance);
		transform.position += step;
	}

	private void UpdateHealthAnimation()
	{
		float targetHealth = m_Health.NormalizedHealth;

		float animStep = m_HealthAnimSpeed * Time.deltaTime * Mathf.Sign(targetHealth - m_AnimHealth);
		m_AnimHealth += animStep;

		if (Mathf.Sign(animStep) > 0)
			m_AnimHealth = Mathf.Min(m_AnimHealth, targetHealth);
		else
			m_AnimHealth = Mathf.Max(m_AnimHealth, targetHealth);
		
		Vector3 scale = m_HealthBarScaled.transform.localScale;
		scale.x = m_AnimHealth;
		m_HealthBarScaled.transform.localScale = scale;	
	}

	public void StartWalkAnimation(Vector3 target, float speedScale = 1.0f)
	{
		m_InWalkAnimation = true;
		m_WalkTarget = target;
		m_AnimationSpeedScale = speedScale;
	}

	public void SetFacingDirection(Vector2Int facingDir)
	{
		if (m_VisualsRoot)
			m_VisualsRoot.transform.localScale = new Vector3(facingDir.x, 1, 1);
	}

	private bool ShouldHealthAnimate
	{
		get => m_Health && m_AnimHealth != m_Health.NormalizedHealth;
	}

	public bool InBlockingAnimating
	{
		get => m_InWalkAnimation || ShouldHealthAnimate;
	}
}
