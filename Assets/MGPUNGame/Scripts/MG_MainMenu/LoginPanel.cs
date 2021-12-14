using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGPUNGame2 { 

	public class LoginPanel : BasePanel
	{
        private Button m_CustomerButton;
        private void Awake()
        {
            m_CustomerButton = transform.Find("CustomerButton").GetComponent<Button>();
        }

		// Start is called before the first frame update
		void Start()
		{

			m_CustomerButton.onClick.RemoveAllListeners();
			m_CustomerButton.onClick.AddListener(()=> {
				LoginToPUNLobby();
			});
		}

		void LoginToPUNLobby() {
			Debug.Log("conntecting to server .");

			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.NickName = CommonDefine.NickName;
			PhotonNetwork.ConnectUsingSettings();
		}
	}
}
