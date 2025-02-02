using System.Collections;
using Data;
using UnityEngine;

namespace Components
{
    public class Boss : BaseUnit
    {
        [SerializeField] private BossData _bossData;

        public int Hp { get; private set; }
        public int Damage => _bossData.Damage;
        public float FightDistance => _bossData.FightDistance;

        public void Initialize()
        {
            var animations = new[]
            {
                _bossData.BossAnimation_Idle,
                _bossData.BossAnimation_Run,
                _bossData.BossAnimation_Fight,
                _bossData.BossAnimation_Win,
                _bossData.BossAnimation_Death,
            };

            var fightAnimationLength = animations[(int) AnimationType.Fight].length;
            var hitDelay = fightAnimationLength * _bossData.FightAnimHitMoment;
            Hp = _bossData.Hp;

            GameEvents.OnBossFightStart += StartFight;
            GameEvents.OnBossFightEnd += StopFight;

            base.Initialize(animations, hitDelay);
        }

        public void DeInitialize()
        {
            GameEvents.OnBossFightStart -= StartFight;
            GameEvents.OnBossFightEnd -= StopFight;
        }

        private void StartFight()
        {
            PlayAnim(AnimationType.Fight);
        }

        private void StopFight(bool isPlayerWin)
        {
            PlayAnim(isPlayerWin ? AnimationType.Death : AnimationType.Win, false);
        }

        public int GetDamage(int damage = 1)
        {
            return Hp -= damage;
        }
    }
}
