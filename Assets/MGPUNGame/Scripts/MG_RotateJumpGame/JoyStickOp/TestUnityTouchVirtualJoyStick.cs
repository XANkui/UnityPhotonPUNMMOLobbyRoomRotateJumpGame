using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANUtil
{

	public class TestUnityTouchVirtualJoyStick : MonoBehaviour, IUnityTouchVirtualJoyStickCallback
	{
		public Transform PlayerCubeTrans;
		public Transform MainCameraTrans;

		public RectTransform VJSBaseImageTrans;
		public RectTransform VJSDirImageTrans;

		private UnityTouchVirtualJoyStickWrapper mUnityTouchVirtualJoyStickWrapper = new UnityTouchVirtualJoyStickWrapper();

		private const float SMOOTHSPEED = 5;

		private Vector2 mVJSBasePos;
		private Vector3 mOffsetCamera;
		private const float SMOOTH_CAMERA_SPEED = 2.5f;

		// Start is called before the first frame update
		void Start()
		{
			Init();
		}

		// Update is called once per frame
		void Update()
		{
			if (mUnityTouchVirtualJoyStickWrapper != null)
			{
				mUnityTouchVirtualJoyStickWrapper.Update();

			}
		}

		void Init()
		{
			mUnityTouchVirtualJoyStickWrapper.Init(this);
			SetVJSActive(false);
			mOffsetCamera = MainCameraTrans.position - PlayerCubeTrans.position;
		}

		void SetVJSActive(bool isActive = true)
		{
			VJSBaseImageTrans.gameObject.SetActive(isActive);
			VJSDirImageTrans.gameObject.SetActive(isActive);
		}

		void GameObjectRotateAndnMoveUpdate(Vector2 offset)
		{
			// 100 临时调节值，根据需要自行调节即可
			float length = offset.magnitude / 100;
			// 旋转注视
			Quaternion qua = Quaternion.LookRotation(new Vector3(offset.normalized.x, 0, offset.normalized.y));
			// 人物旋转
			PlayerCubeTrans.rotation = qua;
			// 人物移动
			PlayerCubeTrans.Translate(new Vector3(offset.normalized.x, 0, offset.normalized.y) * Time.deltaTime * SMOOTHSPEED * length, Space.World);

		}

		void CameraSmoothFollow(Transform targetTran)
		{
			//MainCameraTrans.position = Vector3.Lerp(MainCameraTrans.position, targetTran.position + mOffsetCamera, Time.deltaTime * SMOOTH_CAMERA_SPEED);
			MainCameraTrans.position = targetTran.position + mOffsetCamera;
		}

		#region Interface
		public void TouchDownPos(Vector2 pos)
		{
			VJSBaseImageTrans.anchoredPosition = pos;
			VJSBaseImageTrans.anchoredPosition = pos;
			SetVJSActive();
			mVJSBasePos = pos;

		}
		public void TouchDownMovePos(Vector2 pos)
		{

			VJSDirImageTrans.anchoredPosition = pos;
			GameObjectRotateAndnMoveUpdate(pos - mVJSBasePos);
			CameraSmoothFollow(PlayerCubeTrans);

		}
		public void TouchDownMoveDeltraPos(Vector2 deltraPos) { }
		public void TouchUp()
		{
			SetVJSActive(false);
		}
		public void TouchClick() { }
		#endregion

	}
}