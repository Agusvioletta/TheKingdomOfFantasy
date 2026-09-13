using UnityEngine;
using UnityEngine.InputSystem;
using TKOF.Core;

namespace TKOF.Player
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private Light lightSource;
        [SerializeField] private float maxBattery = 100f;
        [SerializeField] private float drainPerSecond = 3f;
        [SerializeField] private InputActionReference toggleAction;

        public bool IsOn { get; private set; }
        public float BatteryPercent => _battery / maxBattery;

        private float _battery;

        private void OnEnable() => toggleAction.action.Enable();
        private void OnDisable() => toggleAction.action.Disable();

        private void Start()
        {
            _battery = maxBattery;
            SetOn(false);
        }

        private void Update()
        {
            if (toggleAction.action.WasPerformedThisFrame()) Toggle();

            if (IsOn)
            {
                _battery = Mathf.Max(0f, _battery - drainPerSecond * Time.deltaTime);
                EventManager.RaiseFlashlightBattery(BatteryPercent);
                if (_battery <= 0f) SetOn(false);
            }
        }

        private void Toggle() => SetOn(!IsOn);

        private void SetOn(bool value)
        {
            IsOn = value && _battery > 0f;
            lightSource.enabled = IsOn;
            EventManager.RaiseFlashlightToggled();
        }
    }
}
