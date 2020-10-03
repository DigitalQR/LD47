using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AttackAction))]
public class AttackAnimator : MonoBehaviour
{
	private enum AnimationState
	{
		None,
		Setup,
		MoveToTarget,
		Animation,
		MoveBack,
		Finish,
	}

	[SerializeField]
	private bool m_MoveToTarget = false;

	[SerializeField]
	private float m_MoveTargetOffset = 0.3f;

	[SerializeField]
	private float m_MoveSpeedScale = 2.0f;

	private AttackAction m_Action = null;
	private AnimationState m_CurrentState;

	private Pawn m_CurrentCaster = null;
	private PawnAnimator m_CurrentCasterAnim = null;
	private ArenaTile m_CurrentTarget = null;

	private void Start()
    {
		m_Action = GetComponent<AttackAction>();
	}

	private void Update()
	{
		if (m_CurrentState != AnimationState.None)
		{
			AnimationState prevState;
			do
			{
				prevState = m_CurrentState;

				switch (m_CurrentState)
				{
					case AnimationState.Setup:
						{
							if (m_MoveToTarget && m_CurrentCasterAnim)
								m_CurrentCasterAnim.StartWalkAnimation(m_CurrentTarget.transform.position + m_CurrentCaster.FacingDir * -m_MoveTargetOffset, m_MoveSpeedScale);

							m_CurrentState = AnimationState.MoveToTarget;
							break;
						}

					case AnimationState.MoveToTarget:
						{
							if(!m_CurrentCaster.InBlockingAnimating)
								m_CurrentState = AnimationState.Animation;
							break;
						}

					case AnimationState.Animation:
						{
							//TODO - Anim hookup
							if (!m_CurrentCaster.InBlockingAnimating)
							{
								m_Action.ApplyCurrentAttack();

								if (m_MoveToTarget && m_CurrentCasterAnim)
									m_CurrentCasterAnim.StartWalkAnimation(m_CurrentCaster.CurrentTile.transform.position, m_MoveSpeedScale);

								m_CurrentState = AnimationState.MoveBack;
							}
							break;
						}

					case AnimationState.MoveBack:
						{
							if (!m_CurrentCaster.InBlockingAnimating)
								m_CurrentState = AnimationState.Finish;
							break;
						}

					case AnimationState.Finish:
						{
							m_CurrentState = AnimationState.None;
							m_CurrentCaster = null;
							m_CurrentTarget = null;
							m_CurrentCasterAnim = null;
							return;
						}
				}
			}
			while (m_CurrentState != prevState);
		}
	}

	public void BeginAttack(Pawn caster, ArenaTile target)
	{
		m_CurrentCaster = caster;
		m_CurrentTarget = target;
		m_CurrentCasterAnim = caster.GetComponent<PawnAnimator>();
		m_CurrentState = AnimationState.Setup;
	}
	
	public bool InBlockingAnimating
	{
		get => m_CurrentState != AnimationState.None;
	}
}
