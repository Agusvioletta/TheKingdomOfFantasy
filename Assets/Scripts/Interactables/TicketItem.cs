using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    public class TicketItem : Collectible
    {
        protected override void ShowContent() => UIReadingPanel.Instance.ShowText("Ticket de atracción obtenido.");

        // Solo el ticket desaparece del mapa: ya no es algo para "leer de
        // nuevo" como el diario o la foto, es algo que se usa después (el
        // carrusel lo va a pedir por su Item Id).
        protected override void OnCollected() => gameObject.SetActive(false);
    }
}