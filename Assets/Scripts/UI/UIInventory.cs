using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_DoNotDestroyList = null;

	[SerializeField]
	private GameObject m_PartyRoot = null;

	[SerializeField]
	private GameObject m_EquipmentRoot = null;

	[SerializeField]
	private GameObject m_ItemsRoot = null;


	[SerializeField]
	private UIPawnButton m_PawnHeader = null;

	[SerializeField]
	private UIPawnEquipment m_PawnEquipmentSlot = null;

	[SerializeField]
	private UIEquipmentButton m_EquipmentSlot = null;

	private PlayerCoordinator m_CurrentCoordinator = null;
	private PlayerInventory m_Inventory = null;
	private int m_ChangeTag = 0;

	private EquipableItem m_SelectedEquipment = null;

	private void Start()
    {
		gameObject.SetActive(false);

		m_PawnHeader.gameObject.SetActive(false);
		m_PawnEquipmentSlot.gameObject.SetActive(false);
		m_EquipmentSlot.gameObject.SetActive(false);

	}

	public void ToggleOpen(PlayerCoordinator coordinator)
	{
		if (m_CurrentCoordinator == coordinator)
			Close();
		else
			Open(coordinator);
	}

	public void Open(PlayerCoordinator coordinator)
	{
		if (m_CurrentCoordinator != coordinator)
		{
			Clear();
			m_CurrentCoordinator = coordinator;
			m_Inventory = m_CurrentCoordinator.GetComponent<PlayerInventory>();

			BuildMenus();
			gameObject.SetActive(true);
		}
	}

	public void Close()
	{
		if (m_CurrentCoordinator != null)
		{
			Clear();
			gameObject.SetActive(false);

			m_CurrentCoordinator = null;
			m_Inventory = null;
		}
	}

	private void Update()
	{
		if (m_Inventory)
		{
			if (m_Inventory.ChangeTag != m_ChangeTag)
			{
				m_ChangeTag = m_Inventory.ChangeTag;
				Clear();
				BuildMenus();
			}
		}
	}

	public void OnPawnSelected(Pawn pawn)
	{
		if (m_SelectedEquipment != null)
		{
			m_Inventory.EquipTo(pawn, m_SelectedEquipment);
			m_SelectedEquipment = null;
		}
	}

	public void OnPawnEquipmentSelected(Pawn pawn, EquipableSlot slot)
	{
		if (m_SelectedEquipment != null)
		{
			m_Inventory.EquipTo(pawn, m_SelectedEquipment);
			m_SelectedEquipment = null;
		}
		else
		{
			m_Inventory.UnequipSlot(pawn, slot);
		}
	}

	public void OnEquipmentSelected(EquipableItem item)
	{
		m_SelectedEquipment = item;
	}

	private void BuildMenus()
	{
		// Build for all pawns
		foreach (var pawn in m_CurrentCoordinator.OwnedPawns)
		{
			var header = Instantiate(m_PawnHeader, m_PartyRoot.transform);
			header.SetupFor(pawn, this);
			header.gameObject.SetActive(true);

			foreach (var slot in pawn.Equipment.SupportedSlots)
			{
				var slotUI = Instantiate(m_PawnEquipmentSlot, m_PartyRoot.transform);
				slotUI.SetupFor(pawn, slot, this);
				slotUI.gameObject.SetActive(true);
			}
		}

		//Build spare equipment
		foreach (var item in m_Inventory.AllEquipment)
		{
			var slotUI = Instantiate(m_EquipmentSlot, m_EquipmentRoot.transform);
			slotUI.SetupFor(item, this);
			slotUI.gameObject.SetActive(true);
		}
	}

	private void Clear()
	{
		m_SelectedEquipment = null;

		for (int i = 0; i < m_PartyRoot.transform.childCount; ++i)
		{
			var cur = m_PartyRoot.transform.GetChild(i).gameObject;

			if (!m_DoNotDestroyList.Contains(cur))
				Destroy(cur);
		}

		for (int i = 0; i < m_EquipmentRoot.transform.childCount; ++i)
		{
			var cur = m_EquipmentRoot.transform.GetChild(i).gameObject;

			if (!m_DoNotDestroyList.Contains(cur))
				Destroy(cur);
		}

		for (int i = 0; i < m_ItemsRoot.transform.childCount; ++i)
		{
			var cur = m_ItemsRoot.transform.GetChild(i).gameObject;

			if (!m_DoNotDestroyList.Contains(cur))
				Destroy(cur);
		}
	}
}
