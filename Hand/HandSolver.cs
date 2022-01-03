using UnityEngine;
using System.Collections.Generic;

namespace Kalidokit
{
  public class HandSolver
  {
    public static HandResult Solve(List<CapturePoint> poseLandmark, string side = "Right")
    {
      List<Vector3> points = new List<Vector3>();
      poseLandmark.ForEach((CapturePoint point) =>
      {
        points.Add(VectorUtils.GenerateVector3(point));
      });

      Vector3 palm0 = points[0];
      Vector3 palm1 = points[side.Equals("Right") ? 17 : 5];
      Vector3 palm2 = points[side.Equals("Right") ? 5 : 17];
      Vector3 handRotation = VectorUtils.RollPitchYaw(palm0, palm1, palm2);
      handRotation.y = handRotation.z;
      handRotation.y -= side.Equals("Left") ? 0.4f : 0.4f;

      HandResult hand = new HandResult
      {
        Wrist = handRotation,
        RingProximal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[0], points[13], points[14])),
        RingIntermediate = new Vector3(0,0,VectorUtils.AngleBetween3DCoords(points[13], points[14], points[15])),
        RingDistal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[14], points[15], points[16])),
        IndexProximal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[0], points[5], points[6])),
        IndexIntermediate = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[5], points[6], points[7])),
        IndexDistal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[6], points[7], points[8])),
        MiddleProximal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[0], points[9], points[10])),
        MiddleIntermediate = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[9], points[10], points[11])),
        MiddleDistal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[10], points[11], points[12])),
        ThumbProximal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[0], points[1], points[2])),
        ThumbIntermediate = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[1], points[2], points[3])),
        ThumbDistal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[2], points[3], points[4])),
        LittleProximal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[0], points[17], points[18])),
        LittleIntermediate = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[17], points[18], points[19])),
        LittleDistal = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[18], points[19], points[20]))
      };
    }

    private static HandResult RigHand(HandResult hand){
        
    } 
  }
}
