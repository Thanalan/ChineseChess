using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 悔棋类
/// </summary>
public class ChessReseting
{
    private GameManager gameManager;

    //计数器，用来计数当前一共走了几步棋
    public int resetCount;
    //悔棋数组，用来存放所有已经走过的步数用来悔棋
    public Chess[] chessSteps;

    public ChessReseting()
    {
        gameManager = GameManager.Instance;
    }
    /// <summary>
    /// 记录每一步悔棋的具体棋子结构体
    /// </summary>
    public struct Chess
    {
        public ChessSteps from;
        public ChessSteps to;
        public GameObject gridOne;//来的位置所在格子
        public GameObject gridTwo;//去的位置所在格子
        public GameObject chessOne;
        public GameObject chessTwo;
        public int chessOneID;
        public int chessTwoID;
    }
    /// <summary>
    /// 棋子位置
    /// </summary>
    public struct ChessSteps
    {
        public int x;
        public int y;
    }
    /// <summary>
    /// 悔棋方法
    /// </summary>
    public void ResetChess()
    {
        gameManager.HideLastPositionUI();
        gameManager.HideClickUI();
        gameManager.HideCanEatUI();
        if (gameManager.chessPeople==1)//单机PVE
        {
            //每次悔两步棋
            if (resetCount<=1)
            {
                return;
            }
            //获取当前索引
            int f = resetCount - 1;
            int s = resetCount - 2;
            //获取黑色棋子原来的位置
            int oneID = chessSteps[f].chessOneID;
            //获取黑色棋子移动到的位置
            int twoID = chessSteps[f].chessTwoID;
            //获取红色棋子原来的位置
            int threeID = chessSteps[s].chessOneID;
            //获取红色棋子移动到的位置
            int fourID = chessSteps[s].chessTwoID;
            //悔棋的第一步，黑色方
            GameObject b_gridOne, b_gridTwo, b_chessOne, b_chessTwo;
            b_gridOne = chessSteps[f].gridOne;
            b_gridTwo = chessSteps[f].gridTwo;
            b_chessOne = chessSteps[f].chessOne;
            b_chessTwo = chessSteps[f].chessTwo;
            //悔棋的第二步，红色方
            GameObject r_gridOne, r_gridTwo, r_chessOne, r_chessTwo;
            r_gridOne = chessSteps[s].gridOne;
            r_gridTwo = chessSteps[s].gridTwo;
            r_chessOne = chessSteps[s].chessOne;
            r_chessTwo = chessSteps[s].chessTwo;
            //黑色方吃子
            if (b_chessTwo!=null)
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessTwo.transform.SetParent(b_gridTwo.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                b_chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;
                //红色方吃子
                if (r_chessTwo!=null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;
                }
                //红色方移动
                else
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = 0;
                }
            }
            //黑色方走子
            else
            {
                b_chessOne.transform.SetParent(b_gridOne.transform);
                b_chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;
                //红色方吃子
                if (r_chessTwo != null)
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessTwo.transform.SetParent(r_gridTwo.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    r_chessTwo.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = fourID;
                }
                //红色方移动
                else
                {
                    r_chessOne.transform.SetParent(r_gridOne.transform);
                    r_chessOne.transform.localPosition = Vector3.zero;
                    gameManager.chessBoard[chessSteps[s].from.x, chessSteps[s].from.y] = threeID;
                    gameManager.chessBoard[chessSteps[s].to.x, chessSteps[s].to.y] = 0;
                }
            }
            gameManager.chessMove = true;
            resetCount -= 2;
            gameManager.gameOver = false;
            UIManager.Instance.ShowTip("红方走");
            gameManager.checkmate.JudgeIfCheckmate();
            chessSteps[f] = new Chess();
            chessSteps[s] = new Chess();

        }
        else if(gameManager.chessPeople==2)//单机PVP
        {
            if (resetCount<=0)//没有下一步棋，不存在悔棋
            {
                return;
            }
            int f = resetCount - 1;//因为索引是从0开始的
            int oneID = chessSteps[f].chessOneID;//棋子原来位置的ID
            int twoID = chessSteps[f].chessTwoID;//棋子移动到位置的ID
            GameObject gridOne, gridTwo, chessOne, chessTwo;
            gridOne = chessSteps[f].gridOne;
            gridTwo = chessSteps[f].gridTwo;
            chessOne = chessSteps[f].chessOne;
            chessTwo = chessSteps[f].chessTwo;
            //Debug.Log(chessSteps [f].from.x + "," + chessSteps [f].from.y + "--" + chessSteps [f].to.x + "," + chessSteps [f].to.y);
            //吃子
            if (chessTwo!=null)
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessTwo.transform.SetParent(gridTwo.transform);
                chessOne.transform.localPosition = Vector3.zero;
                chessTwo.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = twoID;
            }
            //移动 
            else
            {
                chessOne.transform.SetParent(gridOne.transform);
                chessOne.transform.localPosition = Vector3.zero;
                gameManager.chessBoard[chessSteps[f].from.x, chessSteps[f].from.y] = oneID;
                gameManager.chessBoard[chessSteps[f].to.x, chessSteps[f].to.y] = 0;
            }
            //该黑方走了，但是红方悔棋
            if (gameManager.chessMove==false)
            {
                UIManager.Instance.ShowTip("红方走");
                gameManager.chessMove = true;
            }
            //该红方走了，但是黑方悔棋
            else
            {
                UIManager.Instance.ShowTip("黑方走");
                gameManager.chessMove = false;
            }
            resetCount -= 1;
            chessSteps[f] = new Chess();
        }
    }
    /// <summary>
    /// 添加悔棋步骤（用来之后悔棋）
    /// </summary>
    /// <param name="resetStepNum">具体的悔棋步数索引</param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    /// <param name="ID1">对应悔棋那一步的第一个棋子ID</param>
    /// <param name="ID2">对应悔棋那一步的第二个ID</param>
    public void AddChess(int resetStepNum,int fromX,int fromY,int toX,int toY,int ID1,int ID2)
    {
        //当前需要记录的这步棋中的数据存入我们的chess结构体里，然后存进结构体数组
        GameObject item1 = gameManager.boardGrid[fromX, fromY];
        GameObject item2 = gameManager.boardGrid[toX, toY];
        chessSteps[resetStepNum].from.x = fromX;
        chessSteps[resetStepNum].from.y = fromY;
        chessSteps[resetStepNum].to.x = toX;
        chessSteps[resetStepNum].to.y = toY;
        chessSteps[resetStepNum].gridOne = item1;
        chessSteps[resetStepNum].gridTwo = item2;
        gameManager.HideCanEatUI();
        gameManager.HideClickUI();
        GameObject firstChess = item1.transform.GetChild(0).gameObject;
        chessSteps[resetStepNum].chessOne = firstChess;
        chessSteps[resetStepNum].chessOneID = ID1;
        chessSteps[resetStepNum].chessTwoID = ID2;
        //如果是吃子
        if (item2.transform.childCount!=0)
        {
            GameObject secondChess = item2.transform.GetChild(0).gameObject;
            chessSteps[resetStepNum].chessTwo = secondChess;
        }
        resetCount++;
        //Debug.Log("第" + resetCount + "步添加");
        //Debug.Log("Item1:" + item1.name);
        //Debug.Log("Item2:" + item2.name);
        //Debug.Log("firstChess:" + firstChess.name);
        //if (chessSteps[resetStepNum].chessTwo != null)
        //{
        //    Debug.Log("secondChess:" + chessSteps[resetStepNum].chessTwo.name);
        //}
    }
}
