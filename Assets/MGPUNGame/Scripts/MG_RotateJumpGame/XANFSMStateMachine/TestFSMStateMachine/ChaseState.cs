using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM.Test { 

    /// <summary>
    /// 追赶状态
    /// </summary>
	public class ChaseState : BaseState
    {
        Transform mPlayerTrans;


        public ChaseState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
        {
            mStateID = StateID.Chase;  // 设置状态 ID

            mPlayerTrans = GameObject.Find("Player").transform;
        }

        /// <summary>
        /// 状态中执行函数
        /// </summary>
        /// <param name="npc"></param>
        public override void Act(GameObject npc)
        {
            npc.transform.LookAt(mPlayerTrans.position);
            npc.transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }

        /// <summary>
        /// 切换条件监听
        /// </summary>
        /// <param name="npc"></param>
        public override void Reason(GameObject npc)
        {
            if (Vector3.Distance(npc.transform.position, mPlayerTrans.position) > 6)
            {
                mFiniteStateMachine.PerformTransition(Transition.LostPlayer);
            }
        }
    }
}
