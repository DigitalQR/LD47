using DQR.Debug;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventHandler : SingletonBehaviour<EventHandler>
{
#if DQR_ASSERTS
	private class KnownEvent
	{
		public string Name;
		public System.Type Param;

		public KnownEvent(string name, System.Type param = null)
		{
			Name = name;
			Param = param;
		}

		public bool ParamsMatch(object param)
		{
			if (Param == null)
				return param == null;

			if (param == null)
				return false;

			return Param.IsAssignableFrom(param.GetType());
		}
	}

	private static KnownEvent[] s_KnownEvents = new[]
	{
		new KnownEvent("OnTileSelected", typeof(ArenaTile)),
		new KnownEvent("OnTileHover", typeof(ArenaTile)),
		new KnownEvent("OnTileContentChanged", typeof(ArenaTile)),

		new KnownEvent("OnTileCursorContentChanged", typeof(TileContentCursor)),

		new KnownEvent("OnTurnStateChange", typeof(TurnState)),
		new KnownEvent("OnCoordinatorTurnBegin", typeof(TurnCoordinator)),
		new KnownEvent("OnCoordinatorTurnEnd", typeof(TurnCoordinator)),
		new KnownEvent("OnTeamCoordinatorDefeat", typeof(TeamTurnCoordinator)),

		new KnownEvent("OnPawnKilled", typeof(Pawn)),
		new KnownEvent("OnPawnAttacked", typeof(DamageEvent)),
		new KnownEvent("OnPawnDamaged", typeof(DamageEvent)),
	};
#endif
	
	private void InvokeInternal(string eventName, object param)
	{
#if DQR_ASSERTS
		KnownEvent foundEvent = s_KnownEvents.Where((e) => e.Name == eventName).FirstOrDefault();
		Assert.Format(foundEvent != null && foundEvent.ParamsMatch(param), "Invalid event match found for '{0}'", eventName);
#endif
		BroadcastMessage("Event_" + eventName, param, SendMessageOptions.DontRequireReceiver);
	}

	public static void Invoke(string eventName, object param = null)
	{
		Assert.Condition(IsValid);
		Instance.InvokeInternal(eventName, param);
	}
}
