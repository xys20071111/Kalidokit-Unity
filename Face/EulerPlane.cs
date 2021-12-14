using UnityEngine;

namespace Kalidokit
{
    public class EulerPlane
    {
        private Vector3 p1, p2, p3, p4, p3mid;
        public EulerPlane(Point p1, Point p2, Point p3, Point p4)
        {
            this.p1 = VectorUtils.GenerateVector3(p1);
            this.p2 = VectorUtils.GenerateVector3(p2);
            this.p3 = VectorUtils.GenerateVector3(p3);
            this.p4 = VectorUtils.GenerateVector3(p4);
            this.p3mid = Vector3.Lerp(this.p3, this.p4, 0.5f);
        }
        public Vector3[] GetVector()
        {
            Vector3[] result = {
            this.p1,
            this.p2,
            this.p3mid
            };
            return result;
        }
        public Vector3[] GetPoint()
        {
            Vector3[] result = {
            this.p1,
            this.p2,
            this.p3,
            this.p4
            };
            return result;
        }
    }
}
