using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    public class TicketItem : Collectible
    {
        protected override void ShowContent() => UIReadingPanel.Instance.ShowText("Ticket de atracción obtenido.");

        protected override void OnCollected() => gameObject.SetActive(false);
    }
}