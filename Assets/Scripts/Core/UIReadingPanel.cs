using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    /// <summary>
    /// Panel de lectura (diarios/fotos).
    /// </summary>
    public class UIReadingPanel : UIPanel
    {
        public static UIReadingPanel Instance { get; private set; }

        [SerializeField] private Text bodyText;
        [SerializeField] private Image bodyImage;

        private RectTransform _textRect;
        private Vector2 _textAnchorMinWithImage;
        private Vector2 _textAnchorMaxWithImage;
        private Vector2 _textAnchoredPosWithImage;

        private static readonly Vector2 CenterAnchor = new Vector2(0.5f, 0.5f);

        private void Awake()
        {
            Instance = this;
            _textRect = bodyText.rectTransform;
            _textAnchorMinWithImage = _textRect.anchorMin;
            _textAnchorMaxWithImage = _textRect.anchorMax;
            _textAnchoredPosWithImage = _textRect.anchoredPosition;
        }

        private void Start() => Hide(); 

        public void ShowText(string text)
        {
            bodyImage.gameObject.SetActive(false);
            bodyText.gameObject.SetActive(true);
            bodyText.text = text;
            bodyText.alignment = TextAnchor.MiddleCenter;

            // Solo texto: ocupa todo el panel, centrado.
            _textRect.anchorMin = Vector2.zero;
            _textRect.anchorMax = Vector2.one;
            _textRect.offsetMin = Vector2.zero;
            _textRect.offsetMax = Vector2.zero;

            Show();
        }

        public void ShowImage(Sprite sprite, string caption)
        {
            bodyImage.gameObject.SetActive(true);
            bodyText.gameObject.SetActive(true);
            bodyImage.sprite = sprite;
            bodyText.text = caption;
            bodyText.alignment = TextAnchor.MiddleCenter;

            // Imagen: centrada, ocupando la parte de arriba
            var imgRect = bodyImage.rectTransform;
            imgRect.anchorMin = new Vector2(0.1f, 0.25f);
            imgRect.anchorMax = new Vector2(0.9f, 0.85f);
            imgRect.offsetMin = Vector2.zero;
            imgRect.offsetMax = Vector2.zero;

            // Texto: franja fija abajo
            _textRect.anchorMin = new Vector2(0.05f, 0f);
            _textRect.anchorMax = new Vector2(0.95f, 0.2f);
            _textRect.offsetMin = Vector2.zero;
            _textRect.offsetMax = Vector2.zero;

            Show();
        }
    }
}