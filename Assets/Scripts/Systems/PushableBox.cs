using UnityEngine;

namespace TKOF.Systems
{
    /// <summary>
    /// Ejemplo independiente de Colisión con Rigidbody (requisito de Motores
    /// de Desarrollo 1, punto 7). Caja física empujable: al chocar contra
    /// algo, cambia de color un instante. Demuestra la diferencia con el
    /// trigger de al lado — acá los objetos chocan de verdad, no se
    /// atraviesan.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PushableBox : MonoBehaviour
    {
        [SerializeField] private Color impactColor = Color.red;
        [SerializeField] private float colorResetDelay = 1.5f;

        private Renderer _renderer;
        private Color _originalColor;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _originalColor = _renderer.material.color;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"[PushableBox] Choqué contra {collision.gameObject.name}");
            _renderer.material.color = impactColor;
            Invoke(nameof(ResetColor), colorResetDelay);
        }

        private void ResetColor() => _renderer.material.color = _originalColor;
    }
}