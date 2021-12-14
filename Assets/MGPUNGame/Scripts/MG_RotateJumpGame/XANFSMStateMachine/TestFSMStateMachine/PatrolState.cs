using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM.Test { 

    /// <summary>
    /// 巡逻状态
    /// </summary>
	public class PatrolState : BaseState
	{
        List<Transform> mPath  =new List<Transform>();
        int mIndex = 0;
        Transform mPlayerTrans;

        // 构造函数
        public PatrolState(FiniteStateMachine fsmStateManager) : base(fsmStateManager)
        {
            mStateID = StateID.Patrol;
            Transform pathTrans = GameObject.Find("Path").transform;
            Transform[] children = pathTrans.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child!= pathTrans)
                {
                    mPath.Add(child);
                }
            }

            mPlayerTrans = GameObject.Find("Player").transform;
        }

        // 巡逻函数
        public override void Act(GameObject npc)
        {
            npc.transform.LookAt(mPath[mIndex].position);
            npc.transform.Translate(Vector3.forward * Time.deltaTime *3);
            if (Vector3.Distance(npc.transform.position, mPath[mIndex].position)<1)
            {
                mIndex++;
                mIndex %= mPath.Count;
            }
        }

        // 状态监听切换函数
        public override void Reason(GameObject npc)
        {
            if (Vector3.Distance(npc.transform.position , mPlayerTrans.position)<3)
            {
                mFiniteStateMachine.PerformTransition(Transition.SeePlayer);
            }
        }
    }
}
