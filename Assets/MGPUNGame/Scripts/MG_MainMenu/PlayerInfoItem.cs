using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGPUNGame2 { 

	public class PlayerInfoItem : MonoBehaviour
	{
		private Text InfoText;
		private Transform MasterOrYouImageTrans;
		private Text IsSureText;

        Player m_Player;
        public Player Player { get => m_Player; }
        public bool IsSure { get; set; } // 玩家是否选定
        public void SetPlayerInfo(Player player, bool isMaster,bool isYou,bool isSure) {
            m_Player = player;
            FindComponent();
            UpdateInfo(player,isMaster,isYou,isSure);
        }

        void FindComponent() {
            if (InfoText==null || MasterOrYouImageTrans ==null || IsSureText == null)
            {
                InfoText = transform.Find("InfoText").GetComponent<Text>();
                MasterOrYouImageTrans = transform.Find("MasterOrYouImage");
                IsSureText = transform.Find("IsSureImage/IsSureText").GetComponent<Text>();
            }
        }

        void UpdateInfo(Player player, bool isMaster, bool isYou,bool isSure) {
            if (m_Player!=null)
            {
                InfoText.text = $"{m_Player.NickName}(等级：0)";

                // 房主和你的标识
                if (isMaster == true || isYou ==true)
                {
                    MasterOrYouImageTrans.gameObject.SetActive(true);
                    if (isMaster == true && isYou == true)
                    {
                        MasterOrYouImageTrans.GetComponentInChildren<Text>().text ="房主/你";
                    }
                    else if (isMaster == true)
                    {
                        MasterOrYouImageTrans.GetComponentInChildren<Text>().text = "房主";
                    }
                    else if (isYou == true)
                    {
                        MasterOrYouImageTrans.GetComponentInChildren<Text>().text = "你";
                    }

                }
                else {
                    MasterOrYouImageTrans.gameObject.SetActive(false);
                }

                // 选定与否标识
                if (player.CustomProperties.ContainsKey(CommonDefine.PLAYER_SURE))
                {
                    if ((bool)player.CustomProperties[CommonDefine.PLAYER_SURE]  == true)
                    {
                        IsSureText.text = $"选定{player.CustomProperties[CommonDefine.PLAYER_SURE_ROLE]}";
                    }
                    else {
                        IsSureText.text = "未选定";
                    }
                }
                
            }
        }
    }
}
