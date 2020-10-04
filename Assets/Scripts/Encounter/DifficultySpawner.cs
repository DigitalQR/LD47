using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyGroup<T>
{
	[System.Serializable]
	private class GroupSettings
	{
		public float MinDifficulty = 0.0f;
		public T Target = default;
	}

	[SerializeField]
	private GroupSettings[] m_Groups = null;
	
	public T GetCurrent()
	{
		GroupSettings currSettings = null;

		float currDifficulty = EncounterManager.Instance.CurrentDifficulty;

		foreach (var settings in m_Groups)
		{
			if (currDifficulty >= settings.MinDifficulty && (currSettings == null || settings.MinDifficulty > currSettings.MinDifficulty))
			{
				currSettings = settings;
			}
		}

		return currSettings != null ? currSettings.Target : default;
	}
}
