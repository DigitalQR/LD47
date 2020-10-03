using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	[SerializeField]
	private Transform m_Target = null;
		
    private void LateUpdate()
    {
		if (m_Target)
		{
			transform.position = m_Target.position;
			transform.rotation = m_Target.rotation;
			transform.localScale = new Vector3(Mathf.Sign(m_Target.lossyScale.x), Mathf.Sign(m_Target.lossyScale.y), Mathf.Sign(m_Target.lossyScale.z));
		}
    }
}
