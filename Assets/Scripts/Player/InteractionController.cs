using UnityEngine;
using UnityEngine.InputSystem;
using TKOF.Core;
using TKOF.Interactables;

namespace TKOF.Player
{
    /// <summary>
    /// Lanza un raycast frontal y detecta cualquier IInteractable, sin
    /// importar de qué clase concreta sea (Diary, Photograph, TicketItem,
    /// CarouselPanelInteractable, TrapdoorInteractable...). Esto es lo que
    /// aprovecha la interfaz: un único camino de código para todo el sistema
    /// de interacción. El highlight usa otra interfaz aparte (IHighlightable)
    /// porque no todo interactuable necesita iluminarse.
    ///
    /// El origen del rayo es el CameraPivot (pegado al personaje, a la
    /// altura del hombro) y no la Camera real, porque en tercera persona la
    /// Camera está desplazada varias unidades hacia atrás (shoulderOffset) y
    /// eso arruinaría el cálculo de "interactionRange".
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Transform aimOrigin; // arrastrá el CameraPivot acá
        [SerializeField] private float interactionRange = 2.5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private InputActionReference interactAction;

        private IInteractable _current;
        private IHighlightable _currentHighlightable;

        private void OnEnable() => interactAction.action.Enable();
        private void OnDisable() => interactAction.action.Disable();

        private void Update()
        {
            DetectInteractable();

            if (!interactAction.action.WasPerformedThisFrame()) return;

            bool readingPanelOpen = UIReadingPanel.Instance != null && UIReadingPanel.Instance.IsVisible;

            if (readingPanelOpen)
            {
                UIReadingPanel.Instance.Hide();
                return; // este mismo E no debe además abrir algo nuevo
            }

            _current?.Interact();
        }

        private void DetectInteractable()
        {
            IInteractable found = null;
            IHighlightable foundHighlightable = null;

#if UNITY_EDITOR
            Debug.DrawRay(aimOrigin.position, aimOrigin.forward * interactionRange, Color.red);
#endif

            if (Physics.Raycast(aimOrigin.position, aimOrigin.forward,
                    out RaycastHit hit, interactionRange, interactableLayer))
            {
                found = hit.collider.GetComponent<IInteractable>();
                foundHighlightable = hit.collider.GetComponent<IHighlightable>();
            }

            if (foundHighlightable != _currentHighlightable)
            {
                _currentHighlightable?.SetHighlighted(false);
                foundHighlightable?.SetHighlighted(true);
                _currentHighlightable = foundHighlightable;
            }

            _current = found;
            // TODO: mostrar/ocultar prompt de UI con _current?.InteractionPrompt
        }
    }
}