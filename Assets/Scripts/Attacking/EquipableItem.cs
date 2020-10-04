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
	[SerializeField]
	private WeightedCollection<string> m_NameOptions = default;

	[Header("Visuals")]
	[SerializeField]
	private EquipableSlot m_TargetSlot = EquipableSlot.Weapon;

	[SerializeField]
	private DifficultyAdjustedAttackStats m_AdjustedStats = default;
		

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

	private string m_ItemName = null;
	private List<AttackAction> m_AttackActions = null;
	private AttackStats m_VariationStats = default;
	private bool m_HasValidVariation = false;

	public EquipableSlot TargetSlot
	{
		get => m_TargetSlot;
	}

	public string ItemName
	{
		get
		{
			if (m_ItemName == null)
				m_ItemName = m_NameOptions.SelectValue(0.0f);
			return m_ItemName;
		}
	}

	public bool HasAttackActions
	{
		get => AttackActions.Any();
	}

	public AttackStats DeltaStats
	{
		get => m_HasValidVariation ? m_VariationStats : m_AdjustedStats.BaseStats;
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get
		{
			if (m_AttackActions == null)
			{
				MoveSet moveset = m_MoveSetOptions.GetCurrent();
				if (moveset == null)
				{
					m_AttackActions = new List<AttackAction>();
				}
				else
				{
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
			}

			return m_AttackActions;
		}
	}

	public void ApplyStatChanges(ref AttackStats stats)
	{
		stats = stats.Merge(DeltaStats);
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
		m_HasValidVariation = true;
		
		m_VariationStats = m_AdjustedStats.Next();

		m_ItemName = m_NameOptions.SelectRandom();
		m_TintVariation.ApplyVariationTo(gameObject);

		foreach (var action in AttackActions)
			action.ApplyVariantion();
	}
	
	public void ShowInfoPanel()
	{
		List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();

		content.Add(new KeyValuePair<string, string>("Modifiers", ""));
		content.AddRange(AttackStats.GetPanelContent(DeltaStats, true));

		if (HasAttackActions)
		{
			content.Add(new KeyValuePair<string, string>("Attacks", ""));

			foreach (var attack in AttackActions)
			{
				content.Add(new KeyValuePair<string, string>("", attack.AttackName));
			}
		}

		InfoPanelManager.Instance.OpenPanel(ItemName, m_TargetSlot.ToString(), content);
	}
}
