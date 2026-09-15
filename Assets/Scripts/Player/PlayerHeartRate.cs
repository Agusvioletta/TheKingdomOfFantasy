using UnityEngine;
using UnityEngine.InputSystem;
using TKOF.Core;

namespace TKOF.Player
{
    public enum HeartRateState { Normal, Alert, Danger, Critical }

    /// <summary>
    /// Sube con el sprint y con la cercanía a Brutus
    /// (el BrutusController llama a AddThreatPressure), baja sola con el
    /// tiempo. Cada vez que cambia, publica el evento OnBpmChanged (Observer)
    /// para que el HUD, sin acoplarse a esta clase, actualice color y número.
    /// </summary>
    public class PlayerHeartRate : MonoBehaviour
    {
        [SerializeField] private int minBpm = 60;
        [SerializeField] private int maxBpm = 200;
        [SerializeField] private float restingDecayPerSecond = 8f;
        [SerializeField] private float sprintRisePerSecond = 15f;
        [SerializeField] private float breathingCalmPerSecond = 30f;

        [Header("Input Actions")]
        [SerializeField] private InputActionReference sprintAction;
        [SerializeField] private InputActionReference breatheAction;

        private float _bpm;
        private float _threatPressure; 

        public HeartRateState CurrentState { get; private set; } = HeartRateState.Normal;
        public int CurrentBpm => Mathf.RoundToInt(_bpm);

        private void OnEnable()
        {
            sprintAction.action.Enable();
            breatheAction.action.Enable();
        }

        private void OnDisable()
        {
            sprintAction.action.Disable();
            breatheAction.action.Disable();
        }

        private void Start() => _bpm = minBpm;

        private void Update()
        {
            bool sprinting = sprintAction.action.IsPressed();
            bool calming = breatheAction.action.IsPressed();

            float delta = -restingDecayPerSecond;
            if (sprinting) delta += sprintRisePerSecond;
            delta += _threatPressure * sprintRisePerSecond * 1.5f;
            if (calming) delta -= breathingCalmPerSecond;

            _bpm = Mathf.Clamp(_bpm + delta * Time.deltaTime, minBpm, maxBpm);
            UpdateState();
        }

        public void SetThreatPressure(float value) => _threatPressure = Mathf.Clamp01(value);

        private void UpdateState()
        {
            var previous = CurrentState;
            CurrentState = _bpm switch
            {
                <= 90 => HeartRateState.Normal,
                <= 130 => HeartRateState.Alert,
                <= 160 => HeartRateState.Danger,
                _ => HeartRateState.Critical
            };

            EventManager.RaiseBpmChanged(CurrentBpm);
            if (CurrentState != previous)
            {
            }
        }
    }
}
