using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    public class VictoryPanel : UIPanel
    {
        [SerializeField] private Button menuButton;

        private void OnEnable() => EventManager.OnPlayerVictorious += Show;
        private void OnDisable() => EventManager.OnPlayerVictorious -= Show;

        private void Awake() => menuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());

        private void Start() => Hide(); // arranca oculto, solo se muestra cuando se dispara el evento

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