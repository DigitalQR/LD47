using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCoordinator))]
public class PlayerInventory : MonoBehaviour
{
	[SerializeField]
	private float m_StealItemChance = 0.1f;

	[SerializeField]
	private List<EquipableItem> m_Equipment = new List<EquipableItem>();

	private PlayerCoordinator m_Coordinator = null;
	private int m_ChangeTag = 0;

	private void Start()
	{
		m_Coordinator = GetComponent<PlayerCoordinator>();
	}

	public IEnumerable<EquipableItem> AllEquipment
	{
		get => m_Equipment;
	}

	public int ChangeTag
	{
		get => m_ChangeTag;
	}

	public void AddItem(EquipableItem item)
	{
		m_Equipment.Add(item);
		item.gameObject.SetActive(false);
		++m_ChangeTag;
	}

	public void RemoveItem(EquipableItem item)
	{
		m_Equipment.Remove(item);
		item.gameObject.SetActive(true);
		++m_ChangeTag;
	}

	public void EquipTo(Pawn pawn, EquipableItem item)
	{
		UnequipSlot(pawn, item.TargetSlot);

		if (pawn.Equipment.TryEquipItem(item))
			RemoveItem(item);
	}

	public void UnequipSlot(Pawn pawn, EquipableSlot slot)
	{
		if (pawn.Equipment.TryUnequipSlot(slot, out EquipableItem oldItem))
			AddItem(oldItem);
	}

	private void Event_OnPawnKilled(Pawn pawn)
	{
		if (pawn.TeamIndex == m_Coordinator.TeamIndex)
		{
			foreach (var slot in pawn.Equipment.SupportedSlots)
			{
				if (pawn.Equipment.TryUnequipSlot(slot, out EquipableItem item))
				{
					AddItem(item);
				}

			}
		}
		else
		{
			foreach (var slot in pawn.Equipment.SupportedSlots)
			{
				if (pawn.Equipment.TryUnequipSlot(slot, out EquipableItem item))
				{
					if (Random.value <= m_StealItemChance)
					{
						PopupManager.Instance.CreatePopup3D("Steal " + item.ItemName, pawn.transform.position, 1.0f, Color.green, FontStyle.BoldAndItalic);
						AddItem(item);
					}
					else
						Destroy(item.gameObject);
				}
			}
		}
	}
}
