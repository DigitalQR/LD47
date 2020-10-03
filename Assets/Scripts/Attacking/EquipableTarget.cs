using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;
using System.Linq;

public class EquipableTarget : MonoBehaviour
{
	[SerializeField]
	private AttackStats m_BaseStats = default;

	[SerializeField]
	private SerializableDictionary<EquipableSlot, Transform> m_AvaliableSlots = null;

	private Dictionary<EquipableSlot, EquipableItem> m_EquippedItems = new Dictionary<EquipableSlot, EquipableItem>();

	public IEnumerable<EquipableItem> EquippedItems
	{
		get => m_EquippedItems.Values;
	}

	public bool HasAttackActions
	{
		get => EquippedItems.Where((i) => i.HasAttackActions).Any();
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get => EquippedItems.SelectMany((i) => i.AttackActions);
	}

	public AttackStats BaseStats
	{
		get => m_BaseStats;
	}

	public AttackStats CurrentStats
	{
		get
		{
			AttackStats stats = m_BaseStats;
			ApplyStatChanges(ref stats);
			stats.ClampForUse();
			return stats;
		}
	}

	public Transform GetSlotTransform(EquipableSlot slot)
	{
		if (m_AvaliableSlots.TryGetValue(slot, out Transform slotTransform))
			return slotTransform;

		return null;
	}

	public bool TryEquipItem(EquipableItem item)
	{
		if (m_EquippedItems.ContainsKey(item.TargetSlot) || !m_AvaliableSlots.ContainsKey(item.TargetSlot))
			return false;

		m_EquippedItems.Add(item.TargetSlot, item);
		item.OnEquiped(this, GetSlotTransform(item.TargetSlot));
		return true;
	}

	public bool TryUnequipSlot(EquipableSlot slot, out EquipableItem item)
	{
		if (m_EquippedItems.TryGetValue(slot, out item))
		{
			m_EquippedItems.Remove(slot);
			item.OnUnequiped(this, GetSlotTransform(slot));
			return true;
		}

		return false;
	}

	private void ApplyStatChanges(ref AttackStats stats)
	{
		foreach (var pair in m_EquippedItems)
			pair.Value.ApplyStatChanges(ref stats);
	}
}
