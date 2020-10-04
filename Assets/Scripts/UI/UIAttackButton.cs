using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIAttackButton : MonoBehaviour
{
	public void PopulateContent(PlayerCoordinator coordinator, AttackAction attack, int index)
	{
		var button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			coordinator.SetCurrentAttackAction(index);
		});
	}
}
