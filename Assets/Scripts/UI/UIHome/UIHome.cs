using Constant;
using Framework.Attribute;
using Framework.Manager;
using Framework.UI;
using UI.UIQueue;
using UI.UIShop;
using UnityEngine;

namespace UI.UIHome
{
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
            UIManager.OpenView<UIShop.UIShop>(new UIShopData {ShopLevel = 0});
        }
    
        [UIButton("RandomShopButton")]
        private void OnClickedRandomShop()
        {
            UIManager.OpenView<UIShop.UIShop>(new UIShopData {ShopLevel = Random.Range(0, 5)});
        }
    
        [UIButton("QueueViewButton")]
        private void OnClickedQueueView()
        {
            UIManager.AppendView<UIQueue1>();
            UIManager.AppendView<UIQueue2>();
        }
    }
}