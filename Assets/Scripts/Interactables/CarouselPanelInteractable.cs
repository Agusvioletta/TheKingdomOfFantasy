using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    /// <summary>
    /// El panel del carrusel también es un IInteractable, pero no hereda de
    /// Collectible ya que no se "recoge", se "usa".
    /// </summary>
    public class CarouselPanelInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string requiredItemId = "ticket_atraccion";
        [SerializeField] private Animator carouselAnimator; 
        [SerializeField] private GameObject trapdoor;

        private bool _activated;

        public string InteractionPrompt =>
            GameManager.Instance.HasCollected(requiredItemId) ? "Pulsar E - Usar ticket" : "Falta el ticket";

        public void Interact()
        {
            if (_activated || !GameManager.Instance.HasCollected(requiredItemId)) return;
            _activated = true;
            if (carouselAnimator != null) carouselAnimator.SetTrigger("Activate");
            trapdoor.SetActive(true);
            EventManager.RaiseCarouselActivated();
        }
    }
}
