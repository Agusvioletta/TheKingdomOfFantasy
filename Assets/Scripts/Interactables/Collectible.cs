using UnityEngine;
using TKOF.Core;

namespace TKOF.Interactables
{
    /// <summary>
    /// HERENCIA: clase base propia para todo objeto coleccionable
    /// del mundo (diario, foto, ticket). Centraliza el registro en el
    /// GameManager y el highlight visual. Cada subclase define qué contenido
    /// muestra al leerse, y opcionalmente qué pasa después (OnCollected) 
    /// por defecto no pasa nada más, pero por ejemplo TicketItem la usa para
    /// desaparecer del mapa una vez levantado.
    /// </summary>
    public abstract class Collectible : MonoBehaviour, IInteractable, IHighlightable
    {
        [SerializeField] private string itemId;
        [SerializeField] private GameObject highlightVisual;

        public string InteractionPrompt => "Pulsar E";

        public void SetHighlighted(bool value)
        {
            if (highlightVisual != null) highlightVisual.SetActive(value);
        }

        public void Interact()
        {
            GameManager.Instance.RegisterCollected(itemId);
            ShowContent();
            OnCollected();
        }

        protected abstract void ShowContent();

        protected virtual void OnCollected() { }
    }
}