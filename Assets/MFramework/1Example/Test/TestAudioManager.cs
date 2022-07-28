using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MFramework.AudioManager;

namespace MFramework
{
    /// <summary>
    /// 标题：测试音频管理器
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.07.17
    /// 版本：1.0
    /// </summary>
    public class TestAudioManager : MonoBehaviour
    {
        private void Start()
        {
            AudioManager.GetInstance.Play(SoundType.BGM, Resources.Load<AudioClip>("Audio/bgm1"));

            DelayTool.GetInstance.Delay(2, () => AudioManager.GetInstance.Play(SoundType.SoundEffect, Resources.Load<AudioClip>("Audio/effBtnClick")));
            DelayTool.GetInstance.Delay(2.3f, () => AudioManager.GetInstance.Play(SoundType.SoundEffect, Resources.Load<AudioClip>("Audio/effJumpScene")));

            DelayTool.GetInstance.Delay(5, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/effBtnClick")));
            DelayTool.GetInstance.Delay(5.1f, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/effBtnClick")));
            DelayTool.GetInstance.Delay(5.2f, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/effBtnClick")));
            DelayTool.GetInstance.Delay(5.3f, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/effBtnClick")));

            DelayTool.GetInstance.Delay(7, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/bgm1")));
            DelayTool.GetInstance.Delay(7.5f, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/bgm1")));
            DelayTool.GetInstance.Delay(8f, () => AudioManager.GetInstance.Play(SoundType.SoundEffectTemp, Resources.Load<AudioClip>("Audio/bgm1")));

            DelayTool.GetInstance.Delay(15f, () =>
            {
                Debug.Log("临时音效暂停");
                AudioManager.GetInstance.Pause(SoundType.SoundEffectTemp);
            });

            DelayTool.GetInstance.Delay(20f, () => {
                Debug.Log("临时音效继续");
                AudioManager.GetInstance.Play(SoundType.SoundEffectTemp);
            });
        }
    }
}