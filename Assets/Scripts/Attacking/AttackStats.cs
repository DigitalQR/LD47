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
		damageEvent.DamageAmount = Mathf.Max(1, damageEvent.DamageAmount - DamageReduction);
	}

	private static string GetStatText(int baseStat, int fullStat, bool alwaysShowSign)
	{
		string format = "";

		if (fullStat == baseStat)
			format = fullStat.ToString();
		else
		{
			format = fullStat + " (" + baseStat + (fullStat < baseStat ? "-" : "+") + Mathf.Abs(fullStat - baseStat) + ")";
		}

		if (alwaysShowSign && fullStat >= 0)
			return "+" + format;

		return format;
	}

	private static string GetStatText(float baseStat, float fullStat, bool alwaysShowSign)
	{
		return GetStatText(Mathf.RoundToInt(baseStat), Mathf.RoundToInt(fullStat), alwaysShowSign);
	}

	public static IEnumerable<KeyValuePair<string, string>> GetPanelContent(AttackStats baseStats, bool alwaysShowSign = false)
	{
		return GetPanelContent(baseStats, baseStats, alwaysShowSign);
	}

	public static IEnumerable<KeyValuePair<string, string>> GetPanelContent(AttackStats baseStats, AttackStats fullStats, bool alwaysShowSign = false)
	{
		return new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("Max Health", GetStatText(baseStats.MaxHealth, fullStats.MaxHealth, alwaysShowSign)),
			new KeyValuePair<string, string>("Attack", GetStatText(baseStats.DamageDealt, fullStats.DamageDealt, alwaysShowSign)),
			new KeyValuePair<string, string>("Defence", GetStatText(baseStats.DamageReduction, fullStats.DamageReduction, alwaysShowSign)),
			new KeyValuePair<string, string>("Accuracy", GetStatText(baseStats.Accuracy * 100, fullStats.Accuracy * 100, alwaysShowSign) + "%"),
			new KeyValuePair<string, string>("Speed", GetStatText(baseStats.Accuracy * 100, fullStats.Accuracy * 100, alwaysShowSign)),
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

