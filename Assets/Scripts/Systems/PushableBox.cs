using UnityEngine;

namespace TKOF.Systems
{
    /// <summary>
    /// Caja física empujable: al chocar contra algo,
    /// cambia de color un instante. acá los objetos chocan de verdad, no se
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