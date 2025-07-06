using Constant;
using Framework.Attribute;
using Framework.UI;

namespace UI.UIToast
{
    [UIMetaData(UILayer.Toast)]
    public class UIToast : UIView
    {
        public UIToastMessage toastMessage;
        public static UIToast Instance;

        public override void OnRefresh()
        {
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void AppendToast(string message)
        {
            var toast = Instantiate(toastMessage, transform);
            toast.Initialize(message);
        }
    }
}