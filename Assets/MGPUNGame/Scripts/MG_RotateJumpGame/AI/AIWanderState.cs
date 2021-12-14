using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XANFSM;

namespace MGPUNGame2 { 

	public class AIWanderState : BaseState
	{
        Vector3 m_CurWanderTargetPos;
        private static Vector3 RADIUM_CENTER_POS;
        private const float INNER_RDIUMS = 6;
        private const float OUTTER_RDIUMS = 13;
        public AIWanderState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
        {
            mStateID = StateID.AIWander_MGPUNGame;
            RADIUM_CENTER_POS = new Vector3(0,2,0) ;

            m_CurWanderTargetPos = GetNextWanderPos();
        }

        public override void Act(GameObject npc)
        {
            if (RotateJumpGameManager.Instance.IsMasterClient == false) return;

            npc.transform.LookAt(m_CurWanderTargetPos);
            npc.transform.Translate(Vector3.forward * Time.deltaTime * 3);
            if (Vector3.Distance(npc.transform.position, m_CurWanderTargetPos) < 1 || JudgeKnockToCenterCylinder(npc.transform.position, m_CurWanderTargetPos))
            {
                m_CurWanderTargetPos = GetNextWanderPos();
            }
        }

        public override void Reason(GameObject npc)
        {
            if (RotateJumpGameManager.Instance.IsMasterClient == false) return;
            if (RotateJumpGameManager.Instance.IsGameOver)
            {
                mFiniteStateMachine.PerformTransition(Transition.Idle_MGPUNGame);
            }
           
        }

        // 获取下一个漫游地点
        public Vector3 GetNextWanderPos() {
            Vector3 tmpPos = RandomCirclePos(INNER_RDIUMS, OUTTER_RDIUMS, RADIUM_CENTER_POS);
            m_CurWanderTargetPos = tmpPos;
            return m_CurWanderTargetPos;
        }
        /// <summary>
		/// 跳过障碍 //该功能
		/// </summary>
		public void JumpObstacle(GameObject npc)
        {
            npc.GetComponent<Rigidbody>().AddForce(npc.transform.up * 5 * 80);
        }
        /// <summary>
        /// 判断是否撞到中间圆柱
        /// </summary>
        /// <param name="curPos"></param>
        /// <param name="centerPos"></param>
        /// <returns></returns>
        private bool JudgeKnockToCenterCylinder(Vector3 curPos,Vector3 centerPos) {
            return Vector3.Distance(curPos,centerPos)<=5.8f;
        }

        #region 工具类
        /// <summary>
		/// 环形随机位置
		/// </summary>
		/// <param name="innerRadius">内径</param>
		/// <param name="toOutderadd">向外扩展的宽度</param>
		/// <param name="centerPos">中心位置</param>
		/// <returns></returns>
		Vector3 RandomCirclePos(float innerRadius, float toOutderadd, Vector3 centerPos)
        {
            Vector2 p = Random.insideUnitCircle * toOutderadd;
            Vector2 pos = p.normalized * (innerRadius + p.magnitude);
            return new Vector3(pos.x, 0, pos.y) + centerPos;

        }

        #endregion
    }
}
