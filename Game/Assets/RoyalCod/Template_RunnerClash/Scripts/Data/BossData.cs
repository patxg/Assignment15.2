using UnityEngine;

namespace Data
{
//This scriptable object stores boss data in a specific file.
    [CreateAssetMenu(fileName = "Boss Data", menuName = "Game Configurations/Boss Data", order = 52)]
    public class BossData : ScriptableObject
    {
        [SerializeField] private AnimationClip _bossAnimationIdle;
        [SerializeField] private AnimationClip _bossAnimationRun;
        [SerializeField] private AnimationClip _bossAnimationFight;
        [SerializeField] private AnimationClip _bossAnimationDeath;
        [SerializeField] private AnimationClip _bossAnimationWin;
        [Range(0.1f, 1f), SerializeField] private float _fightAnimHitMoment;
        [SerializeField] private float _fightDistance;
        [SerializeField] private int _damage;
        [SerializeField] private int _hp;

        public AnimationClip BossAnimation_Idle => _bossAnimationIdle;
        public AnimationClip BossAnimation_Run => _bossAnimationRun;
        public AnimationClip BossAnimation_Fight => _bossAnimationFight;
        public AnimationClip BossAnimation_Death => _bossAnimationDeath;
        public AnimationClip BossAnimation_Win => _bossAnimationWin;
        public float FightAnimHitMoment => _fightAnimHitMoment;
        public float FightDistance => _fightDistance;
        public int Damage => _damage;
        public int Hp => _hp;
    }
}