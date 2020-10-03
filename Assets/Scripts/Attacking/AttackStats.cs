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
	public float Speed;

	public void ClampForUse()
	{
		MaxHealth = Mathf.Min(0, MaxHealth);
		DamageDealt = Mathf.Min(0, DamageDealt);
		DamageReduction = Mathf.Clamp01(DamageReduction);
		Accuracy = Mathf.Clamp01(Accuracy);
		Speed = Mathf.Min(0, Speed);
	}

	public AttackStats Merge(AttackStats other)
	{
		AttackStats output = new AttackStats();
		output.MaxHealth += other.MaxHealth;
		output.DamageDealt += other.DamageDealt;
		output.DamageReduction += other.DamageReduction;
		output.Accuracy += other.Accuracy;
		output.Speed += other.Speed;
		return output;
	}
}
