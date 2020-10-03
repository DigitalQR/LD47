using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DQR.Types;

[RequireComponent(typeof(ArenaBoard))]
public class ArenaCoordinator : SingletonBehaviour<ArenaCoordinator>, IEvent_TileSelected
{
	protected override void SingletonInit()
	{
	}

	private void OnTileSelected(ArenaTile tile)
	{
		Debug.Log("TEMP " + tile.Coord.ToString());
	}
}
