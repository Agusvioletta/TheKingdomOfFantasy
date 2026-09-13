using UnityEngine;
using TKOF.Core;

namespace TKOF.Systems
{
    /// <summary>Condición de victoria: el jugador entra a la trampilla ya activada.</summary>
    [RequireComponent(typeof(Collider))]
    public class TrapdoorTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance.OnPlayerReachedTrapdoor();
        }
    }
}
