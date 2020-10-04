using DQR.Debug;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterType
{
	Enemies,
	Good,
	//Boss,
}

public class EncounterManager : SingletonBehaviour<EncounterManager>
{
	[SerializeField]
	private Vector3 m_SpawnOffset = default;

	[SerializeField]
	private SerializableDictionary<EncounterType, WeightedCollection<GameObject>> m_Encounters = null;
	
	[SerializeField]
	private int m_MinGoodEncounterWait = 3;

	[SerializeField]
	private int m_MaxGoodEncounterWait = 5;

	[SerializeField]
	private int m_TurnsForDifficultyIncrease = 5;

	[SerializeField]
	private float m_DifficultyIncreaseAmount = 1.0f;

	private float m_CurrentDifficulty = 0.0f;
	private int m_NextGoodEncouter = 0;

	private EncounterType m_CurrentEncounterType;
	private GameObject m_CurrentEncounter = null;
	private GameObject m_PreviousEncounter = null;
	private int m_EncounterCount = 0;
	private int m_BadEncounterCount = 0;

	protected override void SingletonInit()
	{
		QueueGoodEncounter();
		SpawnNextEncounter();
	}

	public GameObject CurrentEncounterContainer
	{
		get => m_CurrentEncounter;
	}

	public bool IsEncounterActive
	{
		get => m_CurrentEncounter != null;
	}

	public EncounterType CurrentEncounterType
	{
		get => m_CurrentEncounterType;
	}

	public int EncounterCount
	{
		get => m_EncounterCount;
	}

	public int BadEncounterCount
	{
		get => m_BadEncounterCount;
	}

	public float CurrentDifficulty
	{
		get => m_CurrentDifficulty;
	}

	private void QueueGoodEncounter()
	{
		m_NextGoodEncouter = m_EncounterCount + Mathf.RoundToInt(Random.Range(m_MinGoodEncounterWait, m_MaxGoodEncounterWait + 1) * Mathf.Max(1.0f, m_CurrentDifficulty));
	}

	public void EndCurrentEncounter()
	{
		if (m_CurrentEncounter)
		{
			if(m_CurrentEncounterType == EncounterType.Enemies)
				PopupManager.Instance.CreateHeadingPopup3D("Clear!", "Prepare and then move further into the dungeon", m_CurrentEncounter.transform.position, 2.0f);

			EventHandler.Invoke("OnEncounterEnd", m_CurrentEncounterType);

			if (m_PreviousEncounter != null)
				Destroy(m_PreviousEncounter);

			m_PreviousEncounter = m_CurrentEncounter;
			m_CurrentEncounter = null;
		}
	}

	public void SpawnNextEncounter()
	{
		Assert.Message(m_CurrentEncounter == null, "Encounter still active");

		int current = m_EncounterCount++;

		if (current == 0)
			QueueGoodEncounter();

		if (current != 0 && current % m_TurnsForDifficultyIncrease == 0)
			m_CurrentDifficulty += m_DifficultyIncreaseAmount;

		if (current >= m_NextGoodEncouter)
		{
			m_CurrentEncounterType = EncounterType.Good;
			QueueGoodEncounter();
		}
		else
		{
			m_CurrentEncounterType = EncounterType.Enemies;
			m_BadEncounterCount++;
		}

		var options = m_Encounters[m_CurrentEncounterType];
		var encounterPrefab = options.SelectRandom();

		m_CurrentEncounter = Instantiate(encounterPrefab, m_SpawnOffset * current, Quaternion.identity, transform);
		EventHandler.Invoke("OnEncounterBegin", m_CurrentEncounterType);
	}
}
