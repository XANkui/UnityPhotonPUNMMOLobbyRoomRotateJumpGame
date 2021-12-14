using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGPUNGame2
{ 

	public class RotateSelf : MonoBehaviour
	{
		[SerializeField]
		private float m_Speed = 10;
		[SerializeField]
		private bool m_IsRotate = true;

		public bool IsRotate { get => m_IsRotate; set => m_IsRotate = value; }

		// Update is called once per frame
		void Update()
		{
			
            if (m_IsRotate && PhotonNetwork.IsMasterClient==true)
            {
				transform.Rotate(new Vector3(0,m_Speed*Time.deltaTime,0));
            }
		}
	}
}
