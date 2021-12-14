using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MGPUNGame2 { 

	public class RotateJumpGameUIManager : MonoBehaviour
	{
		private Text CountDownTimerText;
		private GameObject GameOverPageGo;
		private Button BackButton;

		private GameObject TouchOpCanvasGo;

		private int m_RemainTime;
		private bool m_IsGameOVer = false;
		CountdownTimer m_CountdownTimer;
		CountdownTimer CountdownTimer {
			get {
                if (m_CountdownTimer==null)
                {
					m_CountdownTimer = RotateJumpGameManager.Instance.CountdownTimer;

				}

				return m_CountdownTimer;
			}
		}
		private void Awake()
        {
			CountDownTimerText = transform.Find("GameUICanvas/CountDownTimerText").GetComponent<Text>();
			BackButton = transform.Find("GameUICanvas/GameOverPage/BackButton").GetComponent<Button>();
			GameOverPageGo = transform.Find("GameUICanvas/GameOverPage").gameObject;
			TouchOpCanvasGo = transform.Find("TouchOpCanvas").gameObject;

		}

        private void OnEnable()
        {
			CountdownTimer.OnCountdownTimerHasExpired += GameOver;
		}

        private void OnDisable()
        {
			CountdownTimer.OnCountdownTimerHasExpired -= GameOver;
			
		}

        // Start is called before the first frame update
        void Start()
		{
			GameOverPageGo.SetActive(false);
			BackButton.onClick.RemoveAllListeners();
			BackButton.onClick.AddListener(()=> {
				LeaveRoom();
			});

		}

		// Update is called once per frame
		void Update()
		{
			UpdateText();
		}
		void UpdateText() {
			if (m_IsGameOVer == true)
				return;
			m_RemainTime = Mathf.CeilToInt(CountdownTimer.TimeRemaining()) -1;
			CountDownTimerText.text = $"剩余时间：{m_RemainTime} s";
		}
		void LeaveRoom() {
			RotateJumpGameManager.Instance.LeaveRoom();
		}
		void GameOver()
		{
			m_IsGameOVer = true;
			GameOverPageGo.SetActive(true);
			TouchOpCanvasGo.SetActive(false);
		}
	}
}
