using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;
using System.Linq;

public class EquipableTarget : MonoBehaviour
{
	[SerializeField]
	private DifficultyAdjustedAttackStats m_AdjustedStats = default;

	[SerializeField]
	private SerializableDictionary<EquipableSlot, Transform> m_AvaliableSlots = null;

	[SerializeField]
	private AttackAction[] m_DefaultAttacks = null;

	private Dictionary<EquipableSlot, EquipableItem> m_EquippedItems = new Dictionary<EquipableSlot, EquipableItem>();
	private AttackStats m_VariationStats = default;
	private bool m_HasValidVariation = false;

	public IEnumerable<EquipableItem> EquippedItems
	{
		get => m_EquippedItems.Values;
	}

	public IEnumerable<EquipableSlot> SupportedSlots
	{
		get => m_AvaliableSlots.Keys;
	}

	public bool HasAttackActions
	{
		get => m_DefaultAttacks.Any() || EquippedItems.Where((i) => i.HasAttackActions).Any();
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get
		{
			var attacks = EquippedItems.SelectMany((i) => i.AttackActions);
			return attacks.Union(m_DefaultAttacks);
		}
	}

	public AttackStats BaseStats
	{
		get
		{
			if (!m_HasValidVariation)
			{
				m_VariationStats = m_AdjustedStats.Next();
				m_HasValidVariation = true;
			}

			return m_VariationStats;
		}
	}

	public AttackStats CurrentStats
	{
		get
		{
			AttackStats stats = BaseStats;
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

	public bool TryGetItem(EquipableSlot slot,  out EquipableItem item)
	{
		return m_EquippedItems.TryGetValue(slot, out item);
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

	public void IncreaseBaseStats(AttackStats delta)
	{
		m_VariationStats = BaseStats.Merge(delta);
	}

	public IEnumerable<KeyValuePair<string, string>> GetPanelContent(bool hideDetails)
	{
		List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();

		if (!hideDetails)
			content.AddRange(AttackStats.GetPanelContent(BaseStats, CurrentStats));
		else
			content.Add(new KeyValuePair<string, string>("Stats", "?"));

		foreach (var slot in m_AvaliableSlots.Keys)
		{
			if (m_EquippedItems.TryGetValue(slot, out EquipableItem item))
				content.Add(new KeyValuePair<string, string>(slot.ToString(), item.ItemName));
			else
				content.Add(new KeyValuePair<string, string>(slot.ToString(), "-"));
		}

		return content;
	}
}
