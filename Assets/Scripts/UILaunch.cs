using System;
using System.Threading.Tasks;
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
        
            // 打开弹窗
            UIManager.OpenView<UIHome>();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}