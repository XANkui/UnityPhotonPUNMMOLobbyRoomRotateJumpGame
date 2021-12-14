using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGPUNGame2 { 

	public class LobbyPanel : BasePanel
	{
		private Text WelcomeText;
		private Text InfoText;
		private InputField RoomNumInputField;
		private Button CreateRoomButton;
		private Text RoomListText;
		private Transform RoomInfoItemsParentContent;
		private RoomInfoItem m_RoomInfoItemPrefab;
		private List<RoomInfoItem> m_RoomItemList = new List<RoomInfoItem>();

		private void Awake()
        {
			WelcomeText = transform.Find("LeftPage/WelcomeText").GetComponent<Text>() ;
			InfoText = transform.Find("LeftPage/InfoText").GetComponent<Text>() ;
			RoomNumInputField = transform.Find("LeftPage/RoomNumInputField").GetComponent<InputField>() ;
			CreateRoomButton = transform.Find("LeftPage/CreateRoomButton").GetComponent<Button>() ;
			RoomListText = transform.Find("RightPage/RoomListText").GetComponent<Text>();
			RoomInfoItemsParentContent = transform.Find("RightPage/RoomListScrollView/Viewport/RoomInfoItemsParentContent") ;
			m_RoomInfoItemPrefab = Resources.Load<GameObject>("RoomInfoItem").GetComponent<RoomInfoItem>();

		}

        private void Start()
        {
			CreateRoomButton.onClick.RemoveAllListeners();
			CreateRoomButton.onClick.AddListener(()=> { CreateRoom(); });

		}

        private void OnEnable()
        {
			UpdateBaseInfo();
		}

        private void Update()
        {
			
			UpdataOnlineRoomCountAndPlayers();

		}

        void UpdateBaseInfo() {
            if (PhotonNetwork.IsConnected==false)
            {
				return;
            }
			WelcomeText.text = $"欢迎 {PhotonNetwork.LocalPlayer.NickName}";
			InfoText.text = $"用户名：{PhotonNetwork.LocalPlayer.NickName}\n等级：0\n金币：500";
			RoomNumInputField.text = CommonDefine.RoomName;
		}

		void CreateRoom() {
			if (PhotonNetwork.IsConnected == false)
			{
				Debug.LogError("未连接服务器成功");
				return;
			}

			RoomOptions options = new RoomOptions();
			options.BroadcastPropsChangeToAll = true;
			options.MaxPlayers = CommonDefine.MAX_COUNT_PLAYER_PER_ROOM;
			PhotonNetwork.JoinOrCreateRoom(RoomNumInputField.text, options, TypedLobby.Default);
		}

		#region OnRoomListUpdate

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			UpdateRoomList(roomList);
		}
		
		void UpdataOnlineRoomCountAndPlayers()
        {
			if (PhotonNetwork.IsConnected == false)
			{
				return;
			}
			RoomListText.text = $"房间列表({PhotonNetwork.CountOfRooms})/在线人数{PhotonNetwork.CountOfPlayers}";
		}
		void UpdateRoomList(List<RoomInfo> roomList) {
			Debug.Log(string.Format("房间更新 roomList.roomList " + roomList.Count));
			foreach (RoomInfo info in roomList)
			{
				// 房间销毁 (房间销毁、房间不对外开发、房间不可见)
				if (info.RemovedFromList || info.IsOpen==false || info.IsVisible==false)
				{
					int index = m_RoomItemList.FindIndex(x => x.RoomInfo.Name == info.Name);
					if (index != -1)
					{
						Destroy(m_RoomItemList[index].gameObject);
						m_RoomItemList.RemoveAt(index);
					}
				}
				else // 创建房间
				{
					int index = m_RoomItemList.FindIndex(x => x.RoomInfo.Name == info.Name);
					if (index == -1)// 创建新房间
					{
						RoomInfoItem item = Instantiate(m_RoomInfoItemPrefab, RoomInfoItemsParentContent);
						if (item != null) 
						{
							item.SetRoomInfo(info);
							m_RoomItemList.Add(item);
						}
						
					}
					else
					{ // 更新存在的信息
						m_RoomItemList[index].SetRoomInfo(info);
					}
				}
			}

            
		}

		#endregion
	}
}
