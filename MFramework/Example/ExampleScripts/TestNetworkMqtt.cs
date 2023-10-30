using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标题：测试Mqtt网络通信协议
    /// 功能：
    /// 测试Mqtt功能初始化，与代理连接、注销，订阅主题，发布主题消息，监听消息回调
    /// 测试兼容性，是否兼容PC和Webglg环境
    /// 作者：毛俊峰
    /// 时间：2023.08.17
    /// </summary>
    public class TestNetworkMqtt : MonoBehaviour
    {
        private void OnEnable()
        {
            //选择MQTT协议通信的平台
#if !UNITY_EDITOR && UNITY_WEBGL
            NetworkMqtt.GetInstance.IsWebgl = true;
#else
            NetworkMqtt.GetInstance.IsWebgl = false;
#endif
            //创建[UnityObjectForWebglMsg]游戏对象，绑定脚本MqttWebglCenter.cs，用于h5向unity通信
            GameObject UnityObjectForWebglMsg = GameObject.Find("[UnityObjectForWebglMsg]");
            if (UnityObjectForWebglMsg == null)
            {
                UnityObjectForWebglMsg = new GameObject("[UnityObjectForWebglMsg]");
            }
            if (UnityObjectForWebglMsg.GetComponent<MqttWebglCenter>() == null)
            {
                UnityObjectForWebglMsg.AddComponent<MqttWebglCenter>();
            }
            UnityObjectForWebglMsg.SetActive(NetworkMqtt.GetInstance.IsWebgl);

            //注册MQTT登录成功回调函数
            NetworkMqtt.GetInstance.AddConnectedSucEvent(() =>
            {
                //订阅多个主题
                NetworkMqtt.GetInstance.Subscribe(MqttTopicName.TopicTest);
                NetworkMqtt.GetInstance.Subscribe("Test");
                NetworkMqtt.GetInstance.Subscribe("TopicTest1", "TopicTest2");
            });

            //初始化MQTT
            NetworkMqtt.GetInstance.Init(new MqttConfig()
            {
                clientIP = "10.5.24.28",
            });

            //监听消息回调
            NetworkMqtt.GetInstance.AddListenerSubscribe((string topic, string msg) =>
            {
                Debug.Log($"[Unity] Recv Msg , topic:" + topic + ",msg:" + msg);
            });

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //发送消息
                NetworkMqtt.GetInstance.Publish("Test", "Unity Send Test Msg:" + System.DateTime.Now.ToString());
                NetworkMqtt.GetInstance.Publish(MqttTopicName.TopicTest, "Unity Send TopicTest Msg:" + System.DateTime.Now.ToString());
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NetworkMqtt.GetInstance.DisConnect();
            }
        }
    }
}

