using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGPUNGame2 { 

	public class OnAITriggerShortRotationCylinder : MonoBehaviour
	{
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("AI"))
            {
                other.GetComponent<AI>()?.JumpObstacle();
            }
        }
    }
}
