using System;

namespace TKOF.Core
{
    /// <summary>
    /// PATRÓN: OBSERVER.
    /// Canal central de eventos del juego. Los BPM, Brutus,
    /// coleccionables disparan estos eventos sin conocer quién los escucha.
    /// Los HUD, AudioManager, GameManager se registran y
    /// reaccionan. Esto desacopla sistemas que de otra forma tendrían
    /// referencias cruzadas directas.
    /// </summary>
    public static class EventManager
    {
        public static event Action<int> OnBpmChanged;
        public static event Action<string, int> OnItemCollected; 
        public static event Action OnFlashlightToggled;
        public static event Action<float> OnFlashlightBatteryChanged;
        public static event Action<float> OnStaminaChanged;
        public static event Action OnPlayerDetectedByEnemy;
        public static event Action OnPlayerDefeated;
        public static event Action OnPlayerVictorious;
        public static event Action OnCarouselActivated;

        public static void RaiseBpmChanged(int bpm) => OnBpmChanged?.Invoke(bpm);
        public static void RaiseItemCollected(string id, int total) => OnItemCollected?.Invoke(id, total);
        public static void RaiseFlashlightToggled() => OnFlashlightToggled?.Invoke();
        public static void RaiseFlashlightBattery(float value) => OnFlashlightBatteryChanged?.Invoke(value);
        public static void RaiseStaminaChanged(float value) => OnStaminaChanged?.Invoke(value);
        public static void RaisePlayerDetected() => OnPlayerDetectedByEnemy?.Invoke();
        public static void RaisePlayerDefeated() => OnPlayerDefeated?.Invoke();
        public static void RaisePlayerVictorious() => OnPlayerVictorious?.Invoke();
        public static void RaiseCarouselActivated() => OnCarouselActivated?.Invoke();
    }
}