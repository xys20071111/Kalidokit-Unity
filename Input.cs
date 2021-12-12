using System.Collections.Generic;

namespace Kalidokit
{
    public class Point
    {
        public float x, y, z;
    }
    public class CapturePoint:Point
    {
        public float visibility;
    }
    public class CaptureData
    {
        public List<CapturePoint> faceLandmarks, poseLandmarks, rightHandLandmarks, leftHandLandmarks;
    }
}
