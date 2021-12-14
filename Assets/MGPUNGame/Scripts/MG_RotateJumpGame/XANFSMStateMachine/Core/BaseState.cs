using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM { 

	/// <summary>
	/// 基础状态
	/// </summary>
	public abstract class BaseState
	{
		// 状态ID
		protected StateID mStateID; 
		public StateID MStateID { get { return mStateID; } }

		// 状态装换 对应的 状态ID 字典集合
		protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
		
		// 有限状态机 （方便 Reason() 函数满足条件后状态切换）
		protected FiniteStateMachine mFiniteStateMachine;

		// 构造函数
		public BaseState(FiniteStateMachine finiteStateMachine) {
			mFiniteStateMachine = finiteStateMachine;
		}

		/// <summary>
		/// 添加状态过渡对应的状态到字典中
		/// </summary>
		/// <param name="trans">状态过渡</param>
		/// <param name="id">状态ID</param>
		public void AddTransition(Transition trans, StateID id)
        {
            if (trans == Transition.None)
            {
				Debug.LogError(GetType()+ "/AddTransition()/ transition is None");
				return;
			}
			if (id == StateID.None)
			{
				Debug.LogError(GetType() + "/AddTransition()/ stateId is None");
				return;
			}
			if (map.ContainsKey(trans))
			{
				Debug.LogError(GetType() + "/AddTransition()/ transition map has already existed, transition = "+trans);
				return;
			}

			map.Add(trans,id);
		}

		// 删除状态集合中的指定的状态过渡
		public void DeleteTransition(Transition trans)
		{
			if (trans == Transition.None)
			{
				Debug.LogError(GetType() + "/DeleteTransition()/ transition is None");
				return;
			}
		
			if (map.ContainsKey(trans)==false)
			{
				Debug.LogError(GetType() + "/DeleteTransition()/ transition map has not  existed, transition = " + trans);
				return;
			}

			map.Remove(trans);
		}

		/// <summary>
		/// 获取对应状态过渡对应的状态
		/// </summary>
		/// <param name="trans"></param>
		/// <returns></returns>
		public StateID GetStateWithTransition(Transition trans) {
            if (map.ContainsKey(trans))
            {
				return map[trans];
            }

			return StateID.None;
		}

		// 状态切换进入前的回调
		public virtual void DoBeforeEntering() { }
		// 状态切换退出的状态的回调
		public virtual void DoAfterLeaving() { }
		public abstract void Act(GameObject npc);	// 执行的回调
		public abstract void Reason(GameObject npc); // 转换条件
	}
}
