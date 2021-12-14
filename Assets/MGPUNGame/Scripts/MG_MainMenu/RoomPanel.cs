using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace MGPUNGame2 { 

	public class RoomPanel : BasePanel
	{
		private Text RoomNameText;
		private Text PlayersInfoText;
		private Button StartGameButton;
		private Button LeaveRoomButton;
		private Button SureButton;
		private Button Role1Button;
		private Button Role2Button;
		private Button Role3Button;
		private Transform RoomInfoItemsParentContent;
		private List<PlayerInfoItem> m_PlayerItemList = new List<PlayerInfoItem>();
		private PlayerInfoItem m_PlayerInfoItemPre;

		private bool m_IsSure = false; // 是否已经选定角色的标记
		private string m_SureRoleName = "Role1";

		private void Awake()
        {
            RoomNameText = transform.Find("RoomNameText").GetComponent<Text>();
			PlayersInfoText = transform.Find("MiddlePage/PlayersInfoText").GetComponent<Text>();
			StartGameButton = transform.Find("MiddlePage/StartGameButton").GetComponent<Button>();
			LeaveRoomButton = transform.Find("MiddlePage/LeaveRoomButton").GetComponent<Button>();
			SureButton = transform.Find("MiddlePage/SureButton").GetComponent<Button>();
			Role1Button = transform.Find("MiddlePage/Role1Button").GetComponent<Button>();
			Role2Button = transform.Find("MiddlePage/Role2Button").GetComponent<Button>();
			Role3Button = transform.Find("MiddlePage/Role3Button").GetComponent<Button>();
			RoomInfoItemsParentContent = transform.Find("MiddlePage/RoomInfoScrollView/Viewport/RoomInfoItemsParentContent");
			m_PlayerInfoItemPre = Resources.Load<GameObject>("PlayerInfoItem").GetComponent<PlayerInfoItem>();
		}

        private void Start()
        {
			StartGameButton.onClick.RemoveAllListeners();
			StartGameButton.onClick.AddListener(() =>
			{
				OnClickStartGameButton();

			});

			SureButton.onClick.RemoveAllListeners();
            SureButton.onClick.AddListener(() =>
            {
				OnClickSendToSure();

			});

            LeaveRoomButton.onClick.RemoveAllListeners();
			LeaveRoomButton.onClick.AddListener(()=> {
				if (PhotonNetwork.IsConnected)
				{
					OnClickLeaveRoomButton();
					
				}
			});

			Role1Button.onClick.RemoveAllListeners();
			Role1Button.onClick.AddListener(() =>
			{
				if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
				{
					m_SureRoleName = Role1Button.GetComponentInChildren<Text>().text;
				}
			});

			Role2Button.onClick.RemoveAllListeners();
			Role2Button.onClick.AddListener(() =>
			{
				m_SureRoleName = Role2Button.GetComponentInChildren<Text>().text;
			});

			Role3Button.onClick.RemoveAllListeners();
			Role3Button.onClick.AddListener(() =>
			{
				m_SureRoleName = Role3Button.GetComponentInChildren<Text>().text;
			});
		}

       

        public void OnJoinedRoom() {
			ClearOldPlayerList(); // 清空可能之前的房间 Player 
			UpdateBaseInfo();
			GetCurrentRoomPlayers();
			JudageAllSureShowToStartGame();
		}
		public void OnPlayerEnteredRoom(Player newPlayer) {
			AddPlayersToList(newPlayer);
			UpdateBaseInfo(); 
			JudageAllSureShowToStartGame(); // 退出的时候监听是否满足开始游戏条件，不满足就取消开始游戏按钮显示
		}
		public void OnPlayerLeftRoom(Player otherPlayer) {
			RemovePlayersToList(otherPlayer);
			UpdateBaseInfo();
			JudageAllSureShowToStartGame(); // 退出的时候监听是否满足开始游戏条件
		}
		public void OnMasterClientSwitched(Player newMasterClient) {
			AddPlayersToList(newMasterClient);
			UpdateBaseInfo();
			JudageAllSureShowToStartGame();// 切换房主时候监听是否满足开始游戏条件
		}
		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
			AddPlayersToList(targetPlayer);
			JudageAllSureShowToStartGame();// 玩家主动发来的选定消息，监听是否满足开始游戏条件
		}

		/// <summary>
		/// 清空房间玩家信息
		/// </summary>
		void ClearOldPlayerList()
		{
			Debug.Log(GetType() + "/ClearOldPlayerList ()");

			if (m_PlayerItemList != null)
			{
				foreach (PlayerInfoItem item in m_PlayerItemList)
				{
                    if (item!=null)
                    {
						Destroy(item.gameObject);
					}
				}
				m_PlayerItemList.Clear();
			}

		}

		/// <summary>
		/// 更新房间基本消息
		/// </summary>
		void UpdateBaseInfo()
		{
			if (PhotonNetwork.IsConnected == false || PhotonNetwork.CurrentRoom == null)
			{
				return;
			}

			RoomNameText.text = $" {PhotonNetwork.CurrentRoom.Name}";
			PlayersInfoText.text = $"人员信息（ {PhotonNetwork.CurrentRoom.Players.Count}/{PhotonNetwork.CurrentRoom.MaxPlayers}）";

		}

		private void JudageAllSureShowToStartGame() {
			StartGameButton.gameObject.SetActive(false);
			if (PhotonNetwork.IsMasterClient == true)
			{
				foreach (PlayerInfoItem item in m_PlayerItemList)
				{
					if (item.IsSure == false)
					{
						return;
					}

				}
				StartGameButton.gameObject.SetActive(true);
			}
			
		}

		/// <summary>
		/// 进入房间后，已加入玩家和自己
		/// </summary>
		private void GetCurrentRoomPlayers()
		{
			if (PhotonNetwork.IsConnected == false || PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
			{
				return;
			}
			Debug.Log(GetType() + "/GetCurrentRoomPlayers ()/Players.cout =  " + PhotonNetwork.CurrentRoom.Players.Count);
			foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
			{
				AddPlayersToList(playerInfo.Value);
			}
		}

		/// <summary>
		/// 进入房间后，加入的其他玩家
		/// </summary>
		/// <param name="player"></param>
		private void AddPlayersToList(Player newPlayer)
		{
			Debug.Log(GetType() + "/AddPlayersToList ()");
			int index = m_PlayerItemList.FindIndex(x => x.Player.ActorNumber == newPlayer.ActorNumber);
			PlayerInfoItem item = null;
			if (index != -1) // 已存在就获取存在的 Player 更新
			{
				item = m_PlayerItemList[index];
			}
			else { // 否则重新生成
				item = Instantiate(m_PlayerInfoItemPre, RoomInfoItemsParentContent);
				m_PlayerItemList.Add(item);
			}

			if (item != null)
			{
				// 更新是否选定信息
				bool isSure = false;
				if (newPlayer.CustomProperties.ContainsKey(CommonDefine.PLAYER_SURE))
				{
					isSure = (bool)newPlayer.CustomProperties[CommonDefine.PLAYER_SURE];
				}
				item.IsSure = isSure;

				item.SetPlayerInfo(newPlayer, PhotonNetwork.CurrentRoom.MasterClientId == newPlayer.ActorNumber, PhotonNetwork.NickName == newPlayer.NickName, isSure);
				
			}
		}

		/// <summary>
		/// 玩家离开房间，从列表中移除
		/// </summary>
		/// <param name="otherPlayer"></param>
		private void RemovePlayersToList(Player otherPlayer) {
			int index = m_PlayerItemList.FindIndex(x => x.Player.ActorNumber == otherPlayer.ActorNumber);
			Debug.Log(GetType() + $"/AddPlayersToList ()/m_PlayerItemList.Count = {m_PlayerItemList.Count}， {otherPlayer.NickName}  index = {index}"  );
			if (index != -1)
			{
				Destroy(m_PlayerItemList[index].gameObject);
				m_PlayerItemList.RemoveAt(index);
			}
		}

		/// <summary>
		/// 玩家选定角色
		/// </summary>
		private void OnClickSendToSure() {
			
			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
			{
				m_IsSure = !m_IsSure;
				SureButton.GetComponentInChildren<Text>().text = m_IsSure==true?"取消选定":"选定";
				Hashtable props = new Hashtable() { { CommonDefine.PLAYER_SURE, m_IsSure }, { CommonDefine.PLAYER_SURE_ROLE, m_SureRoleName } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(props);
			}
			
		}

		private void OnClickLeaveRoomButton()
		{
			ResetInfo();
			PhotonNetwork.LeaveRoom(true);
		}

		private void OnClickStartGameButton()
		{
            if (PhotonNetwork.IsConnected&&PhotonNetwork.InRoom)
            {
				// 加载游戏场景的时候，进行房间不可见
				PhotonNetwork.CurrentRoom.IsOpen = false;
				PhotonNetwork.CurrentRoom.IsVisible = false;

				// 加载场景
				PhotonNetwork.LoadLevel(CommonDefine.PUN_GAME_LEVEL);
			}
		}

        void ResetInfo()
        {
			m_IsSure = false;
			SureButton.GetComponentInChildren<Text>().text = "选定";
			m_SureRoleName = "Role1";
			PhotonNetwork.LocalPlayer.CustomProperties.Clear(); // 注意如不只是离开房间， Player 的 CustomProperties 是不会自动清空的
		}
    }
}
