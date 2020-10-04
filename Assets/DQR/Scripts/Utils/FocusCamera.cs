using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DQR
{
	public class FocusCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform m_CurrentFocus = null;

		[SerializeField]
		private Transform m_FocusAnchor = null;

		[Header("Animation")]
		[SerializeField]
		private bool m_PreferSlerp = false;

		[SerializeField]
		private float m_Snappiness = 1.0f;

		[SerializeField]
		private float m_MinTeleportDistance = 20.0f;

		[SerializeField]
		private UnityEvent m_OnTargetReached = null;

		private bool m_HasReachedTarget = false;

		public Transform FocusAnchor
		{
			get => m_FocusAnchor ? m_FocusAnchor : transform;
		}

		public Transform CurrentFocus
		{
			get => m_CurrentFocus;
			set => m_CurrentFocus = value;
		}
		
		public Vector3 CurrentFocusPosition
		{
			get => m_CurrentFocus ? m_CurrentFocus.position : FocusAnchor.position;
		}

		public bool HasReachedTarget
		{
			get => m_HasReachedTarget;
		}

		public UnityEvent OnTargetReached
		{
			get => m_OnTargetReached;
		}

		private void LateUpdate()
		{
			bool hadReached = m_HasReachedTarget;

			if (Vector3.Distance(FocusAnchor.position, CurrentFocusPosition) >= m_MinTeleportDistance)
			{
				transform.position = CurrentFocusPosition;
				m_HasReachedTarget = true;
			}
			else
			{
				// Offset all positions by anchor delta
				Vector3 anchorDelta = FocusAnchor.position - transform.position;

				Vector3 currentPosition = transform.position;
				Vector3 targetPosition = CurrentFocusPosition - anchorDelta;

				float t = Time.deltaTime * m_Snappiness;
				if (m_PreferSlerp)
					transform.position = Vector3.Slerp(currentPosition, targetPosition, t);
				else
					transform.position = Vector3.Lerp(currentPosition, targetPosition, t);

				m_HasReachedTarget = Vector3.Distance(transform.position, targetPosition) <= 0.1f;
			}

			if (m_HasReachedTarget && !hadReached)
				m_OnTargetReached.Invoke();
		}
	}
}
