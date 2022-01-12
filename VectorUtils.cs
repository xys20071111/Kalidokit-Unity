using UnityEngine;

namespace Kalidokit
{
    class VectorUtils
    {
        public static float TWO_PI = Mathf.PI * 2;

        public static Vector3 GenerateVector3(Point point)
        {
            return new Vector3(point.x, point.y, point.z);
        }
        public static Vector2 GenerateVector2(Point point)
        {
            return new Vector2(point.x, point.y);
        }
        public static float Find2DAngle(float cx, float cy, float ex, float ey)
        {
            float dy = ey - cy;
            float dx = ex - cx;
            return Mathf.Atan2(dy, dx);
        }
        public static float NormalizeRadians(float radians)
        {
            float result = radians;
            if (result >= Mathf.PI / 2)
            {
                result -= 2 * Mathf.PI;
            }
            if (result <= -Mathf.PI / 2)
            {
                result += 2 * Mathf.PI;
                result = Mathf.PI - result;
            }
            //returns normalized values to -1,1
            return result / Mathf.PI;
        }
        public static Vector3 FindRotation(Vector3 a, Vector3 b, bool normalize = true)
        {
            if(normalize)
            {
                return new Vector3(
                    NormalizeRadians(Find2DAngle(a.z, a.x, b.z, b.x)),
                    NormalizeRadians(Find2DAngle(a.z, a.y, b.z, b.y)),
                    NormalizeRadians(Find2DAngle(a.x, a.y, b.x, b.y))
                    );
            }
            else
            {
                return new Vector3(
                    Find2DAngle(a.z, a.x, b.z, b.x),
                    Find2DAngle(a.z, a.y, b.z, b.y),
                    Find2DAngle(a.x, a.y, b.x, b.y)
                    );
            }
        }
        public static float AngleBetween3DCoords(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v1 = a - b;
            Vector3 v2 = c - b;

            Vector3 v1Norm = v1 / v1.magnitude;
            Vector3 v2Norm = v2 / v2.magnitude;

            float dotProduct = Vector3.Dot(v1Norm, v2Norm);
            return NormalizeRadians(Mathf.Acos(dotProduct));
        }
        public static float NormalizeAngle(float radians)
        {
            
            float angle = radians % TWO_PI;
            angle = angle > Mathf.PI ? angle - TWO_PI : angle < -Mathf.PI ? TWO_PI + angle : angle;
            //returns normalized values to -1,1
            return angle / Mathf.PI;
        }
        public static Vector3 RollPitchYaw(Vector3 a, Vector3 b)
        {
            return new Vector3(
                NormalizeAngle(Find2DAngle(a.z, a.y, b.z, b.y)),
                NormalizeAngle(Find2DAngle(a.z, a.x, b.z, b.x)),
                NormalizeAngle(Find2DAngle(a.x, a.y, b.x, b.y))
            );
        }
        public static Vector3 RollPitchYaw(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 qb = b - a;
            Vector3 qc = c - a;
            Vector3 n = Vector3.Cross(qb, qc);

            Vector3 unitZ = n / n.magnitude;
            Vector3 unitX = qb / qb.magnitude;
            Vector3 unitY = Vector3.Cross(unitZ, unitX);

            float beta = Mathf.Asin(unitZ.x);
            if(float.IsNaN(beta))
            { beta = 0; }
            float alpha = Mathf.Atan2(-unitZ.y, unitZ.z);
            if (float.IsNaN(alpha))
            { alpha = 0; }
            float gamma = Mathf.Atan2(-unitY.x, unitX.x);
            if (float.IsNaN(gamma))
            { gamma = 0; }

            return new Vector3(NormalizeAngle(alpha), NormalizeAngle(beta), NormalizeAngle(gamma));
        }
    }
}
