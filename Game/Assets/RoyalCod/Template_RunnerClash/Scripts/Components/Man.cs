using UnityEngine;

namespace Components
{
//This component stores data for a specific man.
    public class Man : BaseUnit
    {
        public Vector3 TargetLocalPos { get; private set; }
        public bool IsInjured { get; private set; }

        public void SetTargetLocalPos(Vector3 targetLocalPos)
        {
            TargetLocalPos = targetLocalPos;
        }

        public void MarkAsInjured(bool isInjured = true)
        {
            IsInjured = isInjured;
        }
    }
}