using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTEquip : MonoBehaviour
{
	public EquipableItem Item;

	public bool DoThing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (DoThing)
		{
			DoThing = false;
			EquipableItem newItem = Instantiate(Item);
			newItem.ApplyVariantion();

			foreach (var target in FindObjectsOfType<EquipableTarget>())
			{
				if (target.TryEquipItem(newItem))
					break;
			}
		}
    }
}
