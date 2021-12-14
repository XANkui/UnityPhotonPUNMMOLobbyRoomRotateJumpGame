using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANFSM {

	/// <summary>
	/// 过渡 ID 
	/// 可以根据需要添加多种 过渡 ID
	/// </summary>
	public enum Transition 
	{
		None,

		// NPC 
		SeePlayer,
		LostPlayer,

		// UI 

		// MGPUNGame 2
		Idle_MGPUNGame,
		Wander_MGPUNGame,
	}
}
