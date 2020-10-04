using DQR.Debug;
using DQR.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EquipableTarget))]
[RequireComponent(typeof(PawnHealth))]
public class Pawn : MonoBehaviour
{
	[Header("Random Variation")]
	[SerializeField]
	private WeightedCollection<string> m_NameOptions = default;

	[SerializeField]
	private TintVariationCollection m_TintVariation = default;

	private string m_PawnName = null;
	private PawnAnimator m_Animator = null;
	private PawnHealth m_Health = null;
	private EquipableTarget m_Equipment = null;

	private int m_TeamIndex = -1;
	private Vector2Int m_FacingDirection = default;
	private ArenaTile m_CurrentTile = null;

	private void Start()
	{
		m_Animator = GetComponent<PawnAnimator>();
		m_Equipment = GetComponent<EquipableTarget>();
		m_Health = GetComponent<PawnHealth>();
	}

	public string PawnName
	{
		get => m_PawnName;
	}

	public ArenaTile CurrentTile
	{
		get => m_CurrentTile;
	}

	public Vector2Int CurrentCoords
	{
		get
		{
			Assert.Message(m_CurrentTile, "Coords aren't known");
			return m_CurrentTile.Coord;
		}
	}

	public int TeamIndex
	{
		get => m_TeamIndex;
		set => m_TeamIndex = value;
	}

	public AttackStats BaseStats
	{
		get => m_Equipment.BaseStats;
	}

	public AttackStats CurrentStats
	{
		get => m_Equipment.CurrentStats;
	}

	public Vector2Int FacingCoordDir
	{
		get => m_FacingDirection;
	}

	public Vector3 FacingDir
	{
		get => new Vector3(m_FacingDirection.x, 0, m_FacingDirection.y).normalized;
	}

	public bool HasAttackActions
	{
		get => m_Equipment && m_Equipment.HasAttackActions;
	}

	public bool IsPendingDestroy
	{
		get => m_Health.IsDead && InBlockingAnimating;
	}

	public IEnumerable<AttackAction> AttackActions
	{
		get => m_Equipment.AttackActions;
	}

	private void Event_OnTileContentChanged(ArenaTile changeTile)
	{
		if (m_CurrentTile)
		{
			// This item has been moved
			if (changeTile == m_CurrentTile && changeTile.Content != gameObject)
				m_CurrentTile = null;
		}

		if (changeTile.Content == gameObject)
			m_CurrentTile = changeTile;
	}

	public void MoveToTile(ArenaTile target, bool shouldAnimate)
	{
		Vector3 startPosition = transform.position;
		target.Content = gameObject;

		if (shouldAnimate && m_Animator != null)
		{
			transform.position = startPosition;
			m_Animator.StartWalkAnimation(target.transform.position);
		}
	}

	public void ReceiveDamage(DamageEvent damageEvent)
	{
		m_Equipment.CurrentStats.ModifyRecievedEvent(damageEvent);
		EventHandler.Invoke("OnPawnAttacked", damageEvent);

		bool hasHit = Random.value <= damageEvent.Accuracy;

		if (hasHit)
		{
			m_Health.ApplyDamage(damageEvent);
		}
		else
		{
			PopupManager.Instance.CreatePopup3D("Miss", transform.position, 1.0f, Color.gray, FontStyle.Italic);
		}
	}

	public void SetFacingDirection(Vector2Int facingDir)
	{
		m_FacingDirection = facingDir;

		// Gets called really early, so make sure grabbed the component
		if (!m_Animator)
			m_Animator = GetComponent<PawnAnimator>();

		if (m_Animator)
			m_Animator.SetFacingDirection(facingDir);
	}

	private void Event_OnRegenerateBoard(ArenaBoard board)
	{
		if (m_CurrentTile != null && board.TryGetTile(m_CurrentTile.Coord, out ArenaTile newTile))
		{
			m_CurrentTile = null;
			MoveToTile(newTile, true);
		}
	}
	
	public void ApplyRandomVariantion(AttackStats instanceStatsDelta)
	{
		m_PawnName = m_NameOptions.SelectRandom();
		m_TintVariation.ApplyVariationTo(gameObject);
		GetComponent<EquipableTarget>().IncreaseBaseStats(instanceStatsDelta);
	}

	public bool InBlockingAnimating
	{
		get => m_Animator && m_Animator.InBlockingAnimating;
	}

	public void ShowInfoPanel()
	{
		List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
		content.Add(new KeyValuePair<string, string>("Health", "" + m_Health.CurrentHealth));
		content.AddRange(m_Equipment.GetPanelContent(TeamIndex != 0));
		InfoPanelManager.Instance.OpenPanel(m_PawnName, "Creature", content);
	}
}
