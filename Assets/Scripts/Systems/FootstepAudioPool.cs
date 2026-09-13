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

        private void Awake()
        {
            _pool = new ObjectPool<AudioSource>(audioSourcePrefab, transform, poolSize);
        }

        public void PlayAt(Vector3 position, AudioClip clip, float volume = 1f)
        {
            var source = _pool.Get();
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.Play();
            StartCoroutine(ReturnWhenDone(source));
        }

        private System.Collections.IEnumerator ReturnWhenDone(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);
            _pool.Return(source);
        }
    }
}
