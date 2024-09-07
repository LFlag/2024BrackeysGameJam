using System;
using Framework;
using UnityEngine;
using Utility;

public class EventCenterExample : MonoBehaviour
{
    public struct GameStart : IEvent
    {
        public DateTime NowTime => DateTime.Now;
    }
    
    void Start()
    {
        InputMgr.Instance.SetInputState(true); // 打开输入检测
        
        EventCenter.Instance.Register<GameStart>(GameStartTrigger)
            .UnRegisterWhenGameObjectOnDestroy(gameObject);
        
        EventCenter.Instance.Register<GetKeyDown>((_)=>{DebugUtil.Log($"{_.Key}", "cyan");})
            .UnRegisterWhenGameObjectOnDestroy(gameObject);
        
        InputMgr.Instance.AddKeyDownListener(KeyCode.A);
    }

    void GameStartTrigger(GameStart gameStart)
    {
        DebugUtil.Log(gameStart.NowTime, "cyan");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventCenter.Instance.Broadcast(new GameStart());
        }
    }
}
