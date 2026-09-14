using UnityEngine;
using TKOF.AI;

namespace TKOF.Systems
{
    /// <summary>
    /// Ejemplo de Trigger (requisito de Motores de Desarrollo 1, punto 7),
    /// integrado como mecánica real de sigilo: al entrar el jugador, prende
    /// una luz roja Y fuerza a Brutus a estado de Alerta hacia esta posición
    /// — como si algo (una trampa, un sensor) delatara al jugador.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AlarmTriggerZone : MonoBehaviour
    {
        [SerializeField] private Light alarmLight;
        [SerializeField] private BrutusController brutus;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (alarmLight != null) alarmLight.gameObject.SetActive(true);
            if (brutus != null) brutus.ForceAlert(transform.position);

            Debug.Log("[AlarmTriggerZone] El jugador entró a la zona de alarma — Brutus fue alertado.");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (alarmLight != null) alarmLight.gameObject.SetActive(false);
            Debug.Log("[AlarmTriggerZone] El jugador salió de la zona de alarma.");
        }
    }
}