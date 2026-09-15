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

            // Solo texto: centrado en el panel.
            _textRect.anchorMin = CenterAnchor;
            _textRect.anchorMax = CenterAnchor;
            _textRect.anchoredPosition = Vector2.zero;

            Show();
        }

        public void ShowImage(Sprite sprite, string caption)
        {
            bodyText.gameObject.SetActive(true);
            bodyImage.gameObject.SetActive(true);
            bodyImage.sprite = sprite;
            bodyText.text = caption;

            // Con foto: el texto vuelve a la posición "abajo" 
            _textRect.anchorMin = _textAnchorMinWithImage;
            _textRect.anchorMax = _textAnchorMaxWithImage;
            _textRect.anchoredPosition = _textAnchoredPosWithImage;

            Show();
        }
    }
}