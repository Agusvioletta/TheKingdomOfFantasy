using UnityEngine;
using UnityEngine.UI;

namespace TKOF.Player
{
    /// <summary>
    /// Punto de mira fijo en el centro de la pantalla 
    /// Cambia de color cuando hay algo interactuable en la mira, así el
    /// jugador sabe tanto hacia dónde apuntar como cuándo puede usar "E".
    /// </summary>
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private InteractionController interactionController;
        [SerializeField] private Image crosshairImage;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color interactableColor = Color.yellow;

        private void Update()
        {
            if (crosshairImage == null || interactionController == null) return;

            crosshairImage.color = interactionController.HasInteractableInSight
                ? interactableColor
                : defaultColor;
        }
    }
}
