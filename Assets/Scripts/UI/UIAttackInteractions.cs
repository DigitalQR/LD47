using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttackInteractions : MonoBehaviour
{
	[SerializeField]
	private UIAttackButton m_DefaultButton = null;

	private bool m_IsOpen = false;

	private void Start()
	{
		m_DefaultButton.gameObject.SetActive(false);
	}

	public void OpenMenu(PlayerCoordinator coordinator)
	{
		if (!m_IsOpen)
		{
			DestroyExistingOptions();

			// Build options
			var attackOptions = coordinator.CurrentAttackActions();

			for (int i = 0; i < attackOptions.Length; ++i)
			{
				UIAttackButton newButton = Instantiate(m_DefaultButton, transform);
				newButton.PopulateContent(coordinator, attackOptions[i], i);
				newButton.gameObject.SetActive(true);
			}

			m_IsOpen = true;
		}
	}

	public void CloseMenu()
	{
		if (m_IsOpen)
		{
			DestroyExistingOptions();
			m_IsOpen = false;
		}
	}

	private void DestroyExistingOptions()
	{
		foreach (var button in GetComponentsInChildren<UIAttackButton>())
		{
			if (button != m_DefaultButton)
				Destroy(button.gameObject);
		}
	}
}
