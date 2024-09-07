namespace Framework
{
    /// 事件中心 (供外部使用基于类型的事件系统 
    public class EventCenter
    {
        private static TypeEventSystem _instance;

        public static TypeEventSystem Instance => _instance ??= new TypeEventSystem();
    }
}