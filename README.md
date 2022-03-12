# Kalidokit-Unity

试着把Kalidokit移植到C#  
# 目前全部抄下来了，但直接使用输出的数据会导致鬼畜
# 直接使用输出的数据会导致鬼畜
# 直接使用输出的数据会导致鬼畜
虽然代码基本上是照抄的原版[Kalidokit](https://github.com/yeemachine/kalidokit), 但我不知道能不能正常运行  

## 目前我使用的脸部绑定代码
```
if (cd.faceLandmarks.Count > 0)
{
  FaceStruct face = FaceSolver.Solve(cd.faceLandmarks, 640, 480, false);
  Loom.QueueOnMainThread((parama) =>
  {
    head.localRotation = Quaternion.Euler(new Vector3(-face.Head.rotate.x, face.Head.rotate.y, face.Head.rotate.z) * Mathf.Rad2Deg);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, face.Mouth.shape.A);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.E, face.Mouth.shape.E);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.I, face.Mouth.shape.I);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.O, face.Mouth.shape.O);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.U, face.Mouth.shape.U);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink_L, 1 - face.Eye.l);
    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink_R, 1 - face.Eye.r);
  }, null);
}
```

## 目前我使用的上半身绑定代码
```
if (cd.poseLandmarks.Count > 0)
{
  PoseStruct pose = PoseSolver.Solve(cd.worldPoseLandmarks, cd.poseLandmarks, false);
  Loom.QueueOnMainThread((parama) =>
  {
    upperArmLeft.localRotation = Quaternion.Euler(new Vector3(-pose.LeftUpperArm.x, -pose.LeftUpperArm.y, pose.LeftUpperArm.z) * Mathf.Rad2Deg * 0.7f);
    lowerArmLeft.localRotation = Quaternion.Euler(new Vector3(pose.LeftLowerArm.x, -pose.LeftLowerArm.y, pose.LeftUpperArm.z) * Mathf.Rad2Deg * 0.7f);
    handLeft.localRotation = Quaternion.Euler(pose.LeftHand * Mathf.Rad2Deg);
    upperArmRight.localRotation = Quaternion.Euler(new Vector3(-pose.RightUpperArm.x, -pose.RightUpperArm.y, pose.RightUpperArm.z) * Mathf.Rad2Deg * 0.7f);
    lowerArmRight.localRotation = Quaternion.Euler(new Vector3(pose.RightLowerArm.x, -pose.RightLowerArm.y, pose.RightUpperArm.z) * Mathf.Rad2Deg * 0.7f);
    handRight.localRotation = Quaternion.Euler(pose.RightHand * Mathf.Rad2Deg);
    }, null);
}
```
