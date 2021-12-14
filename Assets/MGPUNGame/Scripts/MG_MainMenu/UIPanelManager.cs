using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MGPUNGame2 { 

	public class UIPanelManager : MonoBehaviourPunCallbacks
    {
        private LoginPanel LoginPanel;
        private LobbyPanel LobbyPanel;
        private RoomPanel RoomPanel;

        private readonly string connectionStatusMessage = "    Connection Status: ";

        private PlayerNumbering m_PlayerNumbering; // 用于记录角色index,可以用来角色游戏开始生成的位置
        public static UIPanelManager Instance {
			get;private set;
		}

        private void Awake()
        {
			Instance = this;

            LoginPanel = transform.Find("PUNCanvas/LoginPanel").GetComponent<LoginPanel>();
            LobbyPanel = transform.Find("PUNCanvas/LobbyPanel").GetComponent<LobbyPanel>();
            RoomPanel = transform.Find("PUNCanvas/RoomPanel").GetComponent<RoomPanel>();

            if (m_PlayerNumbering == null || this.gameObject.GetComponent<PlayerNumbering>()==null)
            {
                m_PlayerNumbering = gameObject.AddComponent<PlayerNumbering>();
            }
        }

        private void Start()
        {
            ShowUIPanel(LoginPanel);
        }

        private void OnDestroy()
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            Resources.UnloadUnusedAssets();
        }

        void HideAllUIPanel() {
            LoginPanel.gameObject.SetActive(false);
            LobbyPanel.gameObject.SetActive(false);
            RoomPanel.gameObject.SetActive(false);
        }

        void ShowUIPanel(BasePanel uiPanel) {
            HideAllUIPanel();
            uiPanel.gameObject.SetActive(true);
        }

        #region PUN Callback
        public override void OnConnectedToMaster()
        {
            Debug.Log("conntected to server .");
            Debug.Log("NickName : " + PhotonNetwork.LocalPlayer.NickName);
            // 加入大厅，为的是创建房间，大厅可见
            PhotonNetwork.JoinLobby();


        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("disconntected form server for reason :" + cause.ToString());
            ShowUIPanel(LoginPanel);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("JoinedLobby success");
            ShowUIPanel(LobbyPanel);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) {
            LobbyPanel.OnRoomListUpdate(roomList);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log(string.Format("房间创建成功 "));
            ShowUIPanel(RoomPanel);

        }

        public override void OnJoinedRoom()
        {
            Debug.Log(string.Format("加入房间成功 "));
            ShowUIPanel(RoomPanel);
            RoomPanel.OnJoinedRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError(string.Format("房间创建失败 returnCode {0}：{1}", returnCode, message));
        }

        public override void OnLeftRoom()
        {
            Debug.Log(GetType() + "/OnLeftRoom ()");
            ShowUIPanel(LoginPanel);

        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log(string.Format($"{newPlayer.NickName} 新玩家进入房间 "));
            RoomPanel.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(string.Format($"{otherPlayer.NickName} 玩家离开房间 "));
            RoomPanel.OnPlayerLeftRoom(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log(string.Format($"{newMasterClient.NickName} 变为房主 "));
            RoomPanel.OnMasterClientSwitched(newMasterClient);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            Debug.LogWarning(GetType() + "/OnPlayerPropertiesUpdate()/Player " + targetPlayer.NickName);
            Debug.LogWarning(GetType() + "/OnPlayerPropertiesUpdate()/Player PlayerSure :" + targetPlayer.CustomProperties[CommonDefine.PLAYER_SURE]);
            Debug.LogWarning(GetType() + "/OnPlayerPropertiesUpdate()/Player PlayerSureRoleName :" + targetPlayer.CustomProperties[CommonDefine.PLAYER_SURE_ROLE]);
            RoomPanel.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }
        #endregion

        #region Connection Status

        private void OnGUI()
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = null;    //设置背景填充
            fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色
            fontStyle.fontSize = 40;       //字体大小
            GUI.Label(new Rect(20, 20, 200, 200), connectionStatusMessage + PhotonNetwork.NetworkClientState, fontStyle);

        }

        #endregion

       
    }
}
