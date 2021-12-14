using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XANFSM;

namespace MGPUNGame2 { 

	public class AI : MonoBehaviour
	{
		private FiniteStateMachine mFSMStateManager;  // NPC 的状态机
		private AIWanderState m_AIWanderState;
		private AIIdleState m_AIIdleState;
		private OnColliderGroundTrigger m_OnColliderGroundTrigger;

        private void Awake()
        {
			m_OnColliderGroundTrigger = transform.Find("OnColliderTrigger").gameObject.AddComponent<OnColliderGroundTrigger>();
		}

        // Start is called before the first frame update
        void Start()
		{
			InitFSM();
		}

		// Update is called once per frame
		void Update()
		{
			if (RotateJumpGameManager.Instance.IsMasterClient == false) return;
			// 状态机运行
			mFSMStateManager.DoUpdate(this.gameObject);
		}

		/// <summary>
		/// 绑定 NPC 状态机
		/// </summary>
		void InitFSM()
		{
			if (RotateJumpGameManager.Instance.IsMasterClient == false) return;

			mFSMStateManager = new FiniteStateMachine();  // 构建

			// 漫游状态
			m_AIWanderState = new AIWanderState(mFSMStateManager);
			m_AIWanderState.AddTransition(Transition.Idle_MGPUNGame, StateID.AIIdle_MGPUNGame);

			// Idle状态
			m_AIIdleState = new AIIdleState(mFSMStateManager);
			m_AIIdleState.AddTransition(Transition.Wander_MGPUNGame,StateID.AIWander_MGPUNGame);

			// 添加状态到状态击中
			mFSMStateManager.AddState(m_AIWanderState);
			mFSMStateManager.AddState(m_AIIdleState);
		}

		// 回到出生位置
		public void ToBackSpawnPostion() {
			this.transform.position = RotateJumpGameManager.Instance.GetSpawnPosition(Random.Range(0,3)); // 目前三个位置任意一个
			m_AIWanderState.GetNextWanderPos();
		}

		public bool IsGround {
			get { return m_OnColliderGroundTrigger.IsGround; }
		}

		/// <summary>
		/// 跳过障碍 //该功能
		/// </summary>
		public void JumpObstacle() {
			if (IsGround == false) return;
				m_AIWanderState.JumpObstacle(gameObject);
		}
	}
}
