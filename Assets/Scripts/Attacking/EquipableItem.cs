using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipableSlot
{
	Weapon,
	Hat,
	Chest,
	Cape,
	Necklass
}

public class EquipableItem : MonoBehaviour
{
	[SerializeField]
	private EquipableSlot m_TargetSlot = EquipableSlot.Weapon;

	[SerializeField]
	private AttackStats m_StatsDelta = default;

	[SerializeField]
	private GameObject m_VisualsRoot = null;

	public EquipableSlot TargetSlot
	{
		get => m_TargetSlot;
	}

	public void ApplyStatChanges(ref AttackStats stats)
	{
		stats = stats.Merge(m_StatsDelta);
	}

	public void OnEquiped(EquipableTarget target, Transform slot)
	{
		transform.parent = target.transform;

		if (m_VisualsRoot)
		{
			if (slot != null)
			{
				m_VisualsRoot.transform.parent = slot;
				m_VisualsRoot.transform.localPosition = Vector3.zero;
				m_VisualsRoot.transform.localScale = Vector3.one;
				m_VisualsRoot.transform.localRotation = Quaternion.identity;
				m_VisualsRoot.SetActive(true);
			}
			else
			{
				m_VisualsRoot.transform.parent = transform;
				m_VisualsRoot.SetActive(false);
			}
		}

	}

	public void OnUnequiped(EquipableTarget target, Transform slot)
	{
		transform.parent = null;

		if (m_VisualsRoot)
		{
			m_VisualsRoot.transform.parent = transform;
			m_VisualsRoot.transform.localPosition = Vector3.zero;
			m_VisualsRoot.transform.localScale = Vector3.one;
			m_VisualsRoot.transform.localRotation = Quaternion.identity;
			m_VisualsRoot.SetActive(true);
		}
	}
}
