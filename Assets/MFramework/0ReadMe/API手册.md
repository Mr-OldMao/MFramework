# API手册







[TOC]





# 0ReadMe

API手册、开发文档、版本记录



# 1Example

范例

## Test

测试案例

### TestAssets

存放测试所用的资源

#### Resources

Resources资源

### TestScript

测试类

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

标题：消息系统 功能：基于事件的消息系统，

目的：降低不同模块之间的相互调用的耦合性 

具体功能：消息注册(四种)：1.不带参、不带返回值 2.带参、不带返回值 3.不带参、带返回值 4.带参、带返回值 

​					消息发送(四种)：1.不带参、不带返回值 2.带参、不带返回值 3.不带参、带返回值 4.带参、带返回值

​					消息注销(九种)：1.注销指定类型指定消息 2.注销指定类型所有消息 3.注销所有类型所有消息

核心API：

```c#
//注册消息
        //注册不带参不带返回值消息
        MsgEvent.RegisterMsgEvent(MsgEventName.Test, () => { Debug.Log("无参、无返回、lamdba"); });
        //注册带参不带返回值消息
        MsgEvent.RegisterMsgEvent(MsgEventName.Test, (object obj) => { Debug.Log("有参、无返回、lamdba obj：" + obj); });
		//注册不带参带返回值消息
		MsgEvent.RegisterMsgEvent(MsgEventName.Test, () =>
        {
            Debug.Log("无参、有返回、lamdba");
            int value = 123;
            return value;
        });
		//注册无参无返回值消息
		MsgEvent.RegisterMsgEvent(MsgEventName.Test, (object friendID) =>
        {
            Debug.Log("有参、有返回、lamdba");
            string res = (string)friendID + "1111";
            return res;
        });

//注销消息
		//注销指定类型指定消息，仅针对非lamdba事件注册的消息使用
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, action);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, actionParam);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, func);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, funcParam);
        //注销指定类型所有消息，可用于注销使用Lamda和非Lamdba表达式注册过的事件消息
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, MsgEventType.NoParamNoReturn);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, MsgEventType.HasParamNoReturn);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, MsgEventType.NoParamHasReturn);
        MsgEvent.UnregisterMsgEvent(MsgEventName.Test, MsgEventType.HasParamHasReturn);
        //注销所有类型所有消息，可用于注销使用Lamda和非Lamdba表达式注册过的事件消息
        MsgEvent.UnregisterMsgEventAll(MsgEventName.Test);

//发送消息
        //发送无参无返回值消息
        MsgEvent.SendMsg(MsgEventName.Test);
        //发送有参无返回值消息
        MsgEvent.SendMsg(MsgEventName.Test, "测试发送有参无返回值消息");
        //发送无参有返回值消息
        MsgEvent.SendMsg(MsgEventName.Test, (obj) => { Debug.Log("测试发送无参有返回值 res：" + obj); });
        //发送有参有返回值消息
        MsgEvent.SendMsg(MsgEventName.Test, "1001", (obj) => { Debug.Log("测试发送有参有返回值 res：" + obj); });
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

### ResLoader

核心脚本：ResLoader.cs 	资源加载器

核心功能：管理Resource资源、ab包、ab包中具体资源的同步加载、异步加载、资源卸载

ResourcesRes.cs		Resource资源加载、卸载

AssetBundleRes.cs		AB包加载、卸载

AssetRes.cs		AB包中的具体资源加载、卸载

AbRes.cs		资源实现类的抽象基类

测试类： ExampleTest.unity	TestResLoader.cs

核心API：

```c#
///Resource资源
//同步加载Resources资源
	AudioClip al = ResLoader.LoadSync<AudioClip>(ResType.Resources, "Resources文件夹下的资源路径 例TestRes/Audio/bgm1");
//异步加载Reoueces资源
	ResLoader.LoadASync<AudioClip>(ResType.Resources,"Resources文件夹下的资源路径", (AudioClip res) => 
		{ 
			//TODO 
		});
//卸载Resources资源
	ResLoader.UnLoadAssets("Resources文件夹下的资源路径");


///AB包
//同步加载AB包
 	AssetBundle ab = ResLoader.LoadSync<AssetBundle>(ResType.AssetBundle,
		"ab包全路径 例Application.streamingAssetsPath/BuildAssetBundle/abaudio");
//异步加载AB包
	ResLoader.LoadASync<AssetBundle>( ResType.AssetBundle,"ab包全路径", (AssetBundle ab) => 
		{
    		//TODO
		},);
//卸载Resources资源
	ResLoader.UnLoadAssets("ab包全路径");


///AB包中的具体资源
//同步加载AB包中具体资源
    GameObject obj = ResLoader.LoadSync<GameObject>(ResType.Asset, "ab包全路径", "ab包中具体资源名 例:planePrefab");
//异步加载AB包中具体资源
	ResLoader.LoadAsync<GameObject>(ResType.Asset,(GameObject obj)=>
	{
        //TODO
    },"ab包全路径", "ab包中具体资源名");
//卸载AB包中具体资源
	ResLoader.UnLoadAssets("ab包全路径/ab包中具体资源名");
```







### Singleton

单例

SingletonByMono.cs

### Tool

工具类

UnityTool.cs







## 2Extension

UnityEngine静态类扩展

TransformExtension.cs

StringExtension.cs



## 3Editor

Unity编辑器扩展

### AutoCreateScript

#### EditorSceneObjMapEntity

Scene GameObject => EntityClass 场景游戏对象转实体类

核心脚本：EditorSceneObjMapEntity.cs 

目的：解决手动获取场景对象实例需要繁琐且重复的操作

功能：脚本自动化生成工具，根据场景中某游戏对象下的所有子对象，创建实体并与之对应，自动映射与关联

调用：选中菜单MFramework/脚本自动化工具，拖入根节点，设置脚本路径，点击生成，可自动获取根节点其下字段、属性、映射并生成脚本到指定路径

规则：场景游戏对象会被直接引用成为字段名，所以命名要规范，并且不要使用创建场景对象时引擎默认的名称会被屏蔽



#### EditorJsonMapEntity

Json =》 Object，Json反序列化实体类自动生成(基于Newtonsoft.Json) 脚本自动化生成工具

核心脚本：EditorJsonMapEntity.cs

目的：为解决根据Json数据进行反序列化，需要手动创建对应的对象实体进行数据缓存的问题

功能：输入Json格式的字符串，指定实体类的路径，自动生成Json反序列化所需的实体类

调用：Unity菜单栏  MFramework/脚本自动化工具/2.Json自动映射实体类



## Resources

框架资源



# StreamingAssets



## BuildAB

打包的AB文件容器







