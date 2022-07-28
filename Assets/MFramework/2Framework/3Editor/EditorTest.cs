using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MFramework.AudioManager;

namespace MFramework
{
    /// <summary>
    /// 标题：主要用于测试框架功能
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.07.17
    /// 版本：1.0
    /// </summary>
    public class EditorTest : MonoBehaviour
    {

        [MenuItem("MFramework/Test/AudioManager", false, 1)]
        public static void TestAudioManager()
        {
            EditorApplication.isPlaying = true;
            Test();
        }
        private static void Test()
        {
            AudioManager.GetInstance.Play(SoundType.SoundEffect, Resources.Load<AudioClip>("Audio/effJumpScene"), () => Debug.Log("111"));
            AudioManager.GetInstance.Play(SoundType.SoundEffect, Resources.Load<AudioClip>("Audio/effBtnClick"), () => Debug.Log("222"));
            AudioManager.GetInstance.Play(SoundType.BGM, Resources.Load<AudioClip>("Audio/bgm1"));
        }

    }
}