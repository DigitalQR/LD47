using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This shouldn't be used, this is just for my sanity to keep track
public interface IEventsList
{
	void OnTileSelected(ArenaTile tile);
	void OnTileContentChanged(ArenaTile tile);

	void OnTurnPhaseChange(TurnPhase phase);
}
