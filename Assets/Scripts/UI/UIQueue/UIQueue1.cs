using Constant;
using Framework.Attribute;
using Framework.Manager;
using Framework.UI;

namespace UI.UIQueue
{
    [UIMetaData(UILayer.Normal)]
    public class UIQueue1 : UIView
    {
        public override void OnRefresh()
        {
        }
    
        [UIButton("CloseButton")]
        private void OnClickClose()
        {
            UIManager.CloseView(this);
        }
    }
}