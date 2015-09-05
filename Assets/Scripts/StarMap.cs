using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DajiaGame.Px;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 棋局数据和动画处理
/// </summary>
public class StarMap : MonoBehaviour
{
    private GameObject[,] starList = new GameObject[9, 9];
    public Transform startPt;
    public GameObject selectedStar;

    private List<GameObject> openList;
    private List<GameObject> closeList;

    // 可消除的 星星列表
    public List<GameObject> eliminableList;


    // 游戏结束的事件
    public UnityEvent GameOverHandler;
    // 垂直移动的，向下
    private List<Dictionary<string, object>> verticalMoveList;

    // 水平移动的，向左
    private List<Dictionary<string, object>> horizontalMoveList;


    /// <summary>
    /// 搜索成块的相连星星
    /// </summary>
    public void SearchStarChunk()
    {
        if (null == selectedStar) return;
        openList = new List<GameObject>();
        closeList = new List<GameObject>();
        eliminableList = new List<GameObject>();

        openList.Add(selectedStar);
        eliminableList.Add(selectedStar);
        while (openList.Count > 0)
        {
            CheckOpenList();
        }
        GameManager.Instance.UpdateText();
    }

    /// <summary>
    /// 对单个星星搜索上下左右四个位置的邻居，看是否相同颜色
    /// </summary>
    private void CheckOpenList()
    {
        GameObject popCube = PopOpenList();
        if (null == popCube)
            return;
        Star star = popCube.GetComponent<Star>();
        if (closeList.IndexOf(popCube) != -1)
            return;
        if (star.type != selectedStar.GetComponent<Star>().type)
            return;

        closeList.Add(popCube);
        int curRow = star.row;
        int curColumn = star.column;
        GameObject contiguousCube;

        //上边星星
        if (curRow - 1 >= 0)
        {
            contiguousCube = starList[curRow - 1, curColumn];
            PushOpenList(contiguousCube);
        }
        //右边星星
        if (curColumn + 1 < 9)
        {
            contiguousCube = starList[curRow, curColumn + 1];
            PushOpenList(contiguousCube);
        }
        //下边星星
        if (curRow + 1 < 9)
        {
            contiguousCube = starList[curRow + 1, curColumn];
            PushOpenList(contiguousCube);
        }
        //左边星星
        if (curColumn - 1 >= 0)
        {
            contiguousCube = starList[curRow, curColumn - 1];
            PushOpenList(contiguousCube);
        }
    }


    /// <summary>
    /// 对开放列表的push做校验
    /// </summary>
    /// <param name="star"></param>
    private void PushOpenList(GameObject star)
    {
        if (null == star) return;
        if (closeList.IndexOf(star) != -1) return;
        if (star.GetComponent<Star>().type == selectedStar.GetComponent<Star>().type)
        {
            openList.Add(star);
            if (eliminableList.IndexOf(star) == -1)
            {
                eliminableList.Add(star);
            }
        }
        GameManager.Instance.UpdateText();
    }


    /// <summary>
    /// 删除openList的最后一个元素并返回
    /// </summary>
    /// <returns></returns>
    private GameObject PopOpenList()
    {
        if (openList.Count == 0)
        {
            return null;
        }
        int index = openList.Count - 1;
        GameObject popCube = openList[index];
        openList.RemoveAt(index);
        return popCube;
    }

    #region   选中提示
    /// <summary>
    /// 可消除的(Eliminable)星星块发光效果
    /// </summary>
    public void LightEliminableStar()
    {
        if (null == eliminableList || eliminableList.Count == 0)
            return;

        eliminableList.ForEach(LightSelectedStar);
    }

    /// <summary>
    /// 可消除的(Eliminable)星星块取消发光
    /// </summary>
    public void UnlightEliminableStar()
    {
        if (null == eliminableList || eliminableList.Count == 0)
            return;

        eliminableList.ForEach(NormalizeSelectedStar);
        eliminableList = new List<GameObject>();
        GameManager.Instance.UpdateText();
    }

    /// <summary>
    /// 对选中的star应用 少的透明度
    /// </summary>
    /// <param name="star"></param>
    void LightSelectedStar(GameObject star)
    {
        star.GetComponent<Image>().DOFade(0.3f, 0.2f);
    }

    /// <summary>
    /// 还原选中的star 的透明度
    /// </summary>
    /// <param name="star"></param>
    void NormalizeSelectedStar(GameObject star)
    {
        star.GetComponent<Image>().DOFade(1f, 0.2f);
    }
    #endregion

    #region 销毁内容
    /// <summary>
    /// 从场景中删除指定星星格子
    /// </summary>
    /// <param name="star"></param>
    void DestroyStar(GameObject star)
    {
        int row = star.GetComponent<Star>().row;
        int column = star.GetComponent<Star>().column;
        SetStarToMap(row, column, null);
        Destroy(star);
    }

    /// <summary>
    /// 当次点击格子后，从场景中销毁全部可消除的格子
    /// </summary>
    public void DestroyEliminableList()
    {
        eliminableList.ForEach(DestroyStar);
        GameManager.Instance.UpdateText();
    }
    #endregion

    public void SetStarToMap(int row, int columns, GameObject star)
    {
        starList[row, columns] = star;
    }

    public GameObject GetStarFromMap(int row, int columns)
    {
        return starList[row, columns];
    }


    /// <summary>
    /// * ------------------------------------
    /// * 格子的下落和左移计算和播放处理
    /// * ------------------------------------
    /// </summary>
    /// <summary>
    /// 星星的 下降动画
    /// </summary>
    /// <returns></returns>
    public IEnumerator StarFallDown()
    {
        verticalMoveList = new List<Dictionary<string, object>>();
        for (int c = 0; c < 9; c++)
        {
            for (int r = 8; r >= 0; r--)
            {
                CheckStarVertically(r, c);
            }
        }
        // 下降吧
        foreach (var dictionary in verticalMoveList)
        {
            GameObject obj = dictionary["gameobject"] as GameObject;
            float newy = (float)dictionary["newy"];
            obj.transform.DOLocalMoveY(newy, 1f).SetDelay(.1f);
        }
        if (verticalMoveList.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
        }
        // 星星左移
        StarLeftMove();
        if (CheckGameOver())
        {
            if (GameOverHandler != null)
            {
                GameOverHandler.Invoke();
            }
        }
    }


    /// <summary>
    /// 查看指定星星格子是否为空，是的话则往上搜索非空格子，并用来填补当前格
    /// </summary>
    /// <param name="row"></param>
    /// <param name="columns"></param>
    void CheckStarVertically(int row, int columns)
    {
        if (starList[row, columns] != null)
            return;
        var size = GameManager.Instance.stars[0].GetComponent<RectTransform>().sizeDelta;
        for (int uprow = row; uprow >= 0; uprow--)
        {
            if (starList[uprow, columns] != null)
            {
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("gameobject", starList[uprow, columns]);
                dictionary.Add("newy", startPt.localPosition.y - row * size.y/*0.5f*/);
                verticalMoveList.Add(dictionary);

                starList[row, columns] = starList[uprow, columns];
                starList[row, columns].GetComponent<Star>().row = row;
                starList[row, columns].GetComponent<Star>().type = starList[uprow, columns].GetComponent<Star>().type;
                starList[row, columns].name = "star_" + row + "_" + columns;
                starList[uprow, columns] = null;
                break;
            }
        }
    }




    /// <summary>
    /// 星星的 左移操作动画
    /// </summary>
    void StarLeftMove()
    {
        int curColumn = 0;
        while (curColumn < 9)
        {
            bool isCurColumnEmpty = true;
            horizontalMoveList = new List<Dictionary<string, object>>();
            for (int curRow = 0; curRow < 9; curRow++)
            {
                if (starList[curRow, curColumn] != null)
                {
                    isCurColumnEmpty = false;
                    break;
                }
            }
            if (isCurColumnEmpty)
            {
                // 计算有多少连续空列，再依次往左移动多少列
                int emptyColumnCounts = CalcEmptyColumns(curColumn);
                // Debug.Log("start column:" + curColumn + ", empty column:" + emptyColumnCounts);
                MoveStarToLeft(curColumn, emptyColumnCounts);
                curColumn += emptyColumnCounts;
            }
            else
            {
                curColumn += 1;
            }
            foreach (var dictionary in horizontalMoveList)
            {
                GameObject obj = dictionary["gameobject"] as GameObject;
                float newx = (float)dictionary["newx"];
                obj.transform.DOLocalMoveX(newx, 1f).SetDelay(.1f);
            }
        }
    }


    /// <summary>
    /// 将指定列数右边的格子往左移动
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="emptyColumnCounts"></param>
    void MoveStarToLeft(int columns, int emptyColumnCounts)
    {
        var size = GameManager.Instance.stars[0].GetComponent<RectTransform>().sizeDelta;
        for (int rightColumn = columns + 1; rightColumn < 9; rightColumn++)
        {
            for (int r = 0; r < 9; r++)
            {
                if (starList[r, rightColumn] == null) continue;

                int columnAfterMove = rightColumn - emptyColumnCounts;
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("gameobject", starList[r, rightColumn]);
                dictionary.Add("newx", startPt.localPosition.x + (columnAfterMove) * size.x);
                horizontalMoveList.Add(dictionary);

                starList[r, columnAfterMove] = starList[r, rightColumn];
                starList[r, columnAfterMove].GetComponent<Star>().column = columnAfterMove;
                starList[r, columnAfterMove].GetComponent<Star>().type = starList[r, rightColumn].GetComponent<Star>().type;
                starList[r, columnAfterMove].name = "star_" + r + "_" + columnAfterMove;
                starList[r, rightColumn] = null;
            }
        }
    }


    /// <summary>
    /// 计算连续的空列数
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    int CalcEmptyColumns(int columns)
    {
        int emptyColumns = 1;
        for (int newc = columns + 1; newc < 9; newc++)
        {
            bool isEmptyColumn = true;
            for (int r = 0; r < 9; r++)
            {
                if (starList[r, newc] != null)
                {
                    isEmptyColumn = false;
                    break;
                }
            }
            if (isEmptyColumn)
            {
                emptyColumns += 1;
            }
            else
            {
                // 只计算连续的空列，只要遇到第一个非空列就跳出
                break;
            }
        }
        return emptyColumns;
    }

    /// <summary>
    /// 检查游戏 是否结束
    /// </summary>
    /// <returns></returns>
    bool CheckGameOver()
    {
        bool gameOver = true;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                bool eliminable = CheckStarEliminable(r, c);
                if (eliminable)
                {
                    gameOver = false;
                }
            }
        }
        return gameOver;
    }

    /// <summary>
    /// 检查星星  是不是可以消除的
    /// </summary>
    /// <param name="row"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    bool CheckStarEliminable(int row, int columns)
    {
        bool eliminable = false;
        GameObject star = starList[row, columns];
        if (star == null) return eliminable;

        StarType targetType = star.GetComponent<Star>().type;
        bool targetTypeMatched;
        //上边星星
        if (row - 1 >= 0)
        {
            targetTypeMatched = IsStarMatchType(row - 1, columns, targetType);
            if (targetTypeMatched) return true;
        }
        //右边星星
        if (columns + 1 < 9)
        {
            targetTypeMatched = IsStarMatchType(row, columns + 1, targetType);
            if (targetTypeMatched) return true;
        }
        //下边星星
        if (row + 1 < 9)
        {
            targetTypeMatched = IsStarMatchType(row + 1, columns, targetType);
            if (targetTypeMatched) return true;
        }
        //左边星星
        if (columns - 1 >= 0)
        {
            targetTypeMatched = IsStarMatchType(row, columns - 1, targetType);
            if (targetTypeMatched) return true;
        }
        return eliminable;
    }

    // 星星的类型是否一致
    bool IsStarMatchType(int row, int columns, StarType targetType)
    {
        if (starList[row, columns] == null) return false;
        if (starList[row, columns].GetComponent<Star>().type == targetType)
        {
            return true;
        }
        return false;
    }

    // 没有可以消除后， 清楚所有星星
    public void ClearAllStar()
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (starList[r, c] != null)
                {
                    DestroyStar(starList[r, c]);
                }
            }
        }
    }

    // 调试用， 输出所有星星内容
    public void PrintStarList()
    {
        for (int c = 0; c < 9; c++)
        {
            for (int r = 0; r < 9; r++)
            {
                string postfix = "[null]";
                if (starList[r, c] != null)
                {
                    Star star = starList[r, c].GetComponent<Star>();
                    postfix = "[" + star.row + "," + star.column + "]";
                }
                Debug.Log("(" + r + "," + c + "," + starList[r, c] + postfix + ")");
            }
            Debug.Log("\n");
        }
    }
}
