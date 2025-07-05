using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeTop : UINode
{
    public Text coinCount;
    private void Start()
    {
        OnCoinUpdate();
    }

    [UIButton("SettingButton")]
    private void OnClickedSetting()
    {
        UIManager.OpenView<UISetting>();
    }

    [UIListener(UIEvent.CoinUpdate)]
    private void OnCoinUpdate()
    {
        Debug.Log(MockData.CoinCount);
        coinCount.text = $"{MockData.CoinCount}";
    }
}