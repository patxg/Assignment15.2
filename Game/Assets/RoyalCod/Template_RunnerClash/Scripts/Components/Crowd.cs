using System.Collections.Generic;
using Controllers;
using Data;
using Helpers;
using UnityEngine;

namespace Components
{
//This component stores specific crowd data and includes game logic to control child units.
    public class Crowd : MonoBehaviour
    {
        [Min(1), SerializeField] private int _startCount = 1;

        public int MenCount => Men.Count;
        public int MenStartCount => _startCount;
        public Vector3 WorldPos => MenCount > 0 ? Men[0].transform.position : transform.position;
        public Vector3 ForwardManWorldPos => WorldPos + ForwardManLocalPos;
        public Vector3 ForwardManLocalPos { get; private set; }
        public MenCountLabel MenCountLabel { get; private set; }

        public List<Man> Men { get; private set; }
        private List<Man> _injuredMen;

        private PoolOfObjects<Man> _playerMenPool;
        private AnimationClip[] _animations;
        private Vector3[] _menSpawnPositions;
        private GameCore _gameCore;
        private AnimationType _currentMenAnimation = AnimationType.Idle;
        private Color _bloodColor;
        private float _manHitDelay;

        public void Initialize(Man manPrefab, float manHitDelay, AnimationClip[] animations,
            MenCountLabel countLabelPrefab = null)
        {
            Men = new List<Man>();
            _injuredMen = new List<Man>();
            _menSpawnPositions = ValuesCounter.HexagonPositions;

            _manHitDelay = manHitDelay;
            _animations = animations;
            _playerMenPool = new PoolOfObjects<Man>(manPrefab.gameObject, transform);
            _bloodColor = manPrefab.GetComponentInChildren<Renderer>().sharedMaterial.color;
            MenCountLabel = GetComponentInChildren<MenCountLabel>();

            if (!MenCountLabel && countLabelPrefab)
            {
                MenCountLabel = Instantiate(countLabelPrefab, Vector3.up * (ValuesCounter.MEN_OFFSET * 3),
                    Quaternion.Euler(0, 180, 0), transform);
            }

            MenCountLabel.Initialize(_bloodColor);
            SpawnMen(_startCount);

            GameEvents.OnCrowdFightEnd += RefreshMenPositioning;
        }

        public void DeInitialize()
        {
            GameEvents.OnCrowdFightEnd -= RefreshMenPositioning;
        }

        public void ChangeMenCount(int value)
        {
            if (value > 0) SpawnMen(value, false);
            else KillSomeMen(Mathf.Abs(value));
        }

        //This method spawns a unit using the Object Pool pattern.
        private void SpawnMen(int count, bool spawnOnTargetPos = true)
        {
            var startCount = MenCount;
            var targetCount = MenCount + count;

            if (targetCount > ValuesCounter.CROWD_CAPACITY)
                targetCount = ValuesCounter.CROWD_CAPACITY;

            var myTransform = transform;
            var myPos = MenCount > 0 ? Men[0].transform.position : myTransform.position;
            for (var i = startCount; i < targetCount; i++)
            {
                var spawnPos = spawnOnTargetPos ? myPos + _menSpawnPositions[i] : myPos;
                var man = _playerMenPool.Get();
                man.Initialize(_animations, _manHitDelay);
                man.MarkAsInjured(false);
                man.transform.position = spawnPos;
                man.gameObject.SetActive(true);
                Men.Add(man);
            }

            MenCountLabel.SetLabelValue(MenCount);
            RefreshMenPositioning();
            SetAnimation(_currentMenAnimation);
        }

        //This method updates the target positions of units after their number changes.
        private void RefreshMenPositioning()
        {
            ForwardManLocalPos = _menSpawnPositions[0];
            for (var i = 0; i < MenCount; i++)
            {
                var pos = _menSpawnPositions[i];
                Men[i].SetTargetLocalPos(_menSpawnPositions[i]);

                if (pos.z > ForwardManLocalPos.z)
                    ForwardManLocalPos = pos;
            }
        }

        //The same as the previous method, but for a certain array of positions (it is used for Boss Fight)
        public void RefreshMenPositioning(Vector3[] positions)
        {
            for (var i = 0; i < MenCount && i < positions.Length; i++)
            {
                var pos = positions[i];
                Men[i].SetTargetLocalPos(pos);
            }
        }

        //Kills a set of random units.
        public void KillSomeMen(int count)
        {
            for (var i = 0; i < count && i < MenCount; i++)
            {
                var man = Men[i];
                InjureMan(man);
            }

            KillInjuredMen();
            if (MenCount == 0) GameEvents.GameLoose();
        }

        //This method separates the unit's death from the list iteration process to avoid an InvalidOperationException.
        //It can also be useful for handling damage logic if you choose to add it to your game.
        public void InjureMan(Man injuredMan)
        {
            injuredMan.MarkAsInjured();
            _injuredMen.Add(injuredMan);
        }

        public void KillInjuredMen()
        {
            if (_injuredMen.Count <= 0)
                return;

            foreach (var man in _injuredMen)
            {
                KillMan(man);
            }

            _injuredMen.Clear();
            MenCountLabel.SetLabelValue(MenCount);
            RefreshMenPositioning();
        }

        private void KillMan(Man deadMan)
        {
            Men.Remove(deadMan);
            _playerMenPool.Put(deadMan);
            GameEvents.ManDead(deadMan.transform.position, _bloodColor);
            deadMan.gameObject.SetActive(false);

            if (MenCount > 0) Men[^1].SetTargetLocalPos(deadMan.TargetLocalPos);
        }

        public void SetAnimation(AnimationType animationType)
        {
            _currentMenAnimation = animationType;
            foreach (var man in Men)
            {
                man.PlayAnim(animationType);
            }
        }
    }
}