using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EquipableTarget))]
public class PawnHealth : MonoBehaviour
{
	private EquipableTarget m_Equipment = null;

	private int m_CurrentHealth = 1;
	private bool m_IsDead = false;

	private void Start()
	{
		m_Equipment = GetComponent<EquipableTarget>();
		m_CurrentHealth = m_Equipment.CurrentStats.MaxHealth;
	}

	public int CurrentHealth
	{
		get => m_CurrentHealth;
	}

	public int MaxHealth
	{
		get => m_Equipment.CurrentStats.MaxHealth;
	}

	public float NormalizedHealth
	{
		get => MaxHealth == 0 ? 0.0f : Mathf.Clamp01((float)CurrentHealth / (float)MaxHealth);
	}

	public void ApplyDamage(DamageEvent damageEvent)
	{
		if (m_IsDead)
			return;
		
		m_CurrentHealth -= damageEvent.DamageAmount;
		if (m_CurrentHealth <= 0)
		{
			m_CurrentHealth = 0;
			m_IsDead = true;
		}
			
		EventHandler.Invoke("OnPawnDamaged", damageEvent);

		if (m_IsDead)
			EventHandler.Invoke("OnPawnKilled", GetComponent<Pawn>());
	}

	public void ApplyHeal(int amount)
	{
		m_CurrentHealth = Mathf.Min(m_CurrentHealth + amount, MaxHealth);
	}
}
