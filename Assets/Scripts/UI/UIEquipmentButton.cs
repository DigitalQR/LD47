using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Text m_Text = null;
	
	[SerializeField]
	private Button m_SlotSelected = null;

	private EquipableItem m_Item = null;

	public void SetupFor(EquipableItem item, UIInventory inv)
	{
		m_Item = item;
		m_Text.text = item.ItemName;
		
		m_SlotSelected.onClick.AddListener(() =>
		{
			inv.OnEquipmentSelected(item);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_Item.ShowInfoPanel();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		InfoPanelManager.Instance.Close();
	}
}