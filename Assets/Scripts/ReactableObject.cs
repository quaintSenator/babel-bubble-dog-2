using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactableObject : MonoBehaviour
{
    BoxCollider2D boxCollider;
    [SerializeField] public GameObject reactShowingPoint;
    [SerializeField] public bool showOperationPanel = true;
    
    /// <summary>
    /// 确保可交互区域纵向是无限的
    /// </summary>
    void ReactAreaPrefix()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        
        /*var oriSize = boxCollider.size;
        var oriCenter = boxCollider.center;
        boxCollider.size.Set(oriSize.x, 99999, oriSize.z);
        boxCollider.center.Set(oriCenter.x, 0, oriCenter.z);*/ //纵向延展代码注掉了
        boxCollider.isTrigger = true;
    }
    
    //同一时间，玩家可能和若干个ReactableObject叠在一起吗？
    /*
     * 以下是设计意图
     * 
     * 看起来，柴火是可以交互的，且其自身会被移动，那么就不太能保证交互区域不彼此重叠了。
     * （一个邪道是可以选择在丢下柴火的时候通过计算，总是把柴火扔到一个安全的位置，以此来维护可交互区域不彼此重叠这个铁律。）
     * 如果可交互区域是可能彼此重叠的，那么就面临这样一个问题：如果玩家位于1和2区域的交界之处，玩家选择了某个动作时，
     * 应当如何决定玩家是在对什么执行行动？你可能觉得飞盘和柴火这个例子很好办，
     * 特别去判断玩家在什么高度，或者给飞盘和柴火都加上collider看看玩家碰撞了哪一个，
     * 或者计算这两个物体哪个距离玩家近，总之一定能处理。
     * 但这只是一种特殊情况，更普遍的情况是难以预料的，比如玩家把A物体丢到了和B物体完全重叠的位置，此时玩家对于这两个物体执行某个行动X（比如A和B都可以叼），
     * 这两个行为都是有意义的，那个时候要怎么判定玩家此时的X究竟是针对A还是针对B？
     * 一种方法是上面的邪道，但也是通解，非常的方便。
     * 首先在制作关卡时，手动确保静态的物体的可交互范围彼此不重叠。但这样一来柴火从小狗口中脱落的时候会自动掉到一个稍微有点远的地方（会制作一个程序动画，看起来就像掉到一个特定位置），
     * 这样做的弊端在于如果有的交互区域特别大，这样做会看起来很奇怪。
     *
     * 为此拆分ReactableObject和ReactableObject. 
     * Area仅仅用于提示可以显示交互面板了，而实际交互的对象是ReactableObject。
     * 这样一来，点击行动按钮时，还需要增加一段获取行动对象的逻辑。这个逻辑应当写在每个动作里，跳跃就完全不需要对象。
     * 
     */
    // Start is called before the first frame update
    void Start()
    {
        ReactAreaPrefix();
        reactShowingPoint.SetActive(false);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {

    }
    
    public void OnTriggerExit2D(Collider2D other)
    {

    }

    public virtual void SetOutline(bool isOutline)
    {
       
    }

    public virtual bool GetShowOperationPanel()
    {
        return showOperationPanel;
    }
}
