using UnityEngine;

[UIMetaData(UILayer.Background)]
public class UIHome : UIView
{
    public UIHomeTop top;
    public override void OnRefresh()
    {
        Debug.Log("UIHome OnRefresh");
    }

    [UIButton("ShopButton")]
    private void OnClickedShop()
    {
        UIManager.OpenView<UIShop>(new UIShopData {ShopLevel = 0});
    }
    
    [UIButton("RandomShopButton")]
    private void OnClickedRandomShop()
    {
        UIManager.OpenView<UIShop>(new UIShopData {ShopLevel = Random.Range(0, 5)});
    }
    
    [UIButton("QueueViewButton")]
    private void OnClickedQueueView()
    {
        UIManager.AppendView<UIQueue1>();
        UIManager.AppendView<UIQueue2>();
    }
}