using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGPUNGame2 { 

	public class OnDieFallPlaneTrigger : MonoBehaviour
	{
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>()?.ToBackSpawnPostion();
            }else if (other.CompareTag("AI"))
            {
                other.GetComponent<AI>()?.ToBackSpawnPostion();
            }
        }
    }
}
