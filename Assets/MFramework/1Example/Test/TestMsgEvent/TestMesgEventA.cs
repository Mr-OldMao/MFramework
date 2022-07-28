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
public class TestMesgEventA : AbMFrameworkBase
{
    private void Awake()
    {
        //1.使用静态方法直接注册消息事件，注意需要手动注销事件
        MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", (p) => { Debug.Log("TestMesgEventA_MsgNameTest1  " + p); });
        MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", (p) => { Debug.Log("TestMesgEventA_MsgNameTest1 " + p); });
        MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest1", Method);
        MsgEvent.RegisterMsgEvent("TestMesgEventA_MsgNameTest2", (p) => { Debug.Log("TestMesgEventA_MsgNameTest2 " + p); });
        //注销事件
        //MsgEvent.GetInstance.UnregisterMsgEvent("TestMesgEventA_MsgNameTest1", Method);
        //MsgEvent.GetInstance.UnregisterMsgEvent("TestMesgEventA_MsgNameTest1");


        //2.使用AbMFrameworkBase框架注册事件
        base.RegisterMsgEvent("TestMesgEventA_MsgNameTest3", ((p) => { Debug.Log("使用AbMFrameworkBase框架注册事件 P:" + p); }));
    }

    private void Method(object obj)
    {
        Debug.Log("TestMesgEventA_MsgNameTest1 " + obj);
    }
}