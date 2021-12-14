using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM
{ 
	/// <summary>
	/// 状态 ID 
	/// 可以根据需要添加多种 ID
	/// </summary>
	public enum StateID 
	{
		None=0,

		// NPC 
		Patrol,
		Chase,

		// UI 

		// MGPUNGame2
		AIIdle_MGPUNGame,
		AIWander_MGPUNGame,
	}
}
