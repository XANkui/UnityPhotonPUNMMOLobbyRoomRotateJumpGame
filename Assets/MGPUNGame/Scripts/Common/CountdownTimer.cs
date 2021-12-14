using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace MGPUNGame2 { 

    /// <summary>
    /// 房间倒计时
    /// </summary>
	public class CountdownTimer
	{
        /// <summary>
        ///     OnCountdownTimerHasExpired delegate.
        /// </summary>
        public delegate void CountdownTimerHasExpired();

        public string m_CountdownStartTimeStr = "StartTime";

        private float m_Countdown = 5.0f;

        private bool m_IsTimerRunning;

        private int m_StartTime;

        /// <summary>
        ///     Called when the timer has expired.
        /// </summary>
        public event CountdownTimerHasExpired OnCountdownTimerHasExpired;

        public CountdownTimer(string countdownStartTimeStr, float countdown)
        {
            m_CountdownStartTimeStr = countdownStartTimeStr;
            m_Countdown = countdown;
            m_IsTimerRunning = false;
            m_StartTime = 0;

            Initialize();
        }

        public void Update()
        {
            if (!this.m_IsTimerRunning) return;

            float countdown = TimeRemaining();

            if (countdown > 0.0f) return;

            OnTimerEnds();
        }


        private void OnTimerRuns()
        {
            this.m_IsTimerRunning = true;
        }

        private void OnTimerEnds()
        {
            this.m_IsTimerRunning = false;

            if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();
        }


        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
            Initialize();
        }


        private void Initialize()
        {
            int propStartTime;
            if (TryGetStartTime(out propStartTime))
            {
                this.m_StartTime = propStartTime;
                Debug.Log("Initialize sets StartTime " + this.m_StartTime + " server time now: " + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());


                this.m_IsTimerRunning = TimeRemaining() > 0;

                if (this.m_IsTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();
            }
        }


        public float TimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.m_StartTime;
            return this.m_Countdown - timer / 1000f;
        }


        public bool TryGetStartTime(out int startTimestamp)
        {
            startTimestamp = PhotonNetwork.ServerTimestamp;

            object startTimeFromProps;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(m_CountdownStartTimeStr, out startTimeFromProps))
            {
                startTimestamp = (int)startTimeFromProps;
                return true;
            }

            return false;
        }


        public void SetStartTime()
        {
            int startTime = 0;
            bool wasSet = TryGetStartTime(out startTime);

            Hashtable props = new Hashtable
            {
                {m_CountdownStartTimeStr, (int)PhotonNetwork.ServerTimestamp}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);


            Debug.Log("Set Custom Props for Time: " + props.ToStringFull() + " wasSet: " + wasSet);
        }
    }
}
