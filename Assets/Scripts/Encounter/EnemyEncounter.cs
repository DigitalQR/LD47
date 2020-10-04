using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;

[RequireComponent(typeof(AICoordinator))]
public class EnemyEncounter : MonoBehaviour
{
	[System.Serializable]
	private class EncounterSettings
	{
		public float MinDifficulty = 0.0f;
		public TeamTurnCoordinator.PawnSpawnSettings PawnSettings = null;
	}

	[SerializeField]
	private EncounterSettings[] m_DifficultySpawns = null;

	private AICoordinator m_Coordinator = null;

	private void Start()
    {
		// Spawn options
		m_Coordinator = GetComponent<AICoordinator>();

		EncounterSettings settings = SelectSettings();

		if (settings != null)
			m_Coordinator.SpawnPawns(settings.PawnSettings);
    }

	private EncounterSettings SelectSettings()
	{
		EncounterSettings currSettings = null;
	
		float currDifficulty = EncounterManager.Instance.CurrentDifficulty;

		foreach (var settings in m_DifficultySpawns)
		{
			if (currDifficulty >= settings.MinDifficulty && (currSettings == null || settings.MinDifficulty > currSettings.MinDifficulty))
			{
				currSettings = settings;
			}
		}

		return currSettings;
	}

	private void Update()
	{
		if (m_Coordinator == null || (m_Coordinator.OwnedPawnCount == 0 && m_Coordinator.PreviousKnownState == TurnState.Movement))
		{
			EncounterManager.Instance.EndCurrentEncounter();
			gameObject.SetActive(false);
		}
	}
}
