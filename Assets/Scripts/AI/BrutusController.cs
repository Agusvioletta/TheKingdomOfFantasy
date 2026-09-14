using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TKOF.Core;
using TKOF.Player;

namespace TKOF.AI
{
    /// <summary>
    /// Contexto del patrón Strategy: mantiene el estado actual (IAIState) y
    /// delega en él todo el comportamiento frame a frame. También expone los
    /// datos que las estrategias necesitan (velocidades, distancias, referencia
    /// al jugador).
    /// </summary>
    public class BrutusController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float patrolSpeed = 1.5f;
        [SerializeField] private float alertSpeed = 2.5f;
        [SerializeField] private float chaseSpeed = 4.5f;
        [SerializeField] private float searchSpeed = 2.0f;

        [Header("Detección")]
        [SerializeField] private float visionRadius = 25f;
        [SerializeField] private float visionRadiusWithFlashlight = 35f;
        [SerializeField] private float visionAngle = 60f;
        [SerializeField] private LayerMask visionObstacles;
        [SerializeField] private float captureDistance = 1.2f;

        [Header("Patrulla")]
        [SerializeField] private List<Transform> patrolWaypoints;
        [SerializeField] private float waitTimeAtPoint = 5f; // "con espera en cada uno" (GDD)
        [SerializeField] private float pointTolerance = 1.0f;

        // Cambiá esto a true si en algún momento necesitás volver a ver el
        // detalle de distancia/ángulo/patrulla frame a frame. Lo dejamos en
        // false para no inundar la Console con logs de bajo valor.
        public const bool VerboseLogging = false;

        [Header("Animación (opcional)")]
        [SerializeField] private Animator animator; // dejalo vacío si todavía no tenés modelo/animaciones

        private NavMeshAgent _agent;
        private Transform _player;
        private Flashlight _playerFlashlight;
        private PlayerHeartRate _playerHeartRate;
        private IAIState _currentState;

        // ESTRUCTURA DE DATOS: Queue<Transform>.
        // Los 6 puntos de patrulla se recorren en orden circular: se saca el
        // próximo punto (Dequeue) y se lo vuelve a encolar al final (Enqueue),
        // lo que da naturalmente un recorrido cíclico sin manejar índices a mano.
        private Queue<Transform> _patrolQueue = new Queue<Transform>();

        public NavMeshAgent Agent => _agent;
        public Transform PlayerTransform => _player;
        public float PatrolSpeed => patrolSpeed;
        public float AlertSpeed => alertSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float SearchSpeed => searchSpeed;
        public float CaptureDistance => captureDistance;
        public float WaitTimeAtPoint => waitTimeAtPoint;
        public float PointTolerance => pointTolerance;
        public Vector3 LastKnownPlayerPosition { get; private set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            foreach (var point in patrolWaypoints) _patrolQueue.Enqueue(point);
        }

        private void Start()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            _player = playerGo.transform;
            _playerFlashlight = playerGo.GetComponentInChildren<Flashlight>();
            _playerHeartRate = playerGo.GetComponent<PlayerHeartRate>();

            ChangeState(new PatrolState());
        }

        private float _lastDistanceLogTime;

        private void Update()
        {
            _currentState?.Tick(this);
            UpdatePlayerThreatPressure();

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

#if UNITY_EDITOR
            if (VerboseLogging && Time.unscaledTime - _lastDistanceLogTime > 0.5f)
            {
                _lastDistanceLogTime = Time.unscaledTime;
                Debug.Log($"[Brutus] Distancia al jugador: {distanceToPlayer:F2} (Capture Distance actual: {captureDistance:F2})");
            }
#endif

            // El contacto atrapa sin importar el estado actual de la IA (evita que
            // un cambio de estado justo al pegarse al jugador "salve" a Brutus de
            // capturar por estar a mitad de una transición).
            if (distanceToPlayer < captureDistance)
            {
                CapturePlayer();
            }

            if (animator != null) animator.SetFloat("Speed", _agent.velocity.magnitude);
        }

        public void ChangeState(IAIState newState)
        {
#if UNITY_EDITOR
            string from = _currentState?.GetType().Name ?? "(ninguno)";
            Debug.Log($"[Brutus] Cambio de estado: {from} -> {newState.GetType().Name}");
#endif
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);
        }

        public void GoToNextPatrolPoint()
        {
            if (_patrolQueue.Count == 0) return;
            var next = _patrolQueue.Dequeue();
            _patrolQueue.Enqueue(next); // vuelve al final: recorrido cíclico
            _agent.SetDestination(next.position);
        }

        /// <summary>Mira cuál es el próximo punto sin sacarlo de la Queue (no altera el orden).</summary>
        public Vector3 PeekNextPatrolPoint() =>
            _patrolQueue.Count > 0 ? _patrolQueue.Peek().position : transform.position;

        public bool CanSeePlayer()
        {
            float radius = (_playerFlashlight != null && _playerFlashlight.IsOn)
                ? visionRadiusWithFlashlight
                : visionRadius;

            Vector3 toPlayer = _player.position - transform.position;
            float distance = toPlayer.magnitude;

            if (distance > radius)
            {
                LogDebug($"Fuera de rango: distancia {distance:F1} > radio {radius:F1}");
                return false;
            }

            float angle = Vector3.Angle(transform.forward, toPlayer);
            if (angle > visionAngle * 0.5f)
            {
                LogDebug($"Fuera del cono: ángulo {angle:F1}° (máximo {visionAngle * 0.5f:F1}°)");
                return false;
            }

            if (Physics.Linecast(transform.position + Vector3.up, _player.position + Vector3.up,
                    visionObstacles))
            {
                LogDebug("Bloqueado por un obstáculo (capa Vision Obstacles)");
                return false;
            }

            LastKnownPlayerPosition = _player.position;
            LogDebug("¡TE VE! (dentro de rango, ángulo y sin obstáculos)");
            return true;
        }

        private float _lastLogTime;

        private void LogDebug(string message)
        {
#if UNITY_EDITOR
            if (!VerboseLogging) return;
            // Throttle: como esto se llama cada frame, logueamos como mucho 2 veces por segundo.
            if (Time.unscaledTime - _lastLogTime < 0.5f) return;
            _lastLogTime = Time.unscaledTime;
            Debug.Log($"[Brutus] {message}");
#endif
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, visionRadius);

            Vector3 forward = transform.forward * visionRadius;
            Quaternion leftRay = Quaternion.AngleAxis(-visionAngle * 0.5f, Vector3.up);
            Quaternion rightRay = Quaternion.AngleAxis(visionAngle * 0.5f, Vector3.up);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, leftRay * forward);
            Gizmos.DrawRay(transform.position, rightRay * forward);
        }

        private void UpdatePlayerThreatPressure()
        {
            if (_playerHeartRate == null) return;
            float distance = Vector3.Distance(transform.position, _player.position);
            float pressure = 1f - Mathf.Clamp01(distance / visionRadius);
            _playerHeartRate.SetThreatPressure(pressure);
        }

        public void CapturePlayer()
        {
#if UNITY_EDITOR
            Debug.Log($"[Brutus] CapturePlayer() ejecutado. agent.isStopped antes de esto: {_agent.isStopped}");
#endif
            _agent.isStopped = true;
            EventManager.RaisePlayerDetected();
            GameManager.Instance.OnPlayerCaught();
        }

        /// <summary>
        /// Llamado desde afuera (ej. AlarmTriggerZone) para forzar a Brutus a
        /// Alerta, sin que haya sido detección visual real. No pisa una
        /// Persecución en curso: si ya te está persiguiendo o buscando, una
        /// alarma de más no lo "relaja" a Alerta.
        /// </summary>
        public void ForceAlert(Vector3 suspiciousPosition)
        {
            if (_currentState is ChaseState) return;

            LastKnownPlayerPosition = suspiciousPosition;
            ChangeState(new AlertState());
        }
    }
}