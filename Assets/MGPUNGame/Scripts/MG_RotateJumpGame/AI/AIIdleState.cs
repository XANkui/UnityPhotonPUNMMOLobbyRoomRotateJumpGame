using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XANFSM;

namespace MGPUNGame2
{ 

	public class AIIdleState : BaseState
	{
        public AIIdleState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
        {
            mStateID = StateID.AIIdle_MGPUNGame;
        }

        public override void Act(GameObject npc)
        {
            if (RotateJumpGameManager.Instance.IsMasterClient == false) return;
        }

        public override void Reason(GameObject npc)
        {
            if (RotateJumpGameManager.Instance.IsMasterClient == false) return;
        }
    }
}
