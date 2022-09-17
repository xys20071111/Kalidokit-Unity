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
using System.Security.Cryptography;
using System.Diagnostics;
using Newtonsoft.Json;
using VRM;
using UniGLTF;
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
    private Process captureProcess = null;
    // Start is called before the first frame update
    void Start()
    {
        //这里是我自己项目里的模型管理器，判断模型是否加载的
        RuntimeGltfInstance model = ModelManager.GetCurrentModel();
        if (model == null)
        {
            UnityEngine.Debug.LogWarning("在没有加载模型的情况下尝试启动捕获");
            Destroy(this);
            return;
        }
        //验证脚本是不是被篡改了
        using (SHA256 hasher = SHA256.Create())
        {
            using (FileStream fs = File.Open(PY_SCRIPT.Replace("'",""), FileMode.Open))
            {
                try
                {
                    fs.Position = 0;
                    byte[] hashValue = hasher.ComputeHash(fs);
                    string fileHash = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
                    if (fileHash != "d86058a21d16a1805926fcb56b2189f1338bd37144aa1b688883357fb9f8e6a5")
                    {
                        UnityEngine.Debug.LogError("面捕脚本遭到篡改");
                        Destroy(this);
                    }
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError($"I/O Exception: {e.Message}");
                    Destroy(this);
                    return;
                }
                catch (UnauthorizedAccessException e)
                {
                    UnityEngine.Debug.LogError($"Access Exception: {e.Message}");
                    Destroy(this);
                    return;
                }
            }
        }
        //获取模型骨骼节点
        head = GameObject.Find("J_Bip_C_Head").transform;
        upperArmLeft = GameObject.Find("J_Bip_L_UpperArm").transform;
        lowerArmLeft = GameObject.Find("J_Bip_L_LowerArm").transform;
        handLeft = GameObject.Find("J_Bip_L_Hand").transform;
        upperArmRight = GameObject.Find("J_Bip_R_UpperArm").transform;
        lowerArmRight = GameObject.Find("J_Bip_R_LowerArm").transform;
        handRight = GameObject.Find("J_Bip_R_Hand").transform;
        spine = GameObject.Find("J_Bip_C_Spine").transform;
        blendShapeProxy = model.GetComponent<VRMBlendShapeProxy>();
        //运行面捕脚本
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
            if (cd.faceLandmarks.Count > 0)
            {
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
        if(captureProcess != null)
        {
            captureProcess.Kill();
        }
    }
}


```

### 面捕脚本
```
#!/usr/bin/python3
import sys
import mediapipe as mp
import cv2
import json


def main():
    cam = cv2.VideoCapture(0)
    pose_detector = mp.solutions.holistic.Holistic(min_detection_confidence=0.7, min_tracking_confidence=0.7, model_complexity=1,smooth_landmarks=True, refine_face_landmarks=True)
    try:
        while True:
            data = {
                'poseLandmarks': [],
                'worldPoseLandmarks': [],
                'worldPoseLandmarks': [],
                'rightHandLandmarks': [],
                'leftHandLandmarks': [],
                'faceLandmarks': []
            }
            success, image = cam.read()
            if not success:
                continue
            img_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
            result_pose = pose_detector.process(img_rgb)
            if result_pose.pose_landmarks:
                for point in result_pose.pose_landmarks.landmark:
                    data['poseLandmarks'].append({'x': point.x, 'y': point.y, 'z': 0, 'visibility': point.visibility})
                for point in result_pose.pose_world_landmarks.landmark:
                    data['worldPoseLandmarks'].append({'x': point.x, 'y': point.y, 'z': point.z, 'visibility': point.visibility})

            if result_pose.left_hand_landmarks:
                for point in result_pose.left_hand_landmarks.landmark:
                    data['leftHandLandmarks'].append({'x': point.x, 'y': point.y, 'z': 0, 'visibility': point.visibility})

            if result_pose.right_hand_landmarks:
                for point in result_pose.right_hand_landmarks.landmark:
                    data['rightHandLandmarks'].append({'x': point.x, 'y': point.y, 'z': 0, 'visibility': point.visibility})

            if result_pose.face_landmarks:
                for point in result_pose.face_landmarks.landmark:
                    data['faceLandmarks'].append({'x': point.x, 'y': point.y, 'z': point.z, 'visibility': point.visibility})

            print(json.dumps(data))

    except KeyboardInterrupt:
        socket.terminate()
        sys.exit(0)


if __name__ == '__main__':
    main()
```
