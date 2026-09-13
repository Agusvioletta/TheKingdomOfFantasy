using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    /// <summary>
    /// Condición de victoria por interacción directa (E), en vez de un
    /// trigger de física — mismo mecanismo ya probado con los coleccionables
    /// y el carrusel, así que no hay un sistema de detección nuevo que
    /// depurar.
    /// </summary>
    public class TrapdoorInteractable : MonoBehaviour, IInteractable, IHighlightable
    {
        [SerializeField] private GameObject highlightVisual;

        public string InteractionPrompt => "Pulsar E - Bajar por la trampilla";

        public void SetHighlighted(bool value)
        {
            if (highlightVisual != null) highlightVisual.SetActive(value);
        }

        public void Interact()
        {
            GameManager.Instance.OnPlayerReachedTrapdoor();
        }
    }
}