// 不继承Mono的单例基类
public class BaseManager<T> where T : new()
{
    private static T _instance;

    public static T Instance => _instance ??= new T();
}
