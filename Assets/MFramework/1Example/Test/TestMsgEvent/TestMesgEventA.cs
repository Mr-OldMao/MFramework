using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 标题：消息系统测试  外部模块引用本类方法
/// 功能：注册消息
/// 作者：毛俊峰
/// 时间：2022.07.16
/// 版本：1.0
/// </summary>
public class TestMesgEventA : MonoBehaviour
{
    private void Awake()
    {
        ////1.使用静态方法直接注册消息事件，注意需要手动注销事件
        //MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", (p) => { Debug.Log("TestMesgEventA_MsgNameTest1  " + p); });
        //MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", (p) => { Debug.Log("TestMesgEventA_MsgNameTest1 " + p); });
        //MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", Method);
        //MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest2", (p) => { Debug.Log("TestMesgEventA_MsgNameTest2 " + p); });
        ////注销事件
        ////MsgEvent.GetInstance.UnregisterMsgEvent("TestMesgEventA_MsgNameTest1", Method);
        ////MsgEvent.GetInstance.UnregisterMsgEvent("TestMesgEventA_MsgNameTest1");


        ////2.使用AbMFrameworkBase框架注册事件
        //base.RegisterMsgEvent("TestMesgEventA_MsgNameTest3", ((p) => { Debug.Log("使用AbMFrameworkBase框架注册事件 P:" + p); }));

        RegisterMsgEvent();

    }

    /// <summary>
    /// 注册消息
    /// </summary>
    private void RegisterMsgEvent()
    {
        //注册消息
        //注册无参消息
        MsgEvent.RegisterMsgEvent("MsgName1", () => { Debug.Log("无参事件1.1"); }, "当前无参消息事件文字描述1.1(可忽略不填)");
        MsgEvent.RegisterMsgEvent("MsgName1", () => { Debug.Log("无参事件1.2"); }, "当前无参消息事件文字描述1.2(可忽略不填)");
        MsgEvent.RegisterMsgEvent("MsgName1", Method1, "当前无参消息事件文字描述1.3");
        //注册带参消息
        MsgEvent.RegisterMsgEvent("MsgName2", (object p) => { Debug.Log("带参事件2.1 " + p); }, "当前带参消息事件文字描述2.1");
        MsgEvent.RegisterMsgEvent("MsgName2", (object p) => { Debug.Log("带参事件2.2 " + p); }, "当前带参消息事件文字描述2.2");
        MsgEvent.RegisterMsgEvent("MsgName2", Method2, "当前带参消息事件文字描述2.3");
    }
    private void Method1()
    {
        Debug.Log("无参事件Method1 ");
    }
    private void Method2(object obj)
    {
        Debug.Log("带参事件Method2 " + obj);
    }

    private void OnDestroy()
    {
        UnregisterMsgEvent();
    }

    /// <summary>
    /// 注销消息
    /// </summary>
    private void UnregisterMsgEvent()
    {
        //注销消息名下的 所有无参消息
        MsgEvent.UnregisterMsgEventNotParam("MsgName1");
        //注销消息名下的 所有带参消息
        MsgEvent.UnregisterMsgEventParam("MsgName1");
        //注销消息名下的 指定无参消息 一般用于注销 注册过非lamda表达式的事件消息
        MsgEvent.UnregisterMsgEvent("MsgName1", Method1);
        //注销消息名下的 指定带参消息 一般用于注销 注册过非lamda表达式的事件消息
        MsgEvent.UnregisterMsgEvent("MsgName2", Method2);
        //注销消息名下的  所有消息（带参和无参）
        MsgEvent.UnregisterMsgEventAll("MsgName1");
    }


}