[UIMetaData(UILayer.Normal)]
public class UIQueue2 : UIView
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