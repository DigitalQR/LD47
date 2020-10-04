using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelRow : MonoBehaviour
{
	[SerializeField]
	private Text m_KeyText = null;

	[SerializeField]
	private Text m_ValueText = null;

	public void SetText(string key, string value)
	{
		m_KeyText.text = key;
		m_ValueText.text = value;
	}
}
