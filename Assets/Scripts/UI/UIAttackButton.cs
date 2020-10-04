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
		var text = GetComponentInChildren<Text>();

		button.onClick.AddListener(() =>
		{
			coordinator.SetCurrentAttackAction(index);
		});

		text.text = attack.AttackName;
	}
}
