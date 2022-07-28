using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFramework
{
    /// <summary>
    /// 标题：测试简单对象池
    /// 功能：
    /// 作者：毛俊峰
    /// 时间：2022.
    /// 版本：1.0
    /// </summary>
    public class TestPool : MonoBehaviour
    {
        class TestObject { }

        public GameObject cube;
        public Transform cubeParent;


        private void Start()
        {
            //Test1();
            Test2();
        }


        private void Test1()
        {
            Pool<TestObject> poolSimple = new Pool<TestObject>(() => { return new TestObject(); }, null, null, 100);

            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            TestObject obj = poolSimple.Allocate();
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            poolSimple.Recycle(obj);
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            for (int i = 0; i < 10; i++)
            {
                poolSimple.Allocate();
            }
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);
        }

        private void Test2()
        {
            Pool<GameObject> poolSimple = new Pool<GameObject>(() =>
            {
                //回调 获取对象-创建新对象后回调
                GameObject newCube = Instantiate<GameObject>(cube);
                newCube.transform.SetParent(cubeParent);
                newCube.gameObject.SetActive(false);
                return newCube;
            }, (GameObject cube) =>
            {
                //获取对象后回调
                //cube.gameObject.SetActive(true);
            }, (GameObject cube) =>
             {
                 //回收对象时回调
                 //cube.gameObject.SetActive(false);
             }, 10);

            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            //分配对象
            GameObject obj = poolSimple.Allocate();
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            //回收对象
            poolSimple.Recycle(obj);
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            for (int i = 0; i < 3; i++)
            {
                poolSimple.Allocate();
            }
            Debug.Log("当前对象池 未使用的对象个数：" + poolSimple.GetCurUnuserObjCount + " 已使用对象个数：" + poolSimple.GetCurUningObjCount);

            //获取所有正在使用的对象
            poolSimple.GetUsingObjs[2].name = "222";

            //回收所有正在使用的对象
            poolSimple.RecycleAll();

        }
    }
}