using UnityEngine;

namespace TKOF.Core
{
    /// <summary>
    /// HERENCIA: clase base propia (no solo MonoBehaviour) de la
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
#if UNITY_EDITOR
            Debug.Log($"[{GetType().Name}] Show() ejecutado (frame {Time.frameCount})", this);
#endif
            IsVisible = true;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnShow();
        }

        public virtual void Hide()
        {
#if UNITY_EDITOR
            Debug.Log($"[{GetType().Name}] Hide() ejecutado (frame {Time.frameCount})", this);
#endif
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