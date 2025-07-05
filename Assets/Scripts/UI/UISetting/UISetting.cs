using Constant;
using Framework.Attribute;
using Framework.Manager;
using Framework.UI;

namespace UI.UISetting
{
    [UIMetaData(UILayer.Normal)]
    public class UISetting : UIView
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