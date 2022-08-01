# API手册







[TOC]





# 0ReadMe

API手册、开发文档、版本记录



# 1Example

测试案例

## Test

测试类

### Resources

测试所用的资源

## ExampleTest.unity

测试场景



# 2Framework

框架主体

## 0Manager

管理器

AudioManager.cs

MainManager.cs

UIManager.cs

## 1Utility

工具类类库

### MsgEvent

消息系统

脚本：MsgEvent.cs

核心功能：管理消息的注册、注销、调用

规则：注册消息--允许同一个消息名下有一个或者多个无参或者带参事件，调用消息--根据消息名可调用旗下所有无参消息、所有带参消息、所有消息

核心API：

```c#
//注册消息
        //注册无参消息
        MsgEvent.RegisterMsgEvent("MsgName1", () => { Debug.Log("无参事件1.1"); }, "当前无参消息事件文字描述1.1(可忽略不填)");
        MsgEvent.RegisterMsgEvent("MsgName1", () => { Debug.Log("无参事件1.2"); }, "当前无参消息事件文字描述1.2(可忽略不填)");
        MsgEvent.RegisterMsgEvent("MsgName1", Method1, "当前无参消息事件文字描述1.3");
        //注册带参消息
        MsgEvent.RegisterMsgEvent("MsgName2", (object p) => { Debug.Log("带参事件2.1 " + p); }, "当前带参消息事件文字描述2.1");
        MsgEvent.RegisterMsgEvent("MsgName2", (object p) => { Debug.Log("带参事件2.2 " + p); }, "当前带参消息事件文字描述2.2");
        MsgEvent.RegisterMsgEvent("MsgName2", Method2, "当前带参消息事件文字描述2.3");
//注销消息
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
//发送消息
        //发送无参消息
        MsgEvent.SendMsg("MsgName1");
        //发送带参消息
        object objParam = "TestSendMsg";
        MsgEvent.SendMsg("MsgName2",objParam);
```



### Pool

对象池

核心脚本：Pool.cs

核心功能：获取对象、回收对象、获取正在使用的对象实例

核心API：

```c#
//初始化		
            GameObject objRes = null;
            Transform objResParent = null;
            Pool<GameObject> pool = new Pool<GameObject>(() =>
            {
                //回调 获取对象-创建新对象后回调
                GameObject newObj = UnityEngine.Object.Instantiate<GameObject>(objRes);
                newObj.transform.SetParent(objResParent);
                newObj.gameObject.SetActive(false);
                return newObj;
            }, (GameObject cube) =>
            {
                //获取对象后回调
                //cube.gameObject.SetActive(true);
            }, (GameObject cube) =>
            {
                //回收对象时回调
                //cube.gameObject.SetActive(false);
            }, 10);//预先创建对象的个数

//分配一个对象
            GameObject obj = m_PoolObj.Allocate();
//回收一个指定对象
            bool recycleObjSuc = m_PoolObj.Recycle(obj);
//回收所有正在使用的对象
            bool recycleObjsSuc = m_PoolObj.RecycleAll();
//获取所有正在使用的对象
            List<GameObject> objs = m_PoolObj.GetUsingObjs();
//回收指定正在使用的对象通过场景游戏对象名
            GameObject findObj = m_PoolObj.GetUsingObjsByObjName("sceneGameObjectName");
```



### RefCounter

引用计数器

AbRefCounter.cs 

### ReourcesLoader

资源管理器

ReourcesLoader.cs

### Singleton

单例

SingletonByMono.cs

### Tool

工具类

UnityTool.cs







## 2Extension

UnityEngine静态类扩展

## 3Editor

Unity编辑器扩展

4.Base

待定

## Resources

框架资源









