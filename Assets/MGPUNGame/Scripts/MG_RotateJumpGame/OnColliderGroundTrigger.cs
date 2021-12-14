using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGPUNGame2 { 

	public class OnColliderGroundTrigger : MonoBehaviour
	{
        [SerializeField]
        private int m_Count=0;
        public bool IsGround {
            get { return m_Count>0; }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                m_Count++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                m_Count--;
            }
        }
    }
}
