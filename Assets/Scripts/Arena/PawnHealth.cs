﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pawn))]
[RequireComponent(typeof(EquipableTarget))]
public class PawnHealth : MonoBehaviour
{
	private Pawn m_Pawn = null;
	private EquipableTarget m_Equipment = null;

	private int m_CurrentHealth = 1;
	private bool m_IsDead = false;

	private void Start()
	{
		m_Pawn = GetComponent<Pawn>();
		m_Equipment = GetComponent<EquipableTarget>();
		m_CurrentHealth = m_Equipment.CurrentStats.MaxHealth;
	}

	private void Update()
	{
		if (IsDead && !m_Pawn.InBlockingAnimating)
			Destroy(gameObject);
	}

	public int CurrentHealth
	{
		get => m_CurrentHealth;
	}

	public bool IsDead
	{
		get => m_IsDead;
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
		if (m_IsDead)
			return;

		m_CurrentHealth = Mathf.Min(m_CurrentHealth + amount, MaxHealth);
	}
}
