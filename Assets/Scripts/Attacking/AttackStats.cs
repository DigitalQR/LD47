using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackStats
{
	public float MaxHealth;
	public float DamageDealt;
	public float DamageReduction;
	public float Accuracy;

	public void ClampForUse()
	{
		MaxHealth = Mathf.Min(0, MaxHealth);
		DamageDealt = Mathf.Min(0, DamageDealt);
		DamageReduction = Mathf.Clamp01(DamageReduction);
		Accuracy = Mathf.Clamp01(Accuracy);
	}

	public AttackStats Merge(AttackStats other)
	{
		AttackStats output = new AttackStats();
		output.MaxHealth += other.MaxHealth;
		output.DamageDealt += other.DamageDealt;
		output.DamageReduction += other.DamageReduction;
		output.Accuracy += other.Accuracy;
		return output;
	}
}
