using System;
using System.Threading.Tasks;
using Framework.Manager;
using UI.UIHome;
using UI.UIToast;
using UnityEngine;

public class UILaunch : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            // 初始化
            var aa = AAManager.Initialize();
            var ui = UIManager.Initialize();
            await Task.WhenAll(aa, ui);
        
            // 常驻弹窗
            UIManager.OpenView<UIToast>();
            // 打开弹窗
            UIManager.OpenView<UIHome>();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}