using System.Collections.Generic;
using UnityEngine;

namespace Kalidokit
{
    public class EyeUtils
    {
        private static int[] LeftEyeIndex = { 130, 133, 160, 159, 158, 144, 145, 153 };
        private static int[] RightEyeIndex = { 263, 362, 387, 386, 385, 373, 374, 380 };
        private static int[] LeftBrowIndex = { 35, 244, 63, 105, 66, 229, 230, 231 };
        private static int[] RightBrowIndex = { 265, 464, 293, 334, 296, 449, 450, 451 };
        private static int[] LeftPupilIndex = { 468, 469, 470, 471, 472 };
        private static int[] RightPupilIndex = { 473, 474, 475, 476, 477 };
        public static EyeOpenStruct GetEyeOpen(List<CapturePoint> poseLandmark, string side = "left", float high = 0.85f, float low = 0.55f)
        {
            if(side.Equals("left"))
            {
                 float eyeDistance = EyeLidRatio(
                    poseLandmark[LeftEyeIndex[0]],
                    poseLandmark[LeftEyeIndex[1]],
                    poseLandmark[LeftEyeIndex[2]],
                    poseLandmark[LeftEyeIndex[3]],
                    poseLandmark[LeftEyeIndex[4]],
                    poseLandmark[LeftEyeIndex[5]],
                    poseLandmark[LeftEyeIndex[6]],
                    poseLandmark[LeftEyeIndex[7]]
                );
                // human eye width to height ratio is roughly .3
                float maxRatio = 0.285f;
                // compare ratio against max ratio
                float ratio = Helper.Clamp(eyeDistance / maxRatio, 0, 2);
                // remap eye open and close ratios to increase sensitivity
                float eyeOpenRatio = Helper.Remap(ratio, low, high);
                return new EyeOpenStruct
                {
                    // remapped ratio
                    norm = eyeOpenRatio,
                    // ummapped ratio
                    raw = ratio,
                };
            }
            else
            {
                float eyeDistance = EyeLidRatio(
                    poseLandmark[RightEyeIndex[0]],
                    poseLandmark[RightEyeIndex[1]],
                    poseLandmark[RightEyeIndex[2]],
                    poseLandmark[RightEyeIndex[3]],
                    poseLandmark[RightEyeIndex[4]],
                    poseLandmark[RightEyeIndex[5]],
                    poseLandmark[RightEyeIndex[6]],
                    poseLandmark[RightEyeIndex[7]]
                );
                // human eye width to height ratio is roughly .3
                float maxRatio = 0.285f;
                // compare ratio against max ratio
                float ratio = Helper.Clamp(eyeDistance / maxRatio, 0, 2);
                // remap eye open and close ratios to increase sensitivity
                float eyeOpenRatio = Helper.Remap(ratio, low, high);
                return new EyeOpenStruct
                {
                    // remapped ratio
                    norm = eyeOpenRatio,
                    // ummapped ratio
                    raw = ratio,
                };
            }
        }
        public static float EyeLidRatio(Point eyeOuterCorner, Point eyeInnerCorner, Point eyeOuterUpperLid, Point eyeMidUpperLid, Point eyeInnerUpperLid, Point eyeOuterLowerLid, Point eyeMidLowerLid, Point eyeInnerLowerLid)
        {
            Vector3 eyeOuterCornerVector = VectorUtils.GenerateVector3(eyeOuterCorner),
                    eyeInnerCornerVector = VectorUtils.GenerateVector3(eyeInnerCorner),
                    eyeOuterUpperLidVector = VectorUtils.GenerateVector3(eyeOuterUpperLid),
                    eyeMidUpperLidVector = VectorUtils.GenerateVector3(eyeMidUpperLid),
                    eyeInnerUpperLidVector = VectorUtils.GenerateVector3(eyeInnerUpperLid),
                    eyeOuterLowerLidVector = VectorUtils.GenerateVector3(eyeOuterLowerLid),
                    eyeMidLowerLidVector = VectorUtils.GenerateVector3(eyeMidLowerLid),
                    eyeInnerLowerLidVector = VectorUtils.GenerateVector3(eyeInnerLowerLid);
            float eyeWidth = Vector3.Distance(eyeOuterCornerVector, eyeInnerCornerVector); //(eyeOuterCorner as Vector).distance(eyeInnerCorner as Vector, 2)
            float eyeOuterLidDistance = Vector3.Distance(eyeOuterUpperLidVector, eyeOuterLowerLidVector); //(eyeOuterUpperLid as Vector).distance(eyeOuterLowerLid as Vector, 2)
            float eyeMidLidDistance = Vector3.Distance(eyeMidUpperLidVector, eyeMidLowerLidVector); //(eyeMidUpperLid as Vector).distance(eyeMidLowerLid as Vector, 2)
            float eyeInnerLidDistance = Vector3.Distance(eyeInnerUpperLidVector, eyeInnerLowerLidVector); //(eyeInnerUpperLid as Vector).distance(eyeInnerLowerLid as Vector, 2);
            float eyeLidAvg = (eyeOuterLidDistance + eyeMidLidDistance + eyeInnerLidDistance) / 3;
            float ratio = eyeLidAvg / eyeWidth;

            return ratio;
        }
        public static PupilPosStruct GetPupilPos(List<CapturePoint> poseLandmark, string side = "left")
        {
            if (side.Equals("left"))
            {
                Vector3 eyeOuterCorner = VectorUtils.GenerateVector3(poseLandmark[LeftPupilIndex[0]]);
                Vector3 eyeInnerCorner = VectorUtils.GenerateVector3(poseLandmark[LeftPupilIndex[1]]);
                float eyeWidth = Vector3.Distance(eyeOuterCorner, eyeInnerCorner); //eyeOuterCorner.distance(eyeInnerCorner, 2);
                Vector3 midPoint = Vector3.Lerp(eyeOuterCorner, eyeInnerCorner, 0.5f); //eyeOuterCorner.lerp(eyeInnerCorner, 0.5);
                Vector3 pupil = VectorUtils.GenerateVector3(poseLandmark[LeftPupilIndex[0]]);
                float dx = midPoint.x - pupil.x;
                //eye center y is slightly above midpoint
                float dy = midPoint.y - eyeWidth * 0.075f - pupil.y;
                float ratioX = dx / (eyeWidth / 2);
                float ratioY = dy / (eyeWidth / 4);

                ratioX *= 4;
                ratioY *= 4;

                return new PupilPosStruct { x = ratioX, y = ratioY };
            }
            else
            {
                Vector3 eyeOuterCorner = VectorUtils.GenerateVector3(poseLandmark[RightPupilIndex[0]]);
                Vector3 eyeInnerCorner = VectorUtils.GenerateVector3(poseLandmark[RightPupilIndex[1]]);
                float eyeWidth = Vector3.Distance(eyeOuterCorner, eyeInnerCorner); //eyeOuterCorner.distance(eyeInnerCorner, 2);
                Vector3 midPoint = Vector3.Lerp(eyeOuterCorner, eyeInnerCorner, 0.5f); //eyeOuterCorner.lerp(eyeInnerCorner, 0.5);
                Vector3 pupil = VectorUtils.GenerateVector3(poseLandmark[RightPupilIndex[0]]);
                float dx = midPoint.x - pupil.x;
                //eye center y is slightly above midpoint
                float dy = midPoint.y - eyeWidth * 0.075f - pupil.y;
                float ratioX = dx / (eyeWidth / 2);
                float ratioY = dy / (eyeWidth / 4);

                ratioX *= 4;
                ratioY *= 4;

                return new PupilPosStruct { x = ratioX, y = ratioY };
            }
        }
        public static EyeStruct StabilizeBlink(EyeStruct eye, float headY, bool enableWink = true, float maxRot = 0.5f)
        {
            eye.r = Helper.Clamp(eye.r, 0, 1);
            eye.l = Helper.Clamp(eye.l, 0, 1);

            float blinkDiff = Mathf.Abs(eye.l - eye.r);
            //theshold to which difference is considered a wink
            float blinkThresh = enableWink ? 0.8f : 1.2f;
            //detect when both eyes are closing
            bool isClosing = eye.l < 0.3f && eye.r < 0.3f;
            //detect when both eyes are opening
            bool isOpen = eye.l > 0.6f && eye.r > 0.6f;
            if (headY > maxRot)
            {
                return new EyeStruct
                { l = eye.r, r = eye.r };
            }
            if (headY < -maxRot)
            {
                 return new EyeStruct
                { l = eye.l, r = eye.r };
            }
            return new EyeStruct
            {
                l =
                blinkDiff >= blinkThresh && !isClosing && !isOpen
                    ? eye.l
                    : eye.r > eye.l
                    ? Mathf.Lerp(eye.r, eye.l, 0.95f)
                    : Mathf.Lerp(eye.r, eye.l, 0.05f),
                r =
                blinkDiff >= blinkThresh && !isClosing && !isOpen
                    ? eye.r
                    : eye.r > eye.l
                    ? Mathf.Lerp(eye.r, eye.l, 0.95f)
                    : Mathf.Lerp(eye.r, eye.l, 0.05f),
            };
        }
        public static float GetBrowRaise(List<CapturePoint> poseLandmark, string side = "left")
        {
            if(side.Equals("left"))
            {
               
                float browDistance = EyeLidRatio(
                    poseLandmark[LeftBrowIndex[0]],
                    poseLandmark[LeftBrowIndex[1]],
                    poseLandmark[LeftBrowIndex[2]],
                    poseLandmark[LeftBrowIndex[3]],
                    poseLandmark[LeftBrowIndex[4]],
                    poseLandmark[LeftBrowIndex[5]],
                    poseLandmark[LeftBrowIndex[6]],
                    poseLandmark[LeftBrowIndex[7]]
                );

                float maxBrowRatio = 1.15f;
                float browHigh = 0.125f;
                float browLow = 0.07f;
                float browRatio = browDistance / maxBrowRatio - 1;
                float browRaiseRatio = (Helper.Clamp(browRatio, browLow, browHigh) - browLow) / (browHigh - browLow);
                return browRaiseRatio;
            }
            else
            {
                float browDistance = EyeLidRatio(
                    poseLandmark[RightBrowIndex[0]],
                    poseLandmark[RightBrowIndex[1]],
                    poseLandmark[RightBrowIndex[2]],
                    poseLandmark[RightBrowIndex[3]],
                    poseLandmark[RightBrowIndex[4]],
                    poseLandmark[RightBrowIndex[5]],
                    poseLandmark[RightBrowIndex[6]],
                    poseLandmark[RightBrowIndex[7]]
                );

                float maxBrowRatio = 1.15f;
                float browHigh = 0.125f;
                float browLow = 0.07f;
                float browRatio = browDistance / maxBrowRatio - 1;
                float browRaiseRatio = (Helper.Clamp(browRatio, browLow, browHigh) - browLow) / (browHigh - browLow);
                return browRaiseRatio;
            }
        }
    }
}
