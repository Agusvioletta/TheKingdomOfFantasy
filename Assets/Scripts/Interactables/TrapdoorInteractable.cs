using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
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