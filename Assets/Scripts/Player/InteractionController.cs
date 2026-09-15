using UnityEngine;
using UnityEngine.InputSystem;
using TKOF.Core;
using TKOF.Interactables;

namespace TKOF.Player
{
    /// <summary>
    /// Lanza un raycast frontal y detecta cualquier IInteractable, sin
    /// importar de qué clase concreta sea (Diary, Photograph, TicketItem,
    /// CarouselPanelInteractable, TrapdoorInteractable.). Esto es lo que
    /// aprovecha la interfaz: un único camino de código para todo el sistema
    /// de interacción. El highlight usa otra interfaz aparte (IHighlightable)
    /// porque no todo interactuable necesita iluminarse.
    ///
    /// El origen del rayo es el CameraPivot y no la Camera real, porque en tercera persona la
    /// Camera está desplazada varias unidades hacia atrás (shoulderOffset) y
    /// eso arruinaría el cálculo de "interactionRange".
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Transform aimOrigin; 
        [SerializeField] private float interactionRange = 2.5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private InputActionReference interactAction;

        private IInteractable _current;
        private IHighlightable _currentHighlightable;

        public bool HasInteractableInSight => _current != null;

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
                return; 
            }

            _current?.Interact();
        }

        private void DetectInteractable()
        {
            IInteractable found = null;
            IHighlightable foundHighlightable = null;

            Camera cam = Camera.main;
            float extraDist = Vector3.Distance(cam.transform.position, aimOrigin.position);

        #if UNITY_EDITOR
            Debug.DrawRay(cam.transform.position, cam.transform.forward * (interactionRange + extraDist), Color.red);
        #endif

            if (Physics.Raycast(cam.transform.position, cam.transform.forward,
                    out RaycastHit hit, interactionRange + extraDist, interactableLayer))
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
        } 
    }
}