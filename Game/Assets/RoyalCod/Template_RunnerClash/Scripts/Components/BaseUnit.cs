using UnityEngine;

namespace Components
{
//A set of common animations.
public enum AnimationType
{
    Idle,
    Run,
    Fight,
    Win,
    Death
}

//This component serves as the foundation for all units in the game.
//It contains base logic that is common to all active units.
    public class BaseUnit : MonoBehaviour
    {
        private AnimatorOverrideController _animOverrideCtrl;
        private AnimationClip[] _animations;
        private Animator _animator;
        private float _hitTime;
        private float _hitDelay;
        private float _curAnimLength;
        private bool _isReadyToHit;

        public void Initialize(AnimationClip[] animations, float hitDelay)
        {
            _hitDelay = hitDelay;
            _animations = animations;
            _animator = GetComponentInChildren<Animator>();

            if (_animOverrideCtrl)
                return;

            //The RuntimeAnimatorController is used to standardize the animation structure, allowing us to use a single animator for every unit in the game.
            RuntimeAnimatorController runtimeAnimCtrl = _animator.runtimeAnimatorController;
            _animOverrideCtrl = new AnimatorOverrideController(runtimeAnimCtrl);
            _animOverrideCtrl.runtimeAnimatorController = runtimeAnimCtrl;
            _animator.runtimeAnimatorController = _animOverrideCtrl;
        }

        //This method makes the animator play a specific animation.
        public void PlayAnim(AnimationType animType, bool onRandomFrame = true)
        {
            if (!_animator)
                return;

            var clip = _animations[(int) animType];
            _animOverrideCtrl["Action"] = clip;
            _animator.SetTrigger("Play");
            var animState = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(animState.fullPathHash, -1, onRandomFrame ? Random.Range(0f, clip.length) : 0);
            _curAnimLength = clip.length;
            if (animType == AnimationType.Fight)
            {
                _hitTime = 0;
                _isReadyToHit = false;
            }
        }

        //This method counts the time until the unit's next hit.
        public bool TryHit(float deltaTime)
        {
            _hitTime += deltaTime;

            if (!_isReadyToHit)
            {
                if (_hitTime >= _curAnimLength)
                {
                    _hitTime = 0;
                    _isReadyToHit = true;
                }

                return false;
            }

            var isHit = _hitTime >= _hitDelay;
            if (isHit)
            {
                _isReadyToHit = false;
            }

            return isHit;
        }
    }
}
