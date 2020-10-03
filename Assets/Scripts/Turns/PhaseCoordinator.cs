using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase
{
	Inactive,
	Movement,
	Attacking
}

[RequireComponent(typeof(MovementManager))]
public class PhaseCoordinator : SingletonBehaviour<PhaseCoordinator>
{
	private MovementManager m_MovementManager;
	private TurnPhase m_CurrentPhase = TurnPhase.Inactive;
	
	protected override void SingletonInit()
	{
		m_MovementManager = GetComponent<MovementManager>();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void NextPhase()
	{
		switch (m_CurrentPhase)
		{
			case TurnPhase.Movement:
				m_CurrentPhase = TurnPhase.Attacking;
				break;

			case TurnPhase.Inactive:
			case TurnPhase.Attacking:
				m_CurrentPhase = TurnPhase.Movement;
				break;
		}
		
		EventHandler.Invoke("OnTurnPhaseChange", m_CurrentPhase);

		m_MovementManager.SetEnabled(m_CurrentPhase == TurnPhase.Movement);
	}
}
