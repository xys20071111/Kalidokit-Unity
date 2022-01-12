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
        RingIntermediate = new Vector3(0, 0, VectorUtils.AngleBetween3DCoords(points[13], points[14], points[15])),
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

      return RigFingers(hand);
    }

    private static HandResult RigFingers(HandResult hand, string side = "Right")
    {
      HandResult newHand = new HandResult{};
      int invert = side.Equals("Right") ? 1 : -1;
      string[] digits = { "Ring", "Index", "Little", "Thumb", "Middle" };
      string[] segments = { "Proximal", "Intermediate", "Distal" };
      hand.Wrist.x = Helper.Clamp(hand.Wrist.x * 2f * invert, -0.3f, 0.3f);
      hand.Wrist.y = Helper.Clamp(hand.Wrist.y * 2.3f, side.Equals("Right") ? -1.2f : -0.6f, side.Equals("Right") ? 0.6f : 1.6f);
      hand.Wrist.z = hand.Wrist.z * -2.3f * invert;
      foreach (string e in digits)
      {
        foreach (string j in segments)
        {
          Vector3 trackedFinger;
          switch (e + j)
          {
            case "RingProximal":
              trackedFinger = hand.RingProximal;
              break;
            case "RingIntermediate":
              trackedFinger = hand.RingIntermediate;
              break;
            case "RingDistal":
              trackedFinger = hand.RingDistal;
              break;
            case "IndexProximal":
              trackedFinger = hand.IndexProximal;
              break;
            case "IndexIntermediate":
              trackedFinger = hand.IndexIntermediate;
              break;
            case "IndexDistal":
              trackedFinger = hand.IndexDistal;
              break;
            case "MiddleProximal":
              trackedFinger = hand.MiddleProximal;
              break;
            case "MiddleIntermediate":
              trackedFinger = hand.MiddleIntermediate;
              break;
            case "MiddleDistal":
              trackedFinger = hand.MiddleDistal;
              break;
            case "ThumbProximal":
              trackedFinger = hand.ThumbProximal;
              break;
            case "ThumbIntermediate":
              trackedFinger = hand.ThumbIntermediate;
              break;
            case "ThumbDistal":
              trackedFinger = hand.ThumbDistal;
              break;
            case "LittleProximal":
              trackedFinger = hand.LittleProximal;
              break;
            case "LittleIntermediate":
              trackedFinger = hand.LittleIntermediate;
              break;
            case "LittleDistal":
              trackedFinger = hand.LittleDistal;
              break;
            default:
              trackedFinger = hand.LittleDistal;
              break;
          }
          if (e.Equals("Thumb"))
          {
            //dampen thumb rotation depending on segment
            Vector3 dampener = new Vector3(
                j.Equals("Proximal") ? 1.2f : j.Equals("Distal") ? -0.2f : -0.2f,
                j.Equals("Proximal") ? 1.1f * invert : j.Equals("Distal") ? 0.1f * invert : 0.1f * invert,
                j.Equals("Proximal") ? 0.2f * invert : j.Equals("Distal") ? 0.2f * invert : 0.2f * invert
            );
            Vector3 startPos = new Vector3(
                x: j.Equals("Proximal") ? 1.2f : j.Equals("Distal") ? -0.2f : -0.2f,
                y: j.Equals("Proximal") ? 1.1f * invert : j.Equals("Distal") ? 0.1f * invert : 0.1f * invert,
                z: j.Equals("Proximal") ? 0.2f * invert : j.Equals("Distal") ? 0.2f * invert : 0.2f * invert
            );
            Vector3 newThumb = Vector3.zero;
            if (j.Equals("Proximal"))
            {
              newThumb.z = Helper.Clamp(
                  startPos.z + trackedFinger.z * -Mathf.PI * dampener.z * invert,
                  side.Equals("Right") ? -0.6f : -0.3f,
                  side.Equals("Right") ? 0.3f : 0.6f
              );
              newThumb.x = Helper.Clamp(startPos.x + trackedFinger.z * -Mathf.PI * dampener.x, -0.6f, 0.3f);
              newThumb.y = Helper.Clamp(
                  startPos.y + trackedFinger.z * -Mathf.PI * dampener.y * invert,
                  side.Equals("Right") ? -1f : -0.3f,
                  side.Equals("Right") ? 0.3f : 1f
              );
            }
            else
            {
              newThumb.z = Helper.Clamp(startPos.z + trackedFinger.z * -Mathf.PI * dampener.z * invert, -2, 2);
              newThumb.x = Helper.Clamp(startPos.x + trackedFinger.z * -Mathf.PI * dampener.x, -2, 2);
              newThumb.y = Helper.Clamp(startPos.y + trackedFinger.z * -Mathf.PI * dampener.y * invert, -2, 2);
            }
            trackedFinger.x = newThumb.x;
            trackedFinger.y = newThumb.y;
            trackedFinger.z = newThumb.z;
          }
          else
          {
            //will document human limits later
            trackedFinger.z = Helper.Clamp(
                trackedFinger.z * -Mathf.PI * invert,
                side.Equals("Right") ? -Mathf.PI : 0,
                side.Equals("Right") ? 0 : Mathf.PI
            );
          }
          switch (e + j)
          {
            case "RingProximal":
              newHand.RingProximal = trackedFinger;
              break;
            case "RingIntermediate":
              newHand.RingIntermediate = trackedFinger;
              break;
            case "RingDistal":
              newHand.RingDistal = trackedFinger;
              break;
            case "IndexProximal":
              newHand.IndexProximal = trackedFinger;
              break;
            case "IndexIntermediate":
              newHand.IndexIntermediate = trackedFinger;
              break;
            case "IndexDistal":
              newHand.IndexDistal = trackedFinger;
              break;
            case "MiddleProximal":
              newHand.MiddleProximal = trackedFinger;
              break;
            case "MiddleIntermediate":
              newHand.MiddleIntermediate = trackedFinger;
              break;
            case "MiddleDistal":
              newHand.MiddleDistal = trackedFinger;
              break;
            case "ThumbProximal":
              newHand.ThumbProximal = trackedFinger;
              break;
            case "ThumbIntermediate":
              newHand.ThumbIntermediate = trackedFinger;
              break;
            case "ThumbDistal":
              newHand.ThumbDistal = trackedFinger;
              break;
            case "LittleProximal":
              newHand.LittleProximal = trackedFinger;
              break;
            case "LittleIntermediate":
              newHand.LittleIntermediate = trackedFinger;
              break;
            case "LittleDistal":
              newHand.LittleDistal = trackedFinger;
              break;
          }
        }
      }
      return newHand;
    }
  }
}
