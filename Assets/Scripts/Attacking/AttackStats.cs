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

	public void ModifyDispatchedEvent(DamageEvent damageEvent)
	{

	}

	public void ModifyRecievedEvent(DamageEvent damageEvent)
	{

	}
}
