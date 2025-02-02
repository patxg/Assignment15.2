using System.Collections;
using Data;
using Helpers;
using UnityEngine;

namespace Controllers
{
//This controller works as a Fabric for effects spawning.
    public class FxSpawner : MonoBehaviour
    {
        private PoolOfObjects<ParticleSystem> _killFxPool;
        private GameObject _fxParent;
        private float _fxLifeDuration;
        private float _fxColorRandomizer;

        public void Initialize(GameObject killFxPrefab, float fxLifeDuration, float fxColorRandomizer)
        {
            _fxLifeDuration = fxLifeDuration;
            _fxColorRandomizer = fxColorRandomizer;
            _fxParent = new GameObject("FxParent");
            _killFxPool = new PoolOfObjects<ParticleSystem>(killFxPrefab, _fxParent.transform);

            GameEvents.OnManDead += SpawnKillFx;
        }

        public void DeInitialize()
        {
            GameEvents.OnManDead -= SpawnKillFx;
            Destroy(_fxParent.gameObject);
        }

        private void SpawnKillFx(Vector3 spawnPos, Color bloodColor)
        {
            spawnPos.y = 0.01f;
            var fx = _killFxPool.Get();
            foreach (var p in fx.gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                var maimParam = p.main;
                var colorShift = Random.Range(1 - _fxColorRandomizer, 1 + _fxColorRandomizer);
                bloodColor.r *= colorShift;
                bloodColor.g *= colorShift;
                bloodColor.b *= colorShift;
                maimParam.startColor = bloodColor;
            }

            fx.transform.position = spawnPos;
            fx.gameObject.SetActive(true);

            StartCoroutine(HideEffect(fx));
        }

        private IEnumerator HideEffect(ParticleSystem fx)
        {
            yield return new WaitForSeconds(_fxLifeDuration);
            fx.gameObject.SetActive(false);
            _killFxPool.Put(fx);
        }
    }
}