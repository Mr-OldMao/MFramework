using UnityEngine;
using MFramework;
public class UIFormTest : UIFormBase
{
    public override UILayerType GetUIFormLayer
    {
        get => UILayerType.Common;
        protected set => _ = UILayerType.Common;
    }
    public override string AssetPath
    {
        get => AssetPathRootDir + "/Main/UIFormTest.prefab";
        protected set => _ = AssetPathRootDir + "/Main/UIFormTest.prefab";
    }

    public void Test()
    {
        Debugger.Log("UIFormTest Test()", LogTag.MF);
    }

    public override void Show()
    {
        base.Show();
        Debugger.Log("UIFormTest Show()", LogTag.MF);
    }
    protected override void InitMapField()
    {

    }
    protected override void RegisterUIEvnet()
    {

    }
}
