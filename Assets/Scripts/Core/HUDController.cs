using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Core
{
    /// <summary>
    /// Se suscribe Observer a los eventos de BPM, batería de linterna y
    /// stamina, y actualiza el texto/color/barra en pantalla y solo
    /// escucha el EventManager.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Text bpmText;
        [SerializeField] private Image bpmFillBar;
        [SerializeField] private Image batteryFillBar;
        [SerializeField] private Image staminaFillBar;

        [SerializeField] private Color normalColor = new Color(0.3f, 0.8f, 0.3f);
        [SerializeField] private Color alertColor = new Color(0.9f, 0.85f, 0.2f);
        [SerializeField] private Color dangerColor = new Color(0.95f, 0.5f, 0.1f);
        [SerializeField] private Color criticalColor = new Color(0.9f, 0.1f, 0.1f);

        private void OnEnable()
        {
            EventManager.OnBpmChanged += HandleBpmChanged;
            EventManager.OnFlashlightBatteryChanged += HandleBatteryChanged;
            EventManager.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            EventManager.OnBpmChanged -= HandleBpmChanged;
            EventManager.OnFlashlightBatteryChanged -= HandleBatteryChanged;
            EventManager.OnStaminaChanged -= HandleStaminaChanged;
        }

        private void HandleBpmChanged(int bpm)
        {
            if (bpmText != null) bpmText.text = $"{bpm} BPM";
            if (bpmFillBar == null) return;

            bpmFillBar.fillAmount = Mathf.InverseLerp(60, 200, bpm);
            bpmFillBar.color = bpm switch
            {
                <= 90 => normalColor,
                <= 130 => alertColor,
                <= 160 => dangerColor,
                _ => criticalColor
            };
        }

        private void HandleBatteryChanged(float percent)
        {
            if (batteryFillBar != null) batteryFillBar.fillAmount = percent;
        }

        private void HandleStaminaChanged(float percent)
        {
            if (staminaFillBar != null) staminaFillBar.fillAmount = percent;
        }
    }
}