using System.Collections.Generic;
using Constant;
using Data;
using Framework.Attribute;
using Framework.Manager;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIShop
{
    public class UIShopData : UIData
    {
        public int ShopLevel;
    }

    [UIMetaData(UILayer.Normal)]
    public class UIShop : UIViewWithData<UIShopData>
    {
        public Text title;
        public List<Image> imgList;
        public Button buyItemButton;
        public Button sellItemButton;

        public override void OnRefresh()
        {
            title.text = $"{Data.ShopLevel + 1}级商店";
            buyItemButton.interactable = MockData.CoinCount >= 100 * (Data.ShopLevel + 1);
            sellItemButton.interactable = MockData.CoinCount <= 5000;

            for (var i = 0; i < imgList.Count; i++)
            {
                imgList[i].color = Data.ShopLevel >= i ? Color.red : Color.white;
            }
        }

        [UIButton("CloseButton")]
        private void OnClickedClose()
        {
            UIManager.CloseView(this);
        }

        [UIButton("BuyItemButton")]
        private void OnClickedBuyItem()
        {
            MockData.CoinCount -= 100 * (Data.ShopLevel + 1);
            UIManager.TriggerEvent(UIEvent.CoinUpdate);
            OnRefresh();
        }

        [UIButton("SellItemButton")]
        private void OnClickedSellItem()
        {
            MockData.CoinCount += 100 * (Data.ShopLevel + 1);
            UIManager.TriggerEvent(UIEvent.CoinUpdate);
            OnRefresh();
        }
    }
}