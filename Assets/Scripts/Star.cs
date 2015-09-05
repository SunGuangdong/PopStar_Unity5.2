using UnityEngine;
using System.Collections;
using DajiaGame.Px;
using UnityEngine.Events;

/// <summary>
/// 星星Item 脚本
/// </summary>
public class Star : MonoBehaviour
{
    // 带一个 参数的事件
    class OnClickStarEvent : UnityEvent<GameObject>
    {
    }

    // 类型
    public StarType type;

    // 位置
    public int row;
    public int column;

    // 声明事件
    public UnityEvent<GameObject> onClickStarEvent = new OnClickStarEvent();

    // 初始化函数
    public void Init(StarType starType, Vector2 pos)
    {
        type = starType;
        row = (int)pos.x;
        column = (int)pos.y;
    }

    // 点击star星星的处理函数
    public void OnClickStar()
    {
        onClickStarEvent.Invoke(gameObject);
    }
}
