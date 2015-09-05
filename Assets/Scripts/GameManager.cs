using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DajiaGame.Px;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    [HeaderAttribute("粉色、黄、绿、蓝、紫")]
    // 所有星星的父对象
    public GameObject ParentObj ;
    // 所有星星
    public GameObject[] stars;
    // 脚本
    public StarMap starMap;
    public UIPanel uiPanel;

    // 预测可以得到的分数是
    private int predictPoint;
    // 目前的累计得分
    private int totalPoint;

    // 存放历史成绩的
    private List<int> historyList = new List<int>() { 0 };

    void Start()
    {
        starMap.GameOverHandler.AddListener(OnGameOverHandler);
        NewGame();
        historyList = JsonReadFileTool.JsonToClasses<int[]>(PlayerPrefs.GetString("history")).ToList();
        if (historyList != null && historyList.Count > 0)
        {
            uiPanel.HistoryPoint.text = "历史最高：" + historyList[0];
        }
        else
        {
            historyList = new List<int>() {0};
        }
    }

    // 重新生成地图
    void NewGame()
    {
        var size = stars[0].GetComponent<RectTransform>().sizeDelta;
        totalPoint = 0;
        UpdateText();
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                int type = Random.Range(0, stars.Length);
                Vector3 offset = new Vector3(c * size.y, -r * size.x, 0f);
                Vector3 dest = starMap.startPt.localPosition + offset;
                GameObject starObj = Instantiate(stars[type], dest, Quaternion.identity) as GameObject;
                starObj.transform.SetParent(ParentObj.transform, false);
                starObj.name = "star_" + r + "_" + c;

                Star starInstance = starObj.GetComponent<Star>();
                starInstance.onClickStarEvent.AddListener(OnClickStarAction);     // 监听事件处理
                starInstance.Init((StarType)type, new Vector2(r, c));
                starMap.SetStarToMap(r, c, starObj);
            }
        }
    }

    // 点击星星Item的事件处理
    public void OnClickStarAction(GameObject obj)
    {
        Debug.Log("点击的star是：" + obj.name);
        OnClickStar(obj);
    }

    // 更新UI 的显示
    public void UpdateText/*OnGUI*/()
    {
        int count = (null == starMap.eliminableList) ? 0 : starMap.eliminableList.Count;
        uiPanel.StarCount.text = "星星个数: " + count; /*+ "\n"*/
        uiPanel.PredictPoint.text = "可获得分数：" + predictPoint;/*+ "\n"*/
        uiPanel.TotalPoint.text = "总分数：" + totalPoint;
    }

    // 重新开始按钮
    public void RestartOnClick()
    {
        starMap.ClearAllStar();
        NewGame();
    }

    // 游戏结束的 事件处理
    void OnGameOverHandler()
    {
        // 纪录  历史纪录
        int count = historyList.Count;   // [法1
        for (int i = 0; i < count; i++)// todo 注意 List的 Count 是动态变化的，插入一个就会更新大小
        {
            if (totalPoint > historyList[i])    // 按照从大到小的 顺序排序
            {
                historyList.Insert(i, totalPoint);
                // return;  // [法2
            }
        }
        PlayerPrefs.SetString("history", JsonReadFileTool.ClassesToString(historyList));
        uiPanel.HistoryPoint.text = "历史最高：" + + historyList[0];
        starMap.ClearAllStar();
    }


    /// <summary>
    /// 单击选择一个格子，高亮可消除的所有格子，1秒后取消选中状态
    /// 双击一个格子，触发消除相连的同颜色格子
    /// </summary>
    /// <param name="cube"></param>
    void OnClickStar(GameObject cube)
    {
        if (starMap.selectedStar && starMap.selectedStar == cube)
        { //double click
            PreprocessPopStar();
            return;
        }
        if (starMap.selectedStar)
        {
            CancelSelectImmediately();
        }
        starMap.selectedStar = cube;
        starMap.SearchStarChunk();

        if (starMap.eliminableList.Count > 1)
        {
            predictPoint = (int)(5 * Mathf.Pow(starMap.eliminableList.Count, 2));
            UpdateText();
        }
        //Debug.Log("可消除星星个数：" + starMap.eliminableList.Count + ", 分数：" + predictPoint);
        starMap.LightEliminableStar();
        // Invoke("CancelSelect", 1);   // 处于性能的角度，不使用
        DOVirtual.DelayedCall(1, CancelSelect);
    }

    /// <summary>
    /// 消星星预处理，能消除两个或以上的星星时才合法
    /// </summary>
    void PreprocessPopStar()
    {
        if (starMap.eliminableList.Count > 1)
        {
            totalPoint += predictPoint;
            predictPoint = 0;
            UpdateText();
            PopStars();
        }
        else
        {
            CancelSelectImmediately();
        }
    }


    void CancelSelectImmediately()
    {
        CancelSelect();
        // CancelInvoke("CancelSelect");
    }

    /// <summary>
    /// 取消选中状态
    /// </summary>
    void CancelSelect()
    {
        if (null == starMap.selectedStar)
            return;
        starMap.UnlightEliminableStar();
        starMap.selectedStar = null;
        predictPoint = 0;
        UpdateText();
    }

    /// <summary>
    /// 销毁符合清除规则的星星，余下的星星先往下坠落，再往左移动
    /// </summary>
    void PopStars()
    {
        starMap.DestroyEliminableList();
        StartCoroutine(starMap.StarFallDown());
        CancelSelectImmediately();
    }
}
