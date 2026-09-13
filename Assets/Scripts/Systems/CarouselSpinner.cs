using System.Collections;
using UnityEngine;
using TKOF.Core;

namespace TKOF.Systems
{
    /// <summary>
    /// Alternativa simple a usar un Animator: hace girar el carrusel durante
    /// un tiempo cuando se activa, sin necesitar clips de animación. Se
    /// suscribe al evento del Observer en vez de que CarouselPanelInteractable
    /// lo llame directo — así el carrusel no necesita saber que esto existe.
    /// </summary>
    public class CarouselSpinner : MonoBehaviour
    {
        [SerializeField] private float durationSeconds = 30f;
        [SerializeField] private float degreesPerSecond = 40f;

        private void OnEnable() => EventManager.OnCarouselActivated += StartSpin;
        private void OnDisable() => EventManager.OnCarouselActivated -= StartSpin;

        private void StartSpin() => StartCoroutine(SpinRoutine());

        private IEnumerator SpinRoutine()
        {
            float elapsed = 0f;
            while (elapsed < durationSeconds)
            {
                transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
