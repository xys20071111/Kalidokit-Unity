# Kalidokit-Unity

试着把Kalidokit移植到C#  
# 目前全部抄下来了，但直接使用输出的数据会导致鬼畜
# 直接使用输出的数据会导致鬼畜
# 直接使用输出的数据会导致鬼畜
虽然代码基本上是照抄的原版[Kalidokit](https://github.com/yeemachine/kalidokit), 但我不知道能不能正常运行  

## 目前我使用的绑定代码
```
using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using VRM;
using UnityEngine;
using Kalidokit;

public class FaceCaptureLauncher : MonoBehaviour
{
    private Transform head;
    private Transform upperArmLeft;
    private Transform lowerArmLeft;
    private Transform handLeft;
    private Transform upperArmRight;
    private Transform lowerArmRight;
    private Transform handRight;
    private Transform spine;
    private VRMBlendShapeProxy blendShapeProxy;
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
    private const string PY_RUNTIME = "/usr/bin/python3";
    private string PY_SCRIPT = $"'{Application.dataPath}/StreamingAssets/capture/main.py'";
#elif UNITY_STANDALONE || UNITY_EDITOR
    private string PY_RUNTIME = @$"{Application.dataPath}\StreamingAssets\python\python.exe";
    private string PY_SCRIPT = @$"\"{Application.dataPath}\StreamingAssets\capture\main.py\"";
#endif
    private Process captureProcess;
    // Start is called before the first frame update
    void Start()
    {
        head = GameObject.Find("J_Bip_C_Head").transform;
        upperArmLeft = GameObject.Find("J_Bip_L_UpperArm").transform;
        lowerArmLeft = GameObject.Find("J_Bip_L_LowerArm").transform;
        handLeft = GameObject.Find("J_Bip_L_Hand").transform;
        upperArmRight = GameObject.Find("J_Bip_R_UpperArm").transform;
        lowerArmRight = GameObject.Find("J_Bip_R_LowerArm").transform;
        handRight = GameObject.Find("J_Bip_R_Hand").transform;
        spine = GameObject.Find("J_Bip_C_Spine").transform;
        blendShapeProxy = GameObject.Find("Model").GetComponent<VRMBlendShapeProxy>();
        captureProcess = new Process();
        captureProcess.StartInfo.FileName = PY_RUNTIME;
        captureProcess.StartInfo.Arguments = PY_SCRIPT;
        captureProcess.StartInfo.RedirectStandardOutput = true;
        captureProcess.StartInfo.RedirectStandardError = true;
        captureProcess.StartInfo.UseShellExecute = false;
        captureProcess.StartInfo.CreateNoWindow = true;
        captureProcess.Start();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            string jsonString = captureProcess.StandardOutput.ReadLine();
            CaptureData cd = JsonConvert.DeserializeObject<CaptureData>(jsonString);
            // 面部
            if (cd.faceLandmarks.Count > 0) {
                FaceStruct face = FaceSolver.Solve(cd.faceLandmarks, 480, 640, true);
                head.localRotation = Quaternion.Euler(new Vector3(-face.Head.rotate.x, face.Head.rotate.y, face.Head.rotate.z) * Mathf.Rad2Deg);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.A, face.Mouth.shape.A);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.E, face.Mouth.shape.E);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.I, face.Mouth.shape.I);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.O, face.Mouth.shape.O);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.U, face.Mouth.shape.U);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink_L, 1 - face.Eye.l);
                blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink_R, 1 - face.Eye.r);
            }
            // 上半身
            if (cd.poseLandmarks.Count > 0)
            {
                PoseStruct pose = PoseSolver.Solve(cd.worldPoseLandmarks, cd.poseLandmarks, false);
                upperArmLeft.localRotation = Quaternion.Euler(new Vector3(-pose.LeftUpperArm.x, -pose.LeftUpperArm.y, pose.LeftUpperArm.z) * Mathf.Rad2Deg * 0.7f);
                lowerArmLeft.localRotation = Quaternion.Euler(new Vector3(pose.LeftLowerArm.x, -pose.LeftLowerArm.y, pose.LeftUpperArm.z) * Mathf.Rad2Deg * 0.7f);
                handLeft.localRotation = Quaternion.Euler(pose.LeftHand * Mathf.Rad2Deg);
                upperArmRight.localRotation = Quaternion.Euler(new Vector3(-pose.RightUpperArm.x, -pose.RightUpperArm.y, pose.RightUpperArm.z) * Mathf.Rad2Deg * 0.7f);
                lowerArmRight.localRotation = Quaternion.Euler(new Vector3(pose.RightLowerArm.x, -pose.RightLowerArm.y, pose.RightUpperArm.z) * Mathf.Rad2Deg * 0.7f);
                handRight.localRotation = Quaternion.Euler(pose.RightHand * Mathf.Rad2Deg);
                spine.localRotation = Quaternion.Euler(new Vector3(pose.Spine.x, pose.Spine.y, pose.Spine.z) * Mathf.Rad2Deg);
            }

        } catch (Exception e)
        {
            UnityEngine.Debug.LogWarning(e.Message);
        }
    }
    void OnDestroy()
    {
        captureProcess.Kill();
    }
}

```
