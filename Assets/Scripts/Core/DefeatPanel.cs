using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    public class DefeatPanel : UIPanel
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;

        private void OnEnable() => EventManager.OnPlayerDefeated += Show;
        private void OnDisable() => EventManager.OnPlayerDefeated -= Show;

        private void Awake()
        {
            retryButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                Debug.Log("[DefeatPanel] Botón Reintentar clickeado");
#endif
                GameManager.Instance.RestartGameplay();
            });
            menuButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                Debug.Log("[DefeatPanel] Botón Menú clickeado");
#endif
                GameManager.Instance.ReturnToMenu();
            });
        }

        private void Start() => Hide(); // arranca oculto

        protected override void OnShow()
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        protected override void OnHide()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}