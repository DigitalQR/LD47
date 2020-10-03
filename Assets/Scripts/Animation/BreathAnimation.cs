using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathAnimation : MonoBehaviour
{
	[SerializeField]
	private Transform m_FloorLocator = null;

	[SerializeField]
	private float m_CycleDuration = 1.0f;

	[SerializeField]
	private Vector3 m_MinScale = Vector3.one;
	[SerializeField]
	private Vector3 m_MaxScale = Vector3.one;

	private float m_Timer = 0.0f;
	private Vector3 m_StartPosition;
	private Vector3 m_StartScale;
	private Vector3 m_FloorOffset;

	private void Start()
	{
		m_StartPosition = transform.localPosition;
		m_StartScale = transform.localScale;
		m_FloorOffset = transform.position - m_FloorLocator.position;

		m_Timer = Random.value;
	}

	private void Update()
    {
		m_Timer += Time.deltaTime;

		float s = Mathf.Sin(m_Timer * 2.0f * Mathf.PI * m_CycleDuration);
		float breathAmount = (s + 1.0f) * 0.5f;

		transform.localScale = Vector3.Scale(m_StartScale, Vector3.Lerp(m_MinScale, m_MaxScale, breathAmount));
		transform.localPosition = m_StartPosition + Vector3.Scale(m_FloorOffset, transform.localScale) - m_FloorOffset;

	}
}
