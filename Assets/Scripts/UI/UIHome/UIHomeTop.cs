using Constant;
using Data;
using Framework.Attribute;
using Framework.Manager;
using Framework.UI;
using UnityEngine.UI;

namespace UI.UIHome
{
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
            UIManager.OpenView<UISetting.UISetting>();
        }

        [UIListener(UIEvent.CoinUpdate)]
        private void OnCoinUpdate()
        {
            coinCount.text = $"{MockData.CoinCount}";
        }
    }
}