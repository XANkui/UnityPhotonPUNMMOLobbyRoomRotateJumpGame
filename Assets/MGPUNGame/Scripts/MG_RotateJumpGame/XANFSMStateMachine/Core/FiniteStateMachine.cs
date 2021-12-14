using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM { 

	/// <summary>
	/// 有限状态机
	/// </summary>
	public class FiniteStateMachine 
	{
		private Dictionary<StateID, BaseState> statesDict = new Dictionary<StateID, BaseState>();
		private StateID mCurrentStateId;
		private BaseState mCurrentFSMState;

		/// <summary>
		/// Update 更新
		/// </summary>
		/// <param name="npc"> GameObject 可以根据需要进行更改 Object </param>
		public void DoUpdate(GameObject npc) {
			mCurrentFSMState.Act(npc);
			mCurrentFSMState.Reason(npc);
		}

		/// <summary>
		/// 添加状态到字典中
		/// </summary>
		/// <param name="fsmState"></param>
		public void AddState(BaseState fsmState) {
            if (fsmState==null)
            {
				Debug.LogError(GetType() + "/AddState()/ fsmState is null");
				return;
			}
           

            if (statesDict.ContainsKey(fsmState.MStateID))
            {
				Debug.LogError(GetType() + "/AddState()/ statesDict had already existed, fsmState.StateID = "+ fsmState.MStateID);
				return;
			}

			// 设置默认状态
			if (mCurrentFSMState == null)
			{
				mCurrentFSMState = fsmState;
				mCurrentStateId = fsmState.MStateID;
			}

			statesDict.Add(fsmState.MStateID,fsmState);
		}

		/// <summary>
		/// 删除状态
		/// </summary>
		/// <param name="stateID"></param>
		public void DeleteState(StateID stateID)
		{
			if (stateID == StateID.None)
			{
				Debug.LogError(GetType() + "/DeleteState()/ stateID is None");
				return;
			}


			if (statesDict.ContainsKey(stateID)==false)
			{
				Debug.LogError(GetType() + "/DeleteState()/ statesDict had not existed, StateID = " + stateID);
				return;
			}

			

			statesDict.Remove(stateID);
		}

		/// <summary>
		/// 状态切换到指定状态
		/// </summary>
		/// <param name="trans"></param>
		public void PerformTransition(Transition trans) {
            if (trans == Transition.None)
            {
				Debug.LogError(GetType() + "/PerformTransition()/ trans is None，空转换条件无法执行" );
				return;
			}
			StateID stateID = mCurrentFSMState.GetStateWithTransition(trans);
            if (stateID==StateID.None)
            {
				Debug.LogWarning(GetType() + "/PerformTransition()/ stateID form GetOutputState(trans)  is None，状态为空");
				return;
			}

            if (statesDict.ContainsKey(stateID) ==false)
            {
				Debug.LogError(GetType() + "/PerformTransition()/ statesDict had not stateID，StateID = " +stateID);
				return;
			}

			// 退出当前状态的前的回调
            if (mCurrentFSMState!=null)
            {
				mCurrentFSMState.DoAfterLeaving();
            }

			// 状态更新
			BaseState state = statesDict[stateID];
			mCurrentFSMState = state;
			mCurrentStateId = stateID;

			// 切换后新当前状态的前的回调
			if (mCurrentFSMState != null)
			{
				mCurrentFSMState.DoBeforeEntering();
			}
		}
	}
}
