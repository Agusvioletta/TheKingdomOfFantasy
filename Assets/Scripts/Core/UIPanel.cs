using UnityEngine;

namespace TKOF.Core
{
    /// <summary>
    /// HERENCIA (caso 1): clase base propia (no MonoBehaviour "pelado") de la
    /// que heredan todos los paneles de UI del juego. Encapsula el
    /// mostrar/ocultar y deja que cada panel hijo agregue su propio
    /// comportamiento (Show/Hide con OnShow/OnHide como hooks).
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        public bool IsVisible { get; private set; }

        public virtual void Show()
        {
            IsVisible = true;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnShow();
        }

        public virtual void Hide()
        {
            IsVisible = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            OnHide();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
