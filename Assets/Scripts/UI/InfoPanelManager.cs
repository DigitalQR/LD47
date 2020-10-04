using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;
using UnityEngine.UI;
using System.Linq;

public class InfoPanelManager : SingletonBehaviour<InfoPanelManager>
{
	[SerializeField]
	private Text m_HeadingText = null;

	[SerializeField]
	private Text m_SubheadingText = null;

	[SerializeField]
	private GameObject[] m_DontKillList = null;

	[SerializeField]
	private InfoPanelRow m_RowTemplate = null;

	protected override void SingletonInit()
	{
		m_RowTemplate.gameObject.SetActive(false);
		Close();
	}

	private void Clear()
	{
		for (int i = 0; i < transform.childCount; ++i)
		{
			var curr = transform.GetChild(i).gameObject;
			if (curr != m_RowTemplate.gameObject && !m_DontKillList.Contains(curr))
				Destroy(curr);
		}
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void OpenPanel(string header, string subheading, IEnumerable<KeyValuePair<string, string>> content)
	{
		Clear();

		m_HeadingText.text = header;
		m_SubheadingText.text = subheading;

		foreach (var row in content)
		{
			var rowObj = Instantiate(m_RowTemplate, transform);
			rowObj.SetText(row.Key, row.Value);
			rowObj.gameObject.SetActive(true);
		}

		gameObject.SetActive(true);
	}
}
