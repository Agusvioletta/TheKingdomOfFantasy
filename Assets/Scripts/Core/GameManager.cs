using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TKOF.Core
{
    public enum GameState { MainMenu, Playing, Victory, Defeat, Paused }

    /// <summary>
    /// PATRÓN: SINGLETON.
    /// Único punto de acceso al estado global de la partida (estado actual,
    /// coleccionables recolectados). Evita pasar referencias por el Inspector
    /// entre escenas y garantiza que solo exista una instancia viva.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        /// <summary>
        /// Si todavía no existe en la escena, se crea uno
        /// solo automáticamente la primera vez que alguien lo pide así no
        /// hace falta poner el Bootstrap a mano en cada escena para probar.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GameManager (auto)");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [SerializeField] private string gameplaySceneName = "AvenidaPrincipal";
        [SerializeField] private string menuSceneName = "MainMenu";

        // ESTRUCTURA DE DATOS: Dictionary.
        // Se usa para llevar el registro de qué coleccionable (identificado por
        // un id único como diario_01) ya fue recolectado. Un Dictionary da
        // acceso O(1) por clave, ideal para ver si ya lo recogiste sin recorrer listas.
        private readonly Dictionary<string, bool> _collectedItems = new Dictionary<string, bool>();

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegisterCollected(string itemId)
        {
            _collectedItems[itemId] = true;
            // Observer: avisamos a quien esté escuchando (HUD, UI de inventario)
            EventManager.RaiseItemCollected(itemId, _collectedItems.Count);
        }

        public bool HasCollected(string itemId) =>
            _collectedItems.TryGetValue(itemId, out var value) && value;

        public void StartGame()
        {
            Time.timeScale = 1f; 
            CurrentState = GameState.Playing;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void OnPlayerCaught()
        {
#if UNITY_EDITOR
            Debug.Log($"[GameManager] OnPlayerCaught() llamado. CurrentState en este momento: {CurrentState}");
#endif
            if (CurrentState == GameState.Victory || CurrentState == GameState.Defeat) return;
            CurrentState = GameState.Defeat;
            EventManager.RaisePlayerDefeated();
        }

        public void OnPlayerReachedTrapdoor()
        {
            if (CurrentState == GameState.Victory || CurrentState == GameState.Defeat) return;
            CurrentState = GameState.Victory;
            EventManager.RaisePlayerVictorious();
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            CurrentState = GameState.MainMenu;
            _collectedItems.Clear();
            SceneManager.LoadScene(menuSceneName);
        }

        public void RestartGameplay()
        {
#if UNITY_EDITOR
            Debug.Log($"[GameManager] RestartGameplay() llamado. CurrentState antes: {CurrentState}");
#endif
            Time.timeScale = 1f;
            CurrentState = GameState.Playing;
#if UNITY_EDITOR
            Debug.Log($"[GameManager] CurrentState reseteado a: {CurrentState}");
#endif
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}