using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;

public class EquipableTarget : MonoBehaviour
{
	[SerializeField]
	private SerializableDictionary<EquipableSlot, Transform> m_AvaliableSlots = null;

	private Dictionary<EquipableSlot, EquipableItem> m_EquipedItems = new Dictionary<EquipableSlot, EquipableItem>();

	public Transform GetSlotTransform(EquipableSlot slot)
	{
		if (m_AvaliableSlots.TryGetValue(slot, out Transform slotTransform))
			return slotTransform;

		return null;
	}

	public bool TryEquipItem(EquipableItem item)
	{
		if (m_EquipedItems.ContainsKey(item.TargetSlot) || !m_AvaliableSlots.ContainsKey(item.TargetSlot))
			return false;

		m_EquipedItems.Add(item.TargetSlot, item);
		item.OnEquiped(this, GetSlotTransform(item.TargetSlot));
		return true;
	}

	public bool TryUnequipSlot(EquipableSlot slot, out EquipableItem item)
	{
		if (m_EquipedItems.TryGetValue(slot, out item))
		{
			m_EquipedItems.Remove(slot);
			item.OnUnequiped(this, GetSlotTransform(slot));
			return true;
		}

		return false;
	}

	public void ApplyStatChanges(ref AttackStats stats)
	{
		foreach (var pair in m_EquipedItems)
			pair.Value.ApplyStatChanges(ref stats);
	}
}
