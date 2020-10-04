using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	[Header("Visuals")]
	[SerializeField]
	private EquipableSlot m_TargetSlot = EquipableSlot.Weapon;

	[SerializeField]
	private AttackStats m_StatsDelta = default;

	[SerializeField]
	private GameObject m_VisualsRoot = null;
			
	[SerializeField]
	private TintVariationCollection m_TintVariation = default;

	[System.Serializable]
	private class MoveSet
	{
		public int MinCount = 0;
		public int MaxCount = 0;
		public WeightedCollection<AttackAction> Actions = null;
	}

	[Header("Difficulty Scaling")]
	[SerializeField]
	private DifficultyGroup<MoveSet> m_MoveSetOptions = null;

	private List<AttackAction> m_AttackActions = null;
	
	public EquipableSlot TargetSlot
	{
		get => m_TargetSlot;
	}

	public bool HasAttackActions
	{
		get => AttackActions.Any();
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get
		{
			if (m_AttackActions == null)
			{
				MoveSet moveset = m_MoveSetOptions.GetCurrent();
				HashSet<AttackAction> actionsToSpawn = new HashSet<AttackAction>();
				int count = Random.Range(moveset.MinCount, moveset.MaxCount + 1);

				for (int i = 0; i < count; ++i)
					actionsToSpawn.Add(moveset.Actions.SelectRandom());

				m_AttackActions = new List<AttackAction>();
				foreach (var action in actionsToSpawn)
				{
					AttackAction newAction = Instantiate(action, transform);
					m_AttackActions.Add(newAction);
				}
			}

			return m_AttackActions;
		}
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

		foreach (var action in AttackActions)
			action.ApplyVariantion();
	}
}
