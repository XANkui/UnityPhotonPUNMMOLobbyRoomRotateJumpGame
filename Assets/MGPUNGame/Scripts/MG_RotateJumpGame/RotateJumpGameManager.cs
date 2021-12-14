using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace MGPUNGame2 { 

	public class RotateJumpGameManager : MonoBehaviourPunCallbacks
	{

		private Transform m_WorldTrans;
		private Transform m_UITrans;
		private Transform m_SpawnPos1Trans;
		private Transform m_SpawnPos2Trans;
		private Transform m_SpawnPos3Trans;
		private Transform[] m_SpawnPosTransArray = new Transform[3];
		private GameObject OnDieFallPlaneTriggerCylinderGo;
		private GameObject OnAITriggerShortRotationCylinderGo;
		private readonly string connectionStatusMessage = "    Connection Status: ";
		private readonly string ECS_QUIT_GAMEROOM_Message = "    ESC 退出游戏房间";

		private bool m_IsStartGame = false;

		private bool m_IsGameOver = false; // 游戏结束

		private CountdownTimer m_CountdownTimer;
		public CountdownTimer CountdownTimer { get {
                if (m_CountdownTimer==null)
                {
					m_CountdownTimer = new CountdownTimer(CommonDefine.ROOM_ROTATE_JUMP_START_COUNT_TIMER,
				CommonDefine.ROOM_ROTATE_JUMP_GAME_TIME);
				}
				return m_CountdownTimer; 
			} 
		}

		public bool IsGameOver { get=> m_IsGameOver; set=> m_IsGameOver=value; }

		private static RotateJumpGameManager m_Instance;
		public static RotateJumpGameManager Instance { get => m_Instance;  }

		#region Unity

		private void Awake()
        {
			m_Instance = this;

			m_WorldTrans = GameObject.Find("World").transform;
			m_UITrans = GameObject.Find("UI").transform;
			m_SpawnPos1Trans = m_WorldTrans.Find("SpawnPos1");
			m_SpawnPos2Trans = m_WorldTrans.Find("SpawnPos2");
			m_SpawnPos3Trans = m_WorldTrans.Find("SpawnPos3");
			m_SpawnPosTransArray[0]= m_SpawnPos1Trans;
			m_SpawnPosTransArray[1]= m_SpawnPos2Trans;
			m_SpawnPosTransArray[2]= m_SpawnPos3Trans;

			OnDieFallPlaneTriggerCylinderGo= m_WorldTrans.Find("OnDieFallPlaneTriggerCylinder").gameObject;
			OnDieFallPlaneTriggerCylinderGo.AddComponent<OnDieFallPlaneTrigger>();

			OnAITriggerShortRotationCylinderGo = m_WorldTrans.Find("RotateShortRC/OnAITriggerShortRotationCylinder").gameObject;
			OnAITriggerShortRotationCylinderGo.AddComponent<OnAITriggerShortRotationCylinder>();

			m_UITrans.gameObject.AddComponent<RotateJumpGameUIManager>();
		}

        public override void OnEnable()
        {
            base.OnEnable();
			m_CountdownTimer.OnCountdownTimerHasExpired += GameOver;
		}

        public override void OnDisable()
        {
			m_CountdownTimer.OnCountdownTimerHasExpired -= GameOver;
			base.OnDisable();
        }

        private void Start()
        {
			SetSelfLoadedLevelFinish();
			
		}

        // Update is called once per frame
        void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
            {
				LeaveRoom();
			}

			m_CountdownTimer.Update();
		}
		private void OnGUI()
		{
			GUIStyle fontStyle = new GUIStyle();
			fontStyle.normal.background = null;    //设置背景填充
			fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
			fontStyle.fontSize = 40;       //字体大小
			GUI.Label(new Rect(20, 20, 200, 200), PhotonNetwork.IsConnected ? ECS_QUIT_GAMEROOM_Message : "", fontStyle);
			GUI.Label(new Rect(20, 60, 200, 200), connectionStatusMessage + PhotonNetwork.NetworkClientState, fontStyle);

		}

		private void OnDestroy()
		{
			SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

			Resources.UnloadUnusedAssets();
		}

		#endregion

		#region 游戏逻辑

		public void LeaveRoom() {
			if (PhotonNetwork.IsConnected == true)
			{
				if (PhotonNetwork.InRoom == true)
				{
					PhotonNetwork.LocalPlayer.CustomProperties.Clear();
					PhotonNetwork.LeaveRoom();
				}
				Debug.Log("退出房间，准备退出游戏");

			}
		}

		private void StartGame() {
			Debug.Log("StartGame!");
			SpawnPlayer();

			MasterSpawnAIPlayer();
		}

		/// <summary>
		/// 生成角色
		/// </summary>
		void SpawnPlayer() {

			string prefabRoleName = CommonDefine.PLAYER_PREFAB_BASEPATH + PhotonNetwork.LocalPlayer.CustomProperties[CommonDefine.PLAYER_SURE_ROLE];
			Vector3 position = GetSpawnPosition(PhotonNetwork.LocalPlayer);
			Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));
			PhotonNetwork.Instantiate(prefabRoleName, position, rotation, 0).AddComponent<PlayerController>().gameObject.tag = "Player";
		}

		/// <summary>
		/// 自主客户端生成 AI
		/// </summary>
		void MasterSpawnAIPlayer() {
            if (PhotonNetwork.IsMasterClient==false)
            {
				return;
            }
			int maxCount = PhotonNetwork.CurrentRoom.MaxPlayers;
			int remainAICount = (maxCount - PhotonNetwork.CurrentRoom.PlayerCount);
			
			for (int i = 0; i < remainAICount; i++)
            {
				string prefabRoleName = CommonDefine.PLAYER_PREFAB_BASEPATH + "Role"+Random.Range(1,4); // 目前3个角色
				Vector3 position = GetSpawnPosition(maxCount - 1- i);
				Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				PhotonNetwork.Instantiate(prefabRoleName, position, rotation, 0).AddComponent<AI>().gameObject.tag="AI";
			}
		}

		/// <summary>
		/// 角色生成位置的确定
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public Vector3 GetSpawnPosition(Player player) {

			return GetSpawnPosition(player.GetPlayerNumber()); // 在 Lobby 场景挂载 PlayerNumbering 可以用来确定角色的生成位置
		}

		/// <summary>
		/// 角色生成位置的确定
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Vector3 GetSpawnPosition(int index)
		{
			Debug.Log(GetType() + "/GetSpawnPosition()/ index : " + index);
			return m_SpawnPosTransArray[index].position;
		}

		void GameOver() {
			m_IsGameOver = true;

			RotateSelf[] rotateSelfScripts = m_WorldTrans.GetComponentsInChildren<RotateSelf>();
            foreach (RotateSelf item in rotateSelfScripts)
            {
				item.IsRotate = false;

			}
		}

		/// <summary>
		/// 设置自己场景加载完成属性
		/// </summary>
		private void SetSelfLoadedLevelFinish() {
			Hashtable props = new Hashtable
			{
				{CommonDefine.PLAYER_LOADED_LEVEL, true}
			};
			PhotonNetwork.LocalPlayer.SetCustomProperties(props);
		}

		/// <summary>
		/// 检测是否所有玩家加载完场景
		/// </summary>
		/// <returns></returns>
		private bool CheckAllPlayerLoadedLevel()
		{
			foreach (Player p in PhotonNetwork.PlayerList)
			{
				object playerLoadedLevel;

				if (p.CustomProperties.TryGetValue(CommonDefine.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
				{
					if ((bool)playerLoadedLevel)
					{
						continue;
					}
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// 检测其他客户端是场景否加载完成
		/// </summary>
		/// <param name="targetPlayer"></param>
		/// <param name="changedProps"></param>
		void UpdateCheckAllPlayerLoadedLevel(Player targetPlayer, Hashtable changedProps) {
			Debug.Log("UpdateCheckAllPlayerLoadedLevel()====");

			if (changedProps.ContainsKey(CommonDefine.PLAYER_LOADED_LEVEL))
			{
				if (CheckAllPlayerLoadedLevel())
				{
					if (!m_IsStartGame)
					{
						m_IsStartGame = true;

						// 通知所有房间客户端，游戏开始
						ToAllPlayerToStartGame();
					}
				}
				else
				{

					Debug.LogError("Waiting for other players... not all players loaded yet. wait:");
				}
			}
		}

		void ToAllPlayerToStartGame() {

			
			Hashtable props = new Hashtable
			{
				{CommonDefine.ROOM_PROPERTIES_TO_START_GAME, true},
			};
			PhotonNetwork.CurrentRoom.SetCustomProperties(props);

			m_CountdownTimer.SetStartTime();
		}

		void OnRoomPropertiesUpdateToStartGame(Hashtable propertiesThatChanged) {

            if (propertiesThatChanged.ContainsKey(CommonDefine.ROOM_PROPERTIES_TO_START_GAME))
            {
				StartGame();
            }
		}

		public bool IsMasterClient { get { return PhotonNetwork.IsMasterClient; } }
			
		
		#endregion

		#region PUN Callbacks
		public override void OnLeftRoom()
        {
			if (PhotonNetwork.IsConnected == true)
				PhotonNetwork.LoadLevel(CommonDefine.PUN_MAINMENU_LEVEL);
		}

        public override void OnDisconnected(DisconnectCause cause)
        {
			Debug.LogError("OnDisconnected:"+cause);
			UnityEngine.SceneManagement.SceneManager.LoadScene(CommonDefine.PUN_MAINMENU_LEVEL);
		}

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}

			UpdateCheckAllPlayerLoadedLevel(targetPlayer, changedProps);
		}

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {

			Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
			m_CountdownTimer?.OnRoomPropertiesUpdate(propertiesThatChanged);
			OnRoomPropertiesUpdateToStartGame(propertiesThatChanged);
		}

        #endregion

    }
}
