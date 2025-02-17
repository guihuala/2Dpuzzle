using UnityEngine;

/// <summary>
/// 通用的持久化单例基类，适用于继承自 MonoBehaviour 的类。
/// 该类在游戏场景之间保持唯一实例，并确保实例唯一性。
/// </summary>
/// <typeparam name="T">单例类型，必须继承自 MonoBehaviour。</typeparam>
public class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
        }

        DontDestroyOnLoad(gameObject);
    }
}