using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM.Test { 

	public class NPC : MonoBehaviour
	{
		private FiniteStateMachine mFSMStateManager;  // NPC 的状态机

		// Start is called before the first frame update
		void Start()
		{
			InitFSM();
		}

		// Update is called once per frame
		void Update()
		{
			// 状态机运行
			mFSMStateManager.DoUpdate(this.gameObject);
		}

		/// <summary>
		/// 绑定 NPC 状态机
		/// </summary>
		void InitFSM() {
			mFSMStateManager = new FiniteStateMachine();  // 构建

			// 巡逻状态
			PatrolState patrolState = new PatrolState(mFSMStateManager);
			patrolState.AddTransition(Transition.SeePlayer,StateID.Chase);

			// 追赶状态
			ChaseState chaseState = new ChaseState(mFSMStateManager);
			chaseState.AddTransition(Transition.LostPlayer,StateID.Patrol);

			// 添加状态到状态击中
			mFSMStateManager.AddState(patrolState);
			mFSMStateManager.AddState(chaseState);
		}
	}	
}
