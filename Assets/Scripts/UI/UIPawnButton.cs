using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIPawnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Text m_NameText = null;

	private Pawn m_Pawn;

	public void SetupFor(Pawn pawn, UIInventory inv)
	{
		m_Pawn = pawn;
		m_NameText.text = pawn.PawnName;

		var button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			inv.OnPawnSelected(pawn);
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_Pawn.ShowInfoPanel();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		InfoPanelManager.Instance.Close();
	}
}
