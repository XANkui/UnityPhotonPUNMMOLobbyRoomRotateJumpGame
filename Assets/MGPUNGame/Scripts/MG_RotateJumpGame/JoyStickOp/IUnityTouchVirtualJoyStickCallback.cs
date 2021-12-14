using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANUtil
{
	public interface IUnityTouchVirtualJoyStickCallback
	{
		void TouchDownPos(Vector2 pos);
		void TouchDownMovePos(Vector2 pos);
		void TouchDownMoveDeltraPos(Vector2 deltraPos);
		void TouchUp();

		void TouchClick();
	}
}