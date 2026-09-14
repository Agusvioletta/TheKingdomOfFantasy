using UnityEngine;
using UnityEngine.AI;

namespace TKOF.AI
{
    /// <summary>
    /// INTERFAZ (caso 2) + PATRÓN: STRATEGY.
    /// Cada estado de la IA (Patrulla, Alerta, Persecución, Búsqueda) es una
    /// estrategia intercambiable que implementa el mismo contrato. Brutus
    /// (el "contexto") solo conoce IAIState y llama Tick() cada frame; no
    /// tiene un switch gigante de estados, sino que delega el comportamiento
    /// en el objeto-estrategia activo y lo reemplaza cuando corresponde.
    /// </summary>
    public interface IAIState
    {
        void Enter(BrutusController brutus);
        void Tick(BrutusController brutus);
        void Exit(BrutusController brutus);
    }

    public class PatrolState : IAIState
    {
        private bool _isWaiting;
        private float _waitTimer;
        private float _lastLogTime;

        public void Enter(BrutusController b)
        {
            b.Agent.speed = b.PatrolSpeed;
            b.Agent.SetDestination(b.PeekNextPatrolPoint());
            _isWaiting = false;
            _waitTimer = 0f;
        }

        public void Tick(BrutusController b)
        {
            if (b.CanSeePlayer())
            {
                b.ChangeState(new ChaseState());
                return;
            }

#if UNITY_EDITOR
            if (BrutusController.VerboseLogging && Time.unscaledTime - _lastLogTime > 0.5f)
            {
                _lastLogTime = Time.unscaledTime;
                Debug.Log($"[Patrol] esperando={_isWaiting} timer={_waitTimer:F1}/{b.WaitTimeAtPoint:F1} " +
                          $"pathPending={b.Agent.pathPending} remainingDistance={b.Agent.remainingDistance:F2} " +
                          $"tolerancia={b.PointTolerance:F2} hasPath={b.Agent.hasPath}");
            }
#endif

            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= b.WaitTimeAtPoint)
                {
                    _isWaiting = false;
                    _waitTimer = 0f;
                    b.GoToNextPatrolPoint();
                }
                return;
            }

            if (!b.Agent.pathPending && b.Agent.remainingDistance <= b.PointTolerance)
            {
                _isWaiting = true; // llegó al punto: espera acá antes de seguir
            }
        }

        public void Exit(BrutusController b) { }
    }

    public class AlertState : IAIState
    {
        private float _timer;

        public void Enter(BrutusController b)
        {
            _timer = 0f;
            b.Agent.speed = b.AlertSpeed;
            b.Agent.SetDestination(b.LastKnownPlayerPosition);
        }

        public void Tick(BrutusController b)
        {
            if (b.CanSeePlayer())
            {
                b.ChangeState(new ChaseState());
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= 15f) b.ChangeState(new PatrolState());
        }

        public void Exit(BrutusController b) { }
    }

    public class ChaseState : IAIState
    {
        public void Enter(BrutusController b) => b.Agent.speed = b.ChaseSpeed;

        public void Tick(BrutusController b)
        {
            if (!b.CanSeePlayer())
            {
                b.ChangeState(new SearchState());
                return;
            }

            b.Agent.SetDestination(b.PlayerTransform.position);
        }

        public void Exit(BrutusController b) { }
    }

    /// <summary>
    /// A diferencia de la versión anterior (que solo iba una vez al último
    /// punto conocido y esperaba ahí), esta deambula alrededor de esa
    /// posición mientras busca — más creíble, y toma la idea del script
    /// viejo que encontraste.
    /// </summary>
    public class SearchState : IAIState
    {
        private const float WanderRadius = 5f;
        private float _timer;

        public void Enter(BrutusController b)
        {
            _timer = 0f;
            b.Agent.speed = b.SearchSpeed;
            b.Agent.SetDestination(b.LastKnownPlayerPosition);
        }

        public void Tick(BrutusController b)
        {
            if (b.CanSeePlayer())
            {
                b.ChangeState(new ChaseState());
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= 20f)
            {
                b.ChangeState(new PatrolState());
                return;
            }

            if (!b.Agent.pathPending && b.Agent.remainingDistance <= b.PointTolerance)
            {
                Vector3 randomPoint = b.LastKnownPlayerPosition + Random.insideUnitSphere * WanderRadius;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, WanderRadius, NavMesh.AllAreas))
                {
                    b.Agent.SetDestination(hit.position);
                }
            }
        }

        public void Exit(BrutusController b) { }
    }
}