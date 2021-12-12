using UnityEngine;

namespace Kalidokit
{
    public struct ArmStruct
    {
        public Vector3 UpperArm, LowerArm, Hand;
    }
    public struct ArmResult
    {
        public ArmStruct left, right;
    }
}
