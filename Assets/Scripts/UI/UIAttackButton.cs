using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIAttackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private AttackAction m_Attack = null;

	public void PopulateContent(PlayerCoordinator coordinator, AttackAction attack, int index)
	{
		m_Attack = attack;

		var button = GetComponent<Button>();
		var text = GetComponentInChildren<Text>();

		button.onClick.AddListener(() =>
		{
			coordinator.SetCurrentAttackAction(index);
		});
		
		text.text = attack.AttackName;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_Attack.ShowInfoPanel();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		InfoPanelManager.Instance.Close();
	}
}
