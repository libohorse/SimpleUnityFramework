using UnityEngine;

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