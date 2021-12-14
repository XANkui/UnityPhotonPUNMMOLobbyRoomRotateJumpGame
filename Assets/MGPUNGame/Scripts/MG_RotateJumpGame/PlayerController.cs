using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XANUtil;

namespace MGPUNGame2 { 

	public class PlayerController : MonoBehaviour, IUnityTouchVirtualJoyStickCallback
	{
		private Transform m_PlayerCubeTrans;
		private Transform m_MainCameraTrans;

		private Transform m_UI;
		private RectTransform m_VJSBaseImageTrans;
		private RectTransform m_VJSDirImageTrans;

		private UnityTouchVirtualJoyStickWrapper m_UnityTouchVirtualJoyStickWrapper = new UnityTouchVirtualJoyStickWrapper();

		[SerializeField]
		private float m_SMOOTHSPEED = 5; // 平滑运动速度

		private Vector2 m_VJSBasePos;
		private Vector3 m_OffsetCamera;
		private const float SMOOTH_CAMERA_SPEED = 2.5f;


		private Rigidbody m_Rigidbody;
		private Collider m_Collider;
		private Renderer m_Renderer;

		private OnColliderGroundTrigger m_OnColliderGroundTrigger;

		private PhotonView m_PhotonView;
		private bool m_Controllable = true;

        #region Unity


        private void Awake()
        {
			InitGetComponents();

		}

        private void OnEnable()
        {
			RotateJumpGameManager.Instance.CountdownTimer.OnCountdownTimerHasExpired += GameOver;

		}
        private void OnDisable()
        {
			RotateJumpGameManager.Instance.CountdownTimer.OnCountdownTimerHasExpired -= GameOver;
		}
        // Start is called before the first frame update
        void Start()
		{
			Init();
		}

        // Update is called once per frame
        void Update()
		{
			if (!m_PhotonView.AmOwner || !m_Controllable)
			{
				return;
			}

			// we don't want the master client to apply input to remote ships while the remote player is inactive
			if (this.m_PhotonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return;
			}

			OpUpdate();
		}

		#endregion

		#region 游戏逻辑
		void GameOver() {
			m_Controllable = false;
		}
		private void InitGetComponents()
        {
			m_PhotonView = GetComponent<PhotonView>();


			m_Rigidbody = GetComponent<Rigidbody>();
			m_Collider = GetComponent<Collider>();
			m_Renderer = GetComponent<Renderer>();

			m_OnColliderGroundTrigger = transform.Find("OnColliderTrigger").gameObject.AddComponent<OnColliderGroundTrigger>();

			m_PlayerCubeTrans = this.transform;
			m_MainCameraTrans = GameObject.Find("CameraParent").transform;
			m_UI = GameObject.Find("UI").transform;
			m_VJSBaseImageTrans = m_UI.Find("TouchOpCanvas/VJSBaseImageTrans").GetComponent<RectTransform>();
			m_VJSDirImageTrans = m_UI.Find("TouchOpCanvas/VJSDirImageTrans").GetComponent<RectTransform>();
		}

		void Init()
		{
			m_UnityTouchVirtualJoyStickWrapper.Init(this);
			SetVJSActive(false);

			m_OffsetCamera = m_MainCameraTrans.position - m_PlayerCubeTrans.position;
			m_OffsetCamera.y = 0;

		}

		private void OpUpdate() {
			if (m_UnityTouchVirtualJoyStickWrapper != null)
			{
				m_UnityTouchVirtualJoyStickWrapper.Update();

			}
		}

        private void LateUpdate()
        {
			CameraSmoothFollowRotateTo2();

		}

        bool ISJudgeIsMineAndControllable()
		{
			if (!m_PhotonView.IsMine)
			{
				return false;
			}

			if (!m_Controllable)
			{
				return false;
			}

			return true;
		}

		void SetVJSActive(bool isActive = true)
		{
			m_VJSBaseImageTrans.gameObject.SetActive(isActive);
			m_VJSDirImageTrans.gameObject.SetActive(isActive);
		}

		void GameObjectRotateAndnMoveUpdate(Vector2 offset)
		{
			// 100 临时调节值，根据需要自行调节即可
			float length = offset.magnitude / 100;
			// 旋转注视
			Quaternion qua = Quaternion.LookRotation(new Vector3(offset.normalized.x, 0, offset.normalized.y));
			// 人物旋转
			m_PlayerCubeTrans.rotation = qua;
			// 再次修正相机变化的角度
			Vector3 playerAngle = m_PlayerCubeTrans.eulerAngles;
			playerAngle.y += m_MainCameraTrans.eulerAngles.y;
			m_PlayerCubeTrans.eulerAngles = playerAngle;
			// 人物移动
			float runSpeed = m_SMOOTHSPEED * length;
			runSpeed = runSpeed > m_SMOOTHSPEED ? m_SMOOTHSPEED : runSpeed;
			//Debug.LogWarning($"m_PlayerCubeTrans runSpeed = {runSpeed}");
			m_PlayerCubeTrans.Translate(m_PlayerCubeTrans.forward * Time.deltaTime * runSpeed, Space.World);
		}

		void CameraSmoothFollowRotateTo2()
		{
			if (ISJudgeIsMineAndControllable() == false) return;
			Vector3 dir = m_PlayerCubeTrans.transform.position * -1;
			dir.y = 0;
			Quaternion q = Quaternion.LookRotation(dir);
			m_MainCameraTrans.rotation = Quaternion.Slerp(m_MainCameraTrans.rotation, q, 3.0f * Time.deltaTime);
		}

		public void ToBackSpawnPostion() {
			this.transform.position = RotateJumpGameManager.Instance.GetSpawnPosition(PhotonNetwork.LocalPlayer);
		}

		#endregion

		#region IUnityTouchVirtualJoyStickCallback


		public void TouchDownPos(Vector2 pos)
		{
			if (ISJudgeIsMineAndControllable() == false) return;

			m_VJSBaseImageTrans.anchoredPosition = pos;
			m_VJSBaseImageTrans.anchoredPosition = pos;
			SetVJSActive();
			m_VJSBasePos = pos;

		}
		public void TouchDownMovePos(Vector2 pos)
		{
			if (ISJudgeIsMineAndControllable() == false) return;

			m_VJSDirImageTrans.anchoredPosition = pos;
			GameObjectRotateAndnMoveUpdate(pos - m_VJSBasePos);

		}
		public void TouchDownMoveDeltraPos(Vector2 deltraPos) { }
		public void TouchUp()
		{
			if (ISJudgeIsMineAndControllable() == false) return;

			SetVJSActive(false);

		}

		public void TouchClick()
		{
			if (ISJudgeIsMineAndControllable() == false) return;
			if (m_OnColliderGroundTrigger.IsGround == false) return;
			m_PlayerCubeTrans.GetComponent<Rigidbody>().AddForce(m_PlayerCubeTrans.up * m_SMOOTHSPEED * 80);
		}

		#endregion
	}
}
