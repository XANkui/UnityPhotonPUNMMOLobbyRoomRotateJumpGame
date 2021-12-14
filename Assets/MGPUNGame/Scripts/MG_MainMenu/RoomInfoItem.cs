using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGPUNGame2 { 

	public class RoomInfoItem : MonoBehaviour
	{
		private Text InfoText;
		private Button JoinRoomButton;

        private RoomInfo m_RoomInfo;
        public RoomInfo RoomInfo { get => m_RoomInfo; }

        private void Awake()
        {
            InfoText = transform.Find("InfoText").GetComponent<Text>();
            JoinRoomButton = transform.Find("JoinRoomButton").GetComponent<Button>();
        }

        private void Start()
        {
            JoinRoomButton.onClick.RemoveAllListeners();
            JoinRoomButton.onClick.AddListener(()=> { JoinRoom(); });
        }

        public void SetRoomInfo(RoomInfo roomInfo) {
            m_RoomInfo = roomInfo;
            InfoText.text = $"{roomInfo.Name}({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        }

        void JoinRoom() {
            if (m_RoomInfo != null)
            {
                PhotonNetwork.JoinRoom(m_RoomInfo.Name);
            }
        }
	}
}
