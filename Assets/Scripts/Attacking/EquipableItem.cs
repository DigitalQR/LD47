using DQR.Types;
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

	[Header("Random Variation")]
	[SerializeField]
	private TintVariationCollection m_TintVariation = default;

	private AttackAction[] m_AttackActions = null;

	private void Start()
	{
		m_AttackActions = GetComponentsInChildren<AttackAction>();
	}

	public EquipableSlot TargetSlot
	{
		get => m_TargetSlot;
	}

	public bool HasAttackActions
	{
		get => m_AttackActions != null && m_AttackActions.Length != 0;
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get => m_AttackActions;
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

	public void ApplyVariantion()
	{
		m_TintVariation.ApplyVariationTo(gameObject);
	}
}
