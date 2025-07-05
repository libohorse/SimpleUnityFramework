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