using UnityEngine;
using UnityEngine.InputSystem;
using TKOF.Core;

namespace TKOF.Player
{
    /// <summary>
    /// Límite de sprint: se gasta corriendo, se regenera al no correr (con un
    /// pequeño delay antes de empezar a regenerar). PlayerController le
    /// pregunta CanSprint antes de aplicar la velocidad de sprint, así que
    /// aunque mantenga Shift apretado, si no hay stamina no corrés.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float drainPerSecond = 20f;
        [SerializeField] private float regenPerSecond = 15f;
        [SerializeField] private float regenDelay = 1f; // segundos sin correr antes de regenerar
        [SerializeField] private InputActionReference sprintAction;

        private float _stamina;
        private float _regenCooldown;

        public float StaminaPercent => _stamina / maxStamina;
        public bool CanSprint => _stamina > 0f;

        private void OnEnable() => sprintAction.action.Enable();
        private void OnDisable() => sprintAction.action.Disable();

        private void Start() => _stamina = maxStamina;

        private void Update()
        {
            bool wantsSprint = sprintAction.action.IsPressed();

            if (wantsSprint && CanSprint)
            {
                _stamina = Mathf.Max(0f, _stamina - drainPerSecond * Time.deltaTime);
                _regenCooldown = regenDelay;
            }
            else if (_regenCooldown > 0f)
            {
                _regenCooldown -= Time.deltaTime;
            }
            else
            {
                _stamina = Mathf.Min(maxStamina, _stamina + regenPerSecond * Time.deltaTime);
            }

            EventManager.RaiseStaminaChanged(StaminaPercent);
        }
    }
}
