using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：音频管理器
    /// 功能：1.播放背景音乐、音效（常驻音效、临时音效使用对象池动态创建、回收临时音效播放器）
    /// 
    /// 作者：毛俊峰
    /// 时间：2022.07.16
    /// 版本：1.0
    /// </summary>
    public class AudioManager : SingletonByMono<AudioManager>
    {
        /// <summary>
        /// 音效播放器
        /// </summary>
        private AudioSource m_AudioSourceEffect;
        /// <summary>
        /// 背景音乐播放器
        /// </summary>
        private AudioSource m_AudioSourceBGM;
        /// <summary>
        /// 临时音效播放器对象池
        /// </summary>
        private Pool<AudioSource> m_PoolAudioSourceEffectType;
        /// <summary>
        /// 音频类型
        /// </summary>
        public enum SoundType
        {
            /// <summary>
            /// 背景音乐
            /// </summary>
            BGM,
            /// <summary>
            /// 常驻音效（唯一）
            /// </summary>
            SoundEffect,
            /// <summary>
            /// 临时音效（多个，可并发播放） 
            /// </summary>
            SoundEffectTemp
        }
        private void Awake()
        {
            InitAudioSource();
        }
        /// <summary>
        /// 初始化播放器
        /// </summary>
        private void InitAudioSource()
        {
            m_AudioSourceBGM = new GameObject("audioBGM").AddComponent<AudioSource>();
            m_AudioSourceBGM.loop = true;
            m_AudioSourceBGM.transform.SetParent(transform);
            m_AudioSourceEffect = new GameObject("audioSourceEffect").AddComponent<AudioSource>();
            m_AudioSourceEffect.loop = false;
            m_AudioSourceEffect.transform.SetParent(transform);

            Transform audioSourceEffectTypeGroup = new GameObject("audioSourceEffectTypeGroup").transform;
            audioSourceEffectTypeGroup.SetParent(transform);
            AudioSource audioSourceEffectType = new GameObject("audioSourceEffectType").AddComponent<AudioSource>();


            m_PoolAudioSourceEffectType = new Pool<AudioSource>(() =>
            {
                //回调 获取对象-创建新对象后回调
                AudioSource newObj = UnityEngine.Object.Instantiate<AudioSource>(audioSourceEffectType);
                newObj.transform.SetParent(audioSourceEffectTypeGroup.transform);
                newObj.gameObject.SetActive(false);
                return newObj;
            }, (AudioSource audioSource) =>
            {
                //获取对象后回调
                audioSource.gameObject.SetActive(true);
            }, (AudioSource audioSource) =>
            {
                //回收对象时回调
                audioSource.gameObject.SetActive(false);
            }, 0);
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="soundType">音频类型</param>
        /// <param name="clip">音频实例</param>
        /// <param name="action">播放完毕回调</param>
        public void Play(SoundType soundType, AudioClip clip, Action action = null)
        {
            AudioSource audioSource = GetAudioSoucreBySoundType(soundType);
            audioSource.clip = clip;
            audioSource.Play();


            //播放完毕延时调用
            //有bug 不能用audioSource.clip.length 判断音效播放完毕 待解决
            DelayTool.GetInstance.Delay(audioSource.clip.length, () =>
            {
                //临时音效 播放完毕自动回收
                if (soundType == SoundType.SoundEffectTemp)
                {
                    m_PoolAudioSourceEffectType.Recycle(audioSource);
                }
                action?.Invoke();
            });
        }



        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="soundType"></param>
        public void Pause(SoundType soundType)
        {
            if (soundType != SoundType.SoundEffectTemp)
            {
                GetAudioSoucreBySoundType(soundType).Pause();
            }
            else
            {
                List<AudioSource> soundEffectTempArr = m_PoolAudioSourceEffectType.GetUsingObjs;
                foreach (AudioSource item in soundEffectTempArr)
                {
                    item.Pause();
                }


            }
        }


        /// <summary>
        /// 继续播放
        /// </summary>
        /// <param name="soundType"></param>
        public void Play(SoundType soundType)
        {
            if (soundType != SoundType.SoundEffectTemp)
            {
                GetAudioSoucreBySoundType(soundType).Play();
            }
            else
            {
                List<AudioSource> soundEffectTempArr = m_PoolAudioSourceEffectType.GetUsingObjs;
                foreach (AudioSource item in soundEffectTempArr)
                {
                    item.GetComponent<AudioSource>().Play();
                }
            }
        }




        /// <summary>
        /// 获取播放器 根据音效类型
        /// </summary>
        /// <param name="soundType"></param>
        /// <returns></returns>
        private AudioSource GetAudioSoucreBySoundType(SoundType soundType)
        {
            AudioSource audioSource = null;
            switch (soundType)
            {
                case SoundType.BGM:
                    audioSource = m_AudioSourceBGM;
                    break;
                case SoundType.SoundEffect:
                    audioSource = m_AudioSourceEffect;
                    break;
                case SoundType.SoundEffectTemp:
                    audioSource = m_PoolAudioSourceEffectType.Allocate();
                    break;
                default:
                    break;
            }
            return audioSource;
        }
    }
}