using DQR;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAllyEncounter : MonoBehaviour
{
	[SerializeField]
	private WeightedCollection<Pawn> m_Pawns = null;

	private Pawn m_NewPawn = null;

	private void Start()
	{
		PlayerCoordinator player = FindObjectOfType<PlayerCoordinator>();
		if (player)
		{
			m_NewPawn = player.InstantiatePawn(m_Pawns.SelectRandom());
			m_NewPawn.transform.position = transform.position;
		}
	}

	private void Event_OnEncounterInCameraView(FocusCamera camera)
	{
		if (m_NewPawn)
		{
			PopupManager.Instance.CreatePopup3D("New Ally '" + m_NewPawn.PawnName + "'", transform.position, 1.0f, Color.green, FontStyle.Normal);
			m_NewPawn.MoveToTile(m_NewPawn.CurrentTile, true);
		}
		else
		{
			PopupManager.Instance.CreatePopup3D("No new ally today :(", transform.position, 1.0f, Color.yellow, FontStyle.Normal);
		}

		gameObject.SetActive(false);
		EncounterManager.Instance.EndCurrentEncounter();
	}
}
