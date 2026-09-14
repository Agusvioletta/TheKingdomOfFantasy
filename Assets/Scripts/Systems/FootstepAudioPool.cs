using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKOF.Core;

namespace TKOF.Systems
{
    /// <summary>
    /// Uso concreto del ObjectPool&lt;T&gt; genérico: en vez de crear y destruir
    /// un AudioSource por cada paso/cadena de Brutus (costoso y genera
    /// garbage), se piden y devuelven instancias reutilizables.
    /// </summary>
    public class FootstepAudioPool : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int poolSize = 8;

        private ObjectPool<AudioSource> _pool;

        // Si un AudioSource se reutiliza antes de que termine su devolución
        // pendiente del uso anterior, cancelamos esa corrutina vieja para que
        // no lo desactive a mitad de la nueva reproducción.
        private readonly Dictionary<AudioSource, Coroutine> _pendingReturns = new Dictionary<AudioSource, Coroutine>();

        private void Awake()
        {
            _pool = new ObjectPool<AudioSource>(audioSourcePrefab, transform, poolSize);
        }

        public void PlayAt(Vector3 position, AudioClip clip, float volume = 1f)
        {
            var source = _pool.Get();

            if (_pendingReturns.TryGetValue(source, out var oldRoutine) && oldRoutine != null)
            {
                StopCoroutine(oldRoutine);
            }

            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.gameObject.SetActive(true);
            source.Play();

            _pendingReturns[source] = StartCoroutine(ReturnWhenDone(source));
        }

        private IEnumerator ReturnWhenDone(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);
            _pendingReturns.Remove(source);
            _pool.Return(source);
        }
    }
}