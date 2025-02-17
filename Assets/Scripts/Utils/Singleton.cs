using UnityEngine;

/// <summary>
/// 通用的单例基类，适用于继承自 MonoBehaviour 的类。
/// </summary>
/// <typeparam name="T">单例类型，必须继承自 MonoBehaviour。</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance { get { return instance; } set { instance = value; } }

    protected virtual void Awake()
    {
        instance = this as T;
    }

}
