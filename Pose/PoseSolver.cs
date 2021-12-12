using System.Collections.Generic;
using UnityEngine;

namespace Kalidokit
{
    public class PoseSolver
    {
        public static PoseStruct Solve(List<CapturePoint> poseLandmarks, List<CapturePoint> poseLandmarks2D, bool EnableLegs)
        {
            PoseStruct result = new PoseStruct();

            ArmResult arms = CalcArms(poseLandmarks);
            bool rightHandOffscreen = poseLandmarks[15].y > -0.1 || poseLandmarks[15].visibility < 0.23 || 0.995 < poseLandmarks2D[15].y;
            bool leftHandOffscreen = poseLandmarks[16].y > -0.1 || poseLandmarks[16].visibility < 0.23 || 0.995 < poseLandmarks2D[16].y;

            arms.left.UpperArm = arms.left.UpperArm * (leftHandOffscreen ? 0 : 1);
            arms.left.UpperArm.z = leftHandOffscreen ? 1.25f : arms.left.UpperArm.z; //.55 is Hands down Default position
            arms.right.UpperArm = arms.right.UpperArm * (rightHandOffscreen ? 0 : 1);
            arms.right.UpperArm.z = rightHandOffscreen ? -1.25f : arms.right.UpperArm.z;

            result.LeftUpperArm = arms.left.UpperArm;
            result.LeftLowerArm = arms.left.LowerArm;
            result.LeftHand = arms.left.Hand;
            result.RightUpperArm = arms.right.UpperArm;
            result.RightLowerArm = arms.right.LowerArm;
            result.RightHand = arms.right.Hand;

            HipsResult hips = CalcHips(poseLandmarks, poseLandmarks2D);
            result.Hips = hips.Hips;
            result.Spine = hips.Spine;

            if(EnableLegs)
            {
                Debug.Log("现在还不支持腿部计算");
            }

            return result;

        }
        private static ArmResult CalcArms(List<CapturePoint> poseLandmarks)
        {
            Vector3 landmark11 = VectorUtils.GenerateVector3(poseLandmarks[11]),
                    landmark12 = VectorUtils.GenerateVector3(poseLandmarks[12]),
                    landmark13 = VectorUtils.GenerateVector3(poseLandmarks[13]),
                    landmark14 = VectorUtils.GenerateVector3(poseLandmarks[14]),
                    landmark15 = VectorUtils.GenerateVector3(poseLandmarks[15]),
                    landmark16 = VectorUtils.GenerateVector3(poseLandmarks[16]),
                    landmark17 = VectorUtils.GenerateVector3(poseLandmarks[17]),
                    landmark18 = VectorUtils.GenerateVector3(poseLandmarks[18]),
                    landmark19 = VectorUtils.GenerateVector3(poseLandmarks[19]),
                    landmark20 = VectorUtils.GenerateVector3(poseLandmarks[20]);

            Vector3 upperArmRight = VectorUtils.FindRotation(landmark11, landmark13);
            Vector3 upperArmLeft = VectorUtils.FindRotation(landmark12, landmark14);

            upperArmRight.y = VectorUtils.AngleBetween3DCoords(landmark11, landmark13, landmark15);
            upperArmLeft.y = VectorUtils.AngleBetween3DCoords(landmark11, landmark12, landmark14);

            Vector3 lowerArmRight = VectorUtils.FindRotation(landmark13, landmark15);
            Vector3 lowerArmLeft = VectorUtils.FindRotation(landmark14, landmark16);

            lowerArmRight.y = VectorUtils.AngleBetween3DCoords(landmark11, landmark13, landmark15);
            lowerArmLeft.y = VectorUtils.AngleBetween3DCoords(landmark12, landmark14, landmark16);
            lowerArmRight.z = Helper.Clamp(lowerArmRight.z, -2.14f, 0);
            lowerArmLeft.z = Helper.Clamp(lowerArmLeft.z, -2.14f, 0);

            Vector3 leftHand = VectorUtils.FindRotation(landmark15, Vector3.Lerp(landmark17, landmark19, 0.5f));
            Vector3 rightHand = VectorUtils.FindRotation(landmark16, Vector3.Lerp(landmark18, landmark20, 0.5f));

            ArmStruct right = RigArm(upperArmRight, lowerArmRight, rightHand, "Right");
            ArmStruct left = RigArm(upperArmLeft, lowerArmLeft, leftHand, "Left");

            ArmResult result = new ArmResult
            {
                right = right,
                left = left
            };

            return result;
        }

        private static ArmStruct RigArm(Vector3 UpperArm, Vector3 LowerArm, Vector3 Hand, string side)
        {
            int invert = side.Equals("Right") ? 1 : -1;
            UpperArm.z *= -2.3f * invert;
            //Modify UpperArm rotationY  by LowerArm X and Z rotations
            UpperArm.y *= Mathf.PI * invert;
            UpperArm.y -= Mathf.Max(LowerArm.x);
            UpperArm.y -= -invert * Mathf.Max(LowerArm.z, 0);
            UpperArm.x -= 0.3f * invert;

            LowerArm.z *= -2.14f * invert;
            LowerArm.y *= 2.14f * invert;
            LowerArm.x *= 2.14f * invert;

            //Clamp values to human limits
            UpperArm.x = Helper.Clamp(UpperArm.x, -0.5f, Mathf.PI);
            LowerArm.x = Helper.Clamp(LowerArm.x, -0.3f, 0.3f);

            Hand.y = Helper.Clamp(Hand.z * 2, -0.6f, 0.6f); //side to side
            Hand.z = Hand.z * -2.3f * invert; //up down

            ArmStruct result = new ArmStruct();
            result.UpperArm = UpperArm;
            result.LowerArm = LowerArm;
            result.Hand = Hand;
            return result;
        }
        private static HipsResult CalcHips(List<CapturePoint> poseLandmark, List<CapturePoint> poseLandmark2D)
        {
            Vector3 hipLeft2d = VectorUtils.GenerateVector3(poseLandmark2D[23]);
            Vector3 hipRight2d = VectorUtils.GenerateVector3(poseLandmark2D[24]);
            Vector3 shoulderLeft2d = VectorUtils.GenerateVector3(poseLandmark2D[11]);
            Vector3 shoulderRight2d = VectorUtils.GenerateVector3(poseLandmark2D[12]);
            // 此处对应的C#代码存疑
            Vector3 hipCenter2d = Vector3.Lerp(hipLeft2d, hipRight2d, 1); // hipLeft2d.Lerp(hipRight2d, 1);
            Vector3 shoulderCenter2d = Vector3.Lerp(shoulderLeft2d, shoulderRight2d, 1); // shoulderLeft2d.lerp(shoulderRight2d, 1);
            float spineLength = Vector3.Distance(hipCenter2d, shoulderCenter2d);// hipCenter2d.Distance(shoulderCenter2d);

            HipsStruct hips = new HipsStruct();
            hips.position = new Vector3(Helper.Clamp(-1 * (hipCenter2d.x - 0.65f), -1, 1), 0, Helper.Clamp(spineLength - 1, -2, 0));
            hips.rotation = VectorUtils.RollPitchYaw(VectorUtils.GenerateVector3(poseLandmark[23]), VectorUtils.GenerateVector3(poseLandmark[24]));

            //fix -PI, PI jumping
            if (hips.rotation.y > 0.5)
            {
                hips.rotation.y -= 2;
            }
            hips.rotation.y += 0.5f;
            //Stop jumping between left and right shoulder tilt
            if (hips.rotation.z > 0)
            {
                hips.rotation.z = 1 - hips.rotation.z;
            }
            if (hips.rotation.z < 0)
            {
                hips.rotation.z = -1 - hips.rotation.z;
            }
            float turnAroundAmountHips = Helper.Remap(Mathf.Abs(hips.rotation.y), 0.2f, 0.4f);
            hips.rotation.z *= 1 - turnAroundAmountHips;
            hips.rotation.x = 0; //temp fix for inaccurate X axis

            Vector3 spine = VectorUtils.RollPitchYaw(VectorUtils.GenerateVector3(poseLandmark[11]), VectorUtils.GenerateVector3(poseLandmark[12]));
            //fix -PI, PI jumping
            if (spine.y > 0.5)
            {
                spine.y -= 2;
            }
            spine.y += 0.5f;
            //Stop jumping between left and right shoulder tilt
            if (spine.z > 0)
            {
                spine.z = 1 - spine.z;
            }
            if (spine.z < 0)
            {
                spine.z = -1 - spine.z;
            }
            //fix weird large numbers when 2 shoulder points get too close
            float turnAroundAmount = Helper.Remap(Mathf.Abs(spine.y), 0.2f, 0.4f);
            spine.z *= 1 - turnAroundAmount;
            spine.x = 0; //temp fix for inaccurate X axis

            return RigHips(hips, spine);
        }
        public static HipsResult RigHips(HipsStruct hips, Vector3 spine)
        {
            hips.rotation.x *= Mathf.PI;
            hips.rotation.y *= Mathf.PI;
            hips.rotation.z *= Mathf.PI;

            hips.worldPosition = new Vector3(hips.position.x * (0.5f + 1.8f * -hips.position.z), 0, hips.position.z * (0.1f + hips.position.z * -2));
            spine.x *= Mathf.PI;
            spine.y *= Mathf.PI;
            spine.z *= Mathf.PI;

            return new HipsResult
            {
                Hips = hips,
                Spine = spine
            };
        }
    }
}
