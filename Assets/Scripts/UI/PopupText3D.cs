using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupText3D : MonoBehaviour
{
	[SerializeField]
	private TextMesh m_PrimaryText = null;

	[SerializeField]
	private TextMesh m_ShadowText = null;

	[SerializeField]
	private float m_Lifetime = 1.0f;

	[SerializeField]
	private Vector3 m_DriftStep = new Vector3(0.0f, 0.1f, 0.0f);

	public void ExtendLifetime(float amount)
	{
		m_Lifetime += amount;
	}

	public PopupText3D Spawn(string text, Vector3 location, float fontScale, Color colour, FontStyle style)
	{
		PopupText3D popup = Instantiate(this, location, Quaternion.identity);
		popup.gameObject.SetActive(true);
		
		popup.m_PrimaryText.text = text;
		popup.m_ShadowText.text = text;

		popup.m_PrimaryText.fontSize = Mathf.CeilToInt(popup.m_PrimaryText.fontSize * fontScale);
		popup.m_ShadowText.fontSize = Mathf.CeilToInt(popup.m_ShadowText.fontSize * fontScale);

		popup.m_PrimaryText.color = colour;
		popup.m_PrimaryText.fontStyle = style;
		popup.m_ShadowText.fontStyle = style;

		return popup;
	}

	public void Update()
	{
		m_Lifetime -= Time.deltaTime;
		transform.position += m_DriftStep * Time.deltaTime;
		
		if (m_Lifetime <= 0)
			Destroy(gameObject);
	}
}
