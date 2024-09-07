using Framework;

public class Global : BaseManagerMono<Global>
{
    protected override void Init()
    {
        InputMgr.Instance.SetInputState(true); // 打开输入检测
    }
}
