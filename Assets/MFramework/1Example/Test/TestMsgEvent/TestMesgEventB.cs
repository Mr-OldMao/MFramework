using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 标题：消息系统测试  调用方
/// 功能：调用TestMesgEventA模块
/// 作者：毛俊峰
/// 时间：2022.
/// 版本：1.0
/// </summary>
public class TestMesgEventB : AbMFrameworkBase
{
    private void Start()
    {
        //1.使用静态方法直接调用TestMesgEventA模块
        MsgEvent.SendMsg("TestMesgEventA_MsgNameTest1", "我是b1");
        MsgEvent.SendMsg("TestMesgEventA_MsgNameTest2", "我是b2");

        //2.使用AbMFrameworkBase框架调用TestMesgEventA模块
        base.SendMsg("TestMesgEventA_MsgNameTest3", "我是b3");
    }
}