namespace MFramework
{
    public class MFrameworkInfo
    {
        public const string Version = "v0.0.8";
        public const string LastModityDate = "20230901";
        public const string Author = "Mr_OldMao";
        public const string Github = "https://github.com/Mr-OldMao";
        public const string Email = "929764654@qq.com";

        public static void GetAllInfo()
        {
            UnityEngine.Debug.Log($"Version：{Version}，LastModityDate：{LastModityDate}，Author：{Author}，Github：{Github}，Email：{Email}");
        }
    }
}
