using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    public class ControlsPanel : UIPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Text controlsText;

        private void Awake()
        {
            backButton.onClick.AddListener(Hide);

            controlsText.text =
                "MOVIMIENTO\n" +
                "W A S D - Moverse\n" +
                "Shift - Correr\n" +
                "Ctrl - Agacharse\n\n" +
                "CÁMARA\n" +
                "Mouse - Mirar\n\n" +
                "ACCIONES\n" +
                "E - Interactuar\n" +
                "Click derecho - Encender/Apagar linterna\n\n" ;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            var rect = GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            gameObject.SetActive(false);
        }
    }
}