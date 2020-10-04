using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPawnEquipment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Text m_KeyText = null;

	[SerializeField]
	private Text m_ValueText = null;

	[SerializeField]
	private Button m_SlotSelected = null;

	private EquipableItem m_Item = null;

	public void SetupFor(Pawn pawn, EquipableSlot slot, UIInventory inv)
	{
		m_KeyText.text = slot.ToString();

		if (pawn.Equipment.TryGetItem(slot, out m_Item))
		{
			m_ValueText.text = m_Item.ItemName;
		}
		else
		{
			m_ValueText.text = "-";
		}
		
		m_SlotSelected.onClick.AddListener(() =>
		{
			inv.OnPawnEquipmentSelected(pawn, slot);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(m_Item)
			m_Item.ShowInfoPanel();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		InfoPanelManager.Instance.Close();
	}
}
