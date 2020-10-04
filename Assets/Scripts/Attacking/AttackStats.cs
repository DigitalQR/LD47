using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackStats
{
	public int MaxHealth;
	public int DamageDealt;
	public int DamageReduction;
	public float Accuracy;
	public float Speed;

	public void ClampForUse()
	{
		MaxHealth = Mathf.Max(0, MaxHealth);
		DamageDealt = Mathf.Max(0, DamageDealt);
		DamageReduction = Mathf.Max(0, DamageReduction);
		Accuracy = Mathf.Max(0.0f, Accuracy);
		Speed = Mathf.Max(0, Speed);
	}


	public AttackStats Scale(float scale)
	{
		AttackStats output = new AttackStats();
		output.MaxHealth = Mathf.RoundToInt(MaxHealth * scale);
		output.DamageDealt = Mathf.RoundToInt(DamageDealt * scale);
		output.DamageReduction = Mathf.RoundToInt(DamageReduction * scale);
		output.Accuracy = Mathf.RoundToInt(Accuracy * scale);
		output.Speed = Mathf.RoundToInt(Speed * scale);
		return output;
	}

	public AttackStats Merge(AttackStats other)
	{
		AttackStats output = new AttackStats();
		output.MaxHealth = MaxHealth + other.MaxHealth;
		output.DamageDealt = DamageDealt + other.DamageDealt;
		output.DamageReduction = DamageReduction + other.DamageReduction;
		output.Accuracy = Accuracy + other.Accuracy;
		output.Speed = Speed + other.Speed;
		return output;
	}

	public AttackStats GenerateRandomFromSelf()
	{
		AttackStats output = new AttackStats();
		output.MaxHealth = Random.Range(-MaxHealth, MaxHealth);
		output.DamageDealt = Random.Range(-DamageDealt, DamageDealt);
		output.DamageReduction = Random.Range(-DamageReduction, DamageReduction);
		output.Accuracy = Random.Range(-Accuracy, Accuracy);
		output.Speed = Random.Range(-Speed, Speed);
		return output;
	}

	public void ModifyDispatchedEvent(DamageEvent damageEvent)
	{
		damageEvent.DamageAmount += DamageDealt;
	}

	public void ModifyRecievedEvent(DamageEvent damageEvent)
	{
		damageEvent.DamageAmount = Mathf.Max(0, damageEvent.DamageAmount - DamageReduction);
	}

	private static string GetStatText(int baseStat, int fullStat)
	{
		if (fullStat == baseStat)
			return fullStat.ToString();
		else
		{
			return fullStat + " (" + baseStat + (fullStat < baseStat ? "-" : "+") + Mathf.Abs(fullStat - baseStat) + ")";
		}
	}

	private static string GetStatText(float baseStat, float fullStat)
	{
		return GetStatText(Mathf.RoundToInt(baseStat), Mathf.RoundToInt(fullStat));
	}

	public static IEnumerable<KeyValuePair<string, string>> GetPanelContent(AttackStats baseStats)
	{
		return GetPanelContent(baseStats, baseStats);
	}

	public static IEnumerable<KeyValuePair<string, string>> GetPanelContent(AttackStats baseStats, AttackStats fullStats)
	{
		return new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("Max Health", GetStatText(baseStats.MaxHealth, fullStats.MaxHealth)),
			new KeyValuePair<string, string>("Attack", GetStatText(baseStats.DamageDealt, fullStats.DamageDealt)),
			new KeyValuePair<string, string>("Defence", GetStatText(baseStats.DamageReduction, fullStats.DamageReduction)),
			new KeyValuePair<string, string>("Accuracy", GetStatText(baseStats.Accuracy * 100, fullStats.Accuracy * 100) + "%"),
			new KeyValuePair<string, string>("Speed", GetStatText(baseStats.Accuracy * 100, fullStats.Accuracy * 100)),
		};
	}
}

[System.Serializable]
public struct DifficultyAdjustedAttackStats
{
	public AttackStats BaseStats;
	public AttackStats RandomRange;

	public AttackStats Next()
	{
		float currDifficulty = EncounterManager.Instance.CurrentDifficulty;
		return BaseStats.Merge(RandomRange.Scale(currDifficulty).GenerateRandomFromSelf());
	}
}

