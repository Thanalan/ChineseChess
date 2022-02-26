using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 获取当前最好一步下棋着法的搜索引擎
/// </summary>
public class SearchEngine
{
    //搜索深度
    public int searchDepth;
    private GameManager gameManager;
    //当期搜索出的最佳移动着法
    private ChessReseting.Chess bestStep;
    //虚拟棋盘
    private int[,] unrealBoard = new int[10, 9];
    //记录一个棋子相关位置的数组
    private ChessReseting.ChessSteps[] relatePos = new ChessReseting.ChessSteps[30];
    //记录棋子相关位置的个数
    private int posCount;

    public SearchEngine()
    {
        gameManager = GameManager.Instance;
    }

    //棋子的子力价值表
    private int[] baseValue = new int[15]
    {
    //ID  1.将  2.车  3.马  4.炮  5.士  6.象  7.兵
        0,10000,900,  400,  450,  200,  200,  100,
          10000,900,  400,  450,  200,  200,  100
    };
    //棋子的灵活值表
    private int[] flexValue = new int[15]
    {
    //ID  1.将 2.车 3.马 4.炮 5.士 6.象 7.兵
        0,0,   6,   12,  6,   1,   1,   1,
          0,   6,   12,  6,   1,   1,   1
    };
    //每个位置威胁信息(威胁值)（这个威胁指的是被威胁，而不是自身的威胁值,处于被其他棋子攻击的攻击范围）
    private int[,] attackPos;
    //存放每个位置被保护的信息（保护值）
    private int[,] guardPos;
    //存放每个位置的灵活信息（灵活值）
    private int[,] flexPos;
    //存放每个位置的基础值（根据上述值通过公式计算得出基础值总分，后续所有棋子基础值总分用来估算整个
    //局势得分，即评估函数返回值）
    private int[,] chessValue;
    //红兵的位置附加值数组
    private int[,] r_bingValue = new int[10, 9]
    {
        {0,0,0,0,0,0,0,0,0} ,
        {90,90,110,120,120,120,110,90,90} ,
        {90,90,110,120,120,120,110,90,90} ,
        {70,90,110,110,110,110,110,90,70} ,
        {70,70,70,70,70,70,70,70,70} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
    };
    //黑兵的位置附加值数组
    private int[,] b_bingValue = new int[10, 9]
    {
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {0,0,0,0,0,0,0,0,0} ,
        {70,70,70,70,70,70,70,70,70} ,
        {70,90,110,110,110,110,110,90,70} ,
        {90,90,110,120,120,120,110,90,90} ,
        {90,90,110,120,120,120,110,90,90} ,
        {0,0,0,0,0,0,0,0,0} ,
    };

    /// <summary>
    /// 具体搜索当前最佳着法的方法
    /// </summary>
    /// <returns></returns>
    public ChessReseting.Chess SearchAGoodMove(int[,] positon)
    {
        //设置搜索层级
        searchDepth = gameManager.currentLevel;
        //将当前棋盘的情况记录进虚拟棋盘
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unrealBoard[i, j] = positon[i, j];
            }
        }
        //调用具体搜索算法搜索最佳着法
        //负极大值算法
        //NegaMax(searchDepth);
        //alpha-beta剪枝算法
        //AlphaBeta(searchDepth,-20000,20000);
        //归并排序和历史启发优化的剪枝算法
        //MergeSortAlphaBeta(searchDepth, -20000, 20000);
        //渴望算法
        //AspirationSearch();
        //极小窗口搜索
        PrincipalVariation(searchDepth, -20000, 20000);

        GameObject item1 = gameManager.boardGrid[bestStep.from.x, bestStep.from.y];
        GameObject item2 = gameManager.boardGrid[bestStep.to.x, bestStep.to.y];
        bestStep.gridOne = item1;
        bestStep.gridTwo = item2;
        GameObject firstChess = item1.transform.GetChild(0).gameObject;
        bestStep.chessOne = firstChess;
        //移动
        bestStep.chessOneID = positon[bestStep.from.x, bestStep.from.y];
        bestStep.chessTwoID = positon[bestStep.to.x, bestStep.to.y];
        //吃子
        if (item2.transform.childCount != 0)
        {
            GameObject secondChess = item2.transform.GetChild(0).gameObject;
            bestStep.chessTwo = secondChess;
        }

        return bestStep;
    }
    #region 搜索算法
    /// <summary>
    /// 负极大值算法
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int NegaMax(int depth)
    {
        //负无穷
        int best = -20000;
        //当前调用的得分
        int score;
        //当前局面下一步总共可以走的着法
        int count;
        //当前棋局下将帅是否阵亡
        int willKillKing;
        //当前棋局下移动的棋子对应ID
        int chessID;
        //检查棋局是否结束
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            //棋局结束，将帅有阵亡，当前着法为某一方所有着法中最佳或最差着法
            return willKillKing;
        }
        //最底层叶的棋局得分
        if (depth <= 0)
        {
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        //列举当前局面下一步所有可能走的着法
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        for (int i = 0; i < count; i++)
        {
            //根据走法产生新局面
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            //递归调用负极大值搜索函数搜索下一层节点
            score = -NegaMax(depth - 1);
            //恢复当前局面
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score > best)
            {
                best = score;
                if (depth == searchDepth)
                {
                    //搜索到达根部时保存最佳着法
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                }
            }
        }
        return best;
    }
    /// <summary>
    /// alpha-beta剪枝算法
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    private int AlphaBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }
        if (depth <= 0)
        {
            //当前搜索深度是奇数层吗？是的话返回true,偶数层返回flase
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -AlphaBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score >= beta)//剪枝
            {
                return beta;
            }
            if (score > alpha)
            {
                alpha = score;//保存最大值
                if (depth == searchDepth)
                {
                    bestStep = gameManager.movingOfChess.moveList[depth, i];//根节点即最佳得分
                }
            }
        }
        return alpha;//返回最大值
    }
    /// <summary>
    /// 渴望算法
    /// </summary>
    private void AspirationSearch()
    {
        //深度N-1层的最佳得分
        int X;
        //搜索结果
        int current;
        searchDepth = gameManager.currentLevel - 1;
        X = FalphaBeta(searchDepth, -20000, 20000);
        //对目标小床进行搜索
        searchDepth = gameManager.currentLevel;
        current = FalphaBeta(searchDepth, X - 50, X + 50);
        //估值偏大
        if (current < X - 50)
        {
            FalphaBeta(searchDepth, -20000, X - 50);
        }
        //估值偏小
        if (current > X + 50)
        {
            FalphaBeta(searchDepth, X + 50, 20000);
        }
    }
    /// <summary>
    /// 渴望搜索
    /// </summary>
    private int FalphaBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        int current = -20000;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }
        if (depth <= 0)
        {
            //当前搜索深度是奇数层吗？是的话返回true,偶数层返回flase
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -FalphaBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score > current)
            {
                current = score;
                if (score >= alpha)
                {
                    alpha = score;//更新边界值
                    if (depth == searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                if (alpha >= beta)
                {
                    break;
                }
            }
        }
        return current;//返回最大值
    }

    private int PrincipalVariation(int depth, int alpha, int beta)
    {
        int score, count, willkillKing, chessID;
        int best;
        willkillKing = IsGameOver(unrealBoard,depth);
        if (willkillKing!=0)
        {
            return willkillKing;
        }
        if (depth<=0)
        {
            return Eveluate(unrealBoard,((searchDepth-depth)%2)!=0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard,depth, ((searchDepth - depth) % 2) != 0);
        //对于第一个节点,我们按照原来的范围  进行搜索，我们会得到一个最优解best
        chessID = MakeMove(gameManager.movingOfChess.moveList[depth,0]);
        best = -PrincipalVariation(depth-1,-beta,-alpha);
        UnMakeMove(gameManager.movingOfChess.moveList[depth,0],chessID);
        if (depth==searchDepth)
        {
            bestStep = gameManager.movingOfChess.moveList[depth, 0];
        }
        for (int i = 1; i < count; i++)
        {
            //首先保证不能被剪枝，如果被裁剪掉，那么就不再考虑这个分支了
            //只会更新下界，上界我们不更新
            if (best<beta)
            {
                //如果当前得分大于alpha,则更新边界值，让区间更小
                if (best>alpha)
                {
                    alpha = best;
                }
                chessID = MakeMove(gameManager.movingOfChess.moveList[depth,i]);
                //进行极小窗口搜索评估
                score = -PrincipalVariation(depth-1,-alpha-1,-alpha);
                //如果搜索出来的得分比之前的下界更大，那么就说明存在更大值，
                //并且这个值是在我们范围内的，且这个值并不一定是最大，那么重新评估，
                //更新best
                if (score>alpha&&score<beta)
                {
                    best = -PrincipalVariation(depth-1,-beta,-score);
                    if (depth==searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                //这个值本身就是一个更好的行动，更新最佳得分
                else if (score>best)
                {
                    best = score;
                    if (depth == searchDepth)
                    {
                        bestStep = gameManager.movingOfChess.moveList[depth, i];
                    }
                }
                UnMakeMove(gameManager.movingOfChess.moveList[depth,i],chessID);
            }
        }
        return best;
    }


    #region alpha-beta剪枝归并排序的优化算法
    /// <summary>
    /// alpha-beta剪枝归并排序的优化算法
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    private int MergeSortAlphaBeta(int depth, int alpha, int beta)
    {
        int score, count, willKillKing, chessID;
        willKillKing = IsGameOver(unrealBoard, depth);
        if (willKillKing != 0)
        {
            return willKillKing;
        }
        if (depth <= 0)
        {
            //当前搜索深度是奇数层吗？是的话返回true,偶数层返回flase
            return Eveluate(unrealBoard, (searchDepth - depth) % 2 != 0);
        }
        count = gameManager.movingOfChess.CreatePossibleMove(unrealBoard, depth, (searchDepth - depth) % 2 != 0);
        MergeSort(gameManager.movingOfChess.moveList, count, depth);
        for (int i = 0; i < count; i++)
        {
            chessID = MakeMove(gameManager.movingOfChess.moveList[depth, i]);
            score = -AlphaBeta(depth - 1, -beta, -alpha);
            UnMakeMove(gameManager.movingOfChess.moveList[depth, i], chessID);
            if (score >= beta)
            {
                return beta;
            }
            if (score > alpha)
            {
                alpha = score;
                if (depth == searchDepth)
                {
                    bestStep = gameManager.movingOfChess.moveList[depth, i];
                    AddHistoryScore(bestStep, depth);
                }
            }
        }
        return alpha;
    }
    //历史启发
    //历史记录表 键：着法  值：历史得分   每给予一个着法，那么我们可以获取到对应的历史得分
    private Dictionary<ChessReseting.Chess, int> historyDic = new Dictionary<ChessReseting.Chess, int>();
    /// <summary>
    /// 为着法添加历史记录得分
    /// </summary>
    /// <param name="move">具体的着法</param>
    /// <param name="depth"></param>
    public void AddHistoryScore(ChessReseting.Chess move, int depth)
    {
        //当前历史记录里有此着法
        if (historyDic.TryGetValue(move, out int score))
        {
            historyDic[move] += 2 << depth;
        }
        //没有此着法
        else
        {
            historyDic.Add(move, 2 << depth);
        }
    }
    /// <summary>
    /// 取得给定着法的历史得分
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private int GetHistoryScore(ChessReseting.Chess move)
    {
        historyDic.TryGetValue(move, out int score);
        return score;
    }
    /// <summary>
    /// 归并排序
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="count"></param>
    /// <param name="depth"></param>
    private void MergeSort(ChessReseting.Chess[,] move, int count, int depth)
    {
        //在排序前，先建好一个长度等于原数组长度的临时数组，作为盛放新序列的容器
        ChessReseting.Chess[,] temp = new ChessReseting.Chess[8, 80];
        Sort(move, 0, count, temp, depth);
    }
    /// <summary>
    /// 归并排序的分治方法
    /// </summary>
    /// <param name="move"></param>
    /// <param name="startIndex">分段数组的开始索引</param>
    /// <param name="endIndex">分段数组的结束索引</param>
    /// <param name="temp">临时数组</param>
    /// <param name="depth">为取数组中着法元素的必要条件</param>
    private void Sort(ChessReseting.Chess[,] move, int startIndex, int endIndex, ChessReseting.Chess[,] temp, int depth)
    {
        if (startIndex < endIndex)
        {
            int mid = (startIndex + endIndex) / 2;
            //左边归并排序，使得左子序列有序
            Sort(move, startIndex, mid, temp, depth);
            //右边归并排序，使得右子序列有序
            Sort(move, mid + 1, endIndex, temp, depth);
            //将两个有序子数组合并操作
            Merge(move, startIndex, mid, endIndex, temp, depth);
        }
    }
    /// <summary>
    /// 归并且排序的方法
    /// </summary>
    /// <param name="move"></param>
    /// <param name="startIndex"></param>
    /// <param name="mid"></param>
    /// <param name="endIndex"></param>
    /// <param name="temp"></param>
    /// <param name="depth"></param>
    private void Merge(ChessReseting.Chess[,] move, int startIndex, int mid, int endIndex, ChessReseting.Chess[,] temp, int depth)
    {
        //左序列指针
        int i = startIndex;
        //右序列指针
        int j = mid + 1;
        //临时数组指针
        int t = 0;
        while (i <= mid && j <= endIndex)
        {
            //取两个着法中较大的历史得分对应的着法放入临时数组
            if (GetHistoryScore(move[depth, i]) >= GetHistoryScore(move[depth, j]))
            {
                temp[depth, t] = move[depth, i];
                i++;
            }
            else
            {
                temp[depth, t] = move[depth, j];
                j++;
            }
            t++;
        }
        //将左边剩余元素填充进temp中
        while (i <= mid)
        {
            temp[depth, t] = move[depth, i];
            i++;
            t++;
        }
        //将右序列剩余元素填充进temp中
        while (j <= endIndex)
        {
            temp[depth, t++] = move[depth, j++];
        }
        t = 0;
        //将temp中的元素全部拷贝到原数组中
        while (startIndex <= endIndex)
        {
            move[depth, startIndex++] = temp[depth, t++];
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// 评估函数
    /// </summary>
    /// <param name="postion"></param>
    /// <param name="side">当前是以哪一边视角去评估，false是偶数且是AI层(黑方)，true是奇数且是玩家层（红方）</param>
    /// <returns></returns>
    private int Eveluate(int[,] position, bool side)
    {
        int currentPosChessID;
        int targetPosChessID;
        chessValue = new int[10, 9];
        attackPos = new int[10, 9];
        guardPos = new int[10, 9];
        flexPos = new int[10, 9];
        #region 第一次扫描,找出所有棋子的相关位置，并赋值得分（对棋子相关位置的处理）
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                //扫描到该位置是棋子时
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //找出该棋子的所有相关位置
                    GetRelatePos(position, i, j);
                    //对每一个位置进行后续处理
                    for (int k = 0; k < posCount; k++)
                    {
                        //获取每一个相关位置的棋子ID
                        targetPosChessID = position[relatePos[k].x, relatePos[k].y];
                        //相关位置是否为空格子
                        //是空格子
                        if (targetPosChessID == 0)
                        {
                            //空格子，则代表我们指定位置的棋子可以走到这个相关位置，所以灵活值++
                            flexPos[i, j]++;
                        }
                        //是棋子
                        else
                        {
                            //如果是己方棋子，该相关位置被保护，则保护值增加
                            if (gameManager.rules.IsSameSide(currentPosChessID, targetPosChessID))
                            {
                                guardPos[relatePos[k].x, relatePos[k].y]++;
                            }
                            //如果是敌方棋子，该相关位置被威胁，威胁值增加，且该指定位置灵活性增加
                            else
                            {
                                attackPos[relatePos[k].x, relatePos[k].y]++;
                                flexPos[i, j]++;
                                switch (targetPosChessID)
                                {
                                    //如果是红将
                                    case 8:
                                        if (!side)
                                        {
                                            //黑方轮次则返回极大值 
                                            return 18888;
                                        }
                                        break;
                                    //如果是黑将
                                    case 1:
                                        if (side)
                                        {
                                            //红方轮次则返回极大值 
                                            return 18888;
                                        }
                                        break;
                                    //不是将帅的其他棋子
                                    default:
                                        //棋子相关位置威胁值增加
                                        attackPos[relatePos[k].x, relatePos[k].y] += ((baseValue[targetPosChessID] - baseValue[currentPosChessID]) / 10 + 30) / 10;
                                        break;
                                }
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region 第二次扫描，对棋盘上每个棋子的自身基础值做处理
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //如果是棋子，则该位置价值增加。
                    chessValue[i, j]++;
                    //把每个棋子的灵活性价值加进基础值
                    chessValue[i, j] += flexValue[currentPosChessID] * flexPos[i, j];
                    //如果是兵,则基础值加上兵的位置附加值
                    chessValue[i, j] += GetBingValue(i, j, position);
                }
            }
        }
        #endregion

        #region 第三次扫描，计算当前棋子所在位置的基础值总分(根据增量以及威胁值和保护值来计算)
        int delta;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (position[i, j] != 0)
                {
                    //取得当前位置的棋子ID
                    currentPosChessID = position[i, j];
                    //棋子子力价值的1/16作为威胁/保护增量   
                    delta = baseValue[currentPosChessID] / 16;
                    //基础值加上每个棋子的子力价值
                    chessValue[i, j] += baseValue[currentPosChessID];
                    //如果是红棋
                    if (gameManager.rules.IsRed(currentPosChessID))
                    {
                        //当前红棋如果被威胁
                        if (attackPos[i, j] != 0)
                        {
                            //轮到红棋走
                            if (side)
                            {
                                //如果是红帅
                                if (currentPosChessID == 8)
                                {
                                    //基础值降低20
                                    chessValue[i, j] -= 20;
                                }
                                //不是红帅
                                else
                                {
                                    //基础值减去2倍delta
                                    chessValue[i, j] -= delta * 2;
                                    //是否被己方棋子保护
                                    if (guardPos[i, j] != 0)
                                    {
                                        //被保护再加上delta
                                        chessValue[i, j] += delta;
                                    }
                                }
                            }
                            //轮到黑棋走
                            else
                            {
                                //是否是红帅
                                if (currentPosChessID == 8)
                                {
                                    //黑方轮次则返回极大值 
                                    return 18888;
                                }
                                else
                                {
                                    //减去10倍的delta，表示威胁程度高
                                    chessValue[i, j] -= delta * 10;
                                    //如果被保护
                                    if (guardPos[i, j] != 0)
                                    {
                                        //被保护再加上9倍delta
                                        chessValue[i, j] += delta * 9;
                                    }
                                }
                            }
                        }
                        //没受到威胁
                        else
                        {
                            //该位置收到保护，保护值增加
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                    //如果是黑棋
                    else
                    {
                        //当前黑棋如果被威胁
                        if (attackPos[i, j] != 0)
                        {
                            //轮到黑棋走
                            if (!side)
                            {
                                //如果是黑将
                                if (currentPosChessID == 1)
                                {
                                    //棋子价值降低20
                                    chessValue[i, j] -= 20;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 2;
                                    //如果受保护
                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta;
                                    }
                                }
                            }
                            //轮到红棋走
                            else
                            {
                                //如果是黑将
                                if (currentPosChessID == 1)
                                {
                                    return 18888;
                                }
                                else
                                {
                                    chessValue[i, j] -= delta * 10;
                                    //如果受保护
                                    if (guardPos[i, j] != 0)
                                    {
                                        chessValue[i, j] += delta * 9;
                                    }
                                }
                            }
                        }
                        //没受到威胁
                        else
                        {
                            //该位置收到保护，保护值增加
                            if (guardPos[i, j] != 0)
                            {
                                chessValue[i, j] += 5;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        //string str = "保护值：\n";
        //for (int c = 0; c < guardPos.GetLength(0); c++)
        //{
        //    for (int d = 0; d < guardPos.GetLength(1); d++)
        //        str += " " + guardPos[c, d];
        //    str += "\n";
        //}
        //Debug.Log(str);
        //str = "灵活值:\n";
        //for (int c = 0; c < flexPos.GetLength(0); c++)
        //{
        //    for (int d = 0; d < flexPos.GetLength(1); d++)
        //        str += " " + flexPos[c, d];
        //    str += "\n";
        //}
        //Debug.Log(str);
        //str = "威胁值：\n";
        //for (int c = 0; c < attackPos.GetLength(0); c++)
        //{
        //    for (int d = 0; d < attackPos.GetLength(1); d++)
        //        str += " " + attackPos[c, d];
        //    str += "\n";
        //}
        //Debug.Log(str);
        //str = "基础值：\n";
        //for (int c = 0; c < chessValue.GetLength(0); c++)
        //{
        //    for (int d = 0; d < chessValue.GetLength(1); d++)
        //        str += "," + chessValue[c, d];
        //    str += "\n";
        //}
        //Debug.Log(str);
        #region 第四次扫描，计算红方与黑方的总得分，返回评估值
        int redValue = 0;
        int blackValue = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                currentPosChessID = position[i, j];
                //统计所有棋子基础值加入对应一方的总得分里
                if (currentPosChessID != 0)
                {
                    if (gameManager.rules.IsRed(currentPosChessID))
                    {
                        redValue += chessValue[i, j];
                    }
                    else
                    {
                        blackValue += chessValue[i, j];
                    }
                }
            }
        }
        //红方视角得分
        if (side)
        {
            return redValue - blackValue;
        }
        //黑方视角得分
        else
        {
            return blackValue - redValue;
        }
        #endregion
    }



    /// <summary>
    /// 根据传入的走法改变棋盘
    /// </summary>
    /// <param name="move">具体的着法</param>
    /// <returns></returns>
    private int MakeMove(ChessReseting.Chess move)
    {
        int chessID = 0;
        //取到目标位置的棋子
        chessID = unrealBoard[move.to.x, move.to.y];
        //把棋子移动到目标位置
        unrealBoard[move.to.x, move.to.y] = unrealBoard[move.from.x, move.from.y];
        //将原位置清空
        unrealBoard[move.from.x, move.from.y] = 0;

        return chessID;
    }
    /// <summary>
    /// 还原之前走的着法
    /// </summary>
    /// <param name="move">之前的着法</param>
    /// <param name="chessID">走的棋子ID</param>
    private void UnMakeMove(ChessReseting.Chess move, int chessID)
    {
        //将原来位置的棋子ID还原
        unrealBoard[move.from.x, move.from.y] = unrealBoard[move.to.x, move.to.y];
        //恢复目标位置的棋子ID
        unrealBoard[move.to.x, move.to.y] = chessID;
    }
    /// <summary>
    /// 检查当前着法执行后棋盘中是否存在将帅棋子，游戏是否结束（模拟即将行走的着法之后的棋局情况）
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private int IsGameOver(int[,] position, int depth)
    {
        bool redAlive = false;
        bool blackAlive = false;
        //检查搜索九宫格中是否有将帅
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position[i, j] == 1)
                {
                    blackAlive = true;
                }
            }
        }
        for (int i = 7; i < 10; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position[i, j] == 8)
                {
                    redAlive = true;
                }
            }
        }
        //取当前是奇数层还是偶数层
        int num = (searchDepth - depth + 1) % 2;
        //帅不存在的情况
        if (!redAlive)
        {
            if (num != 0)
            {
                //奇数层返回极大值(AI)
                return 19990;
            }
            else
            {
                //偶数层返回极小值(玩家)
                return -19990;
            }
        }
        //将不存在的情况
        if (!blackAlive)
        {
            if (num != 0)
            {
                return -19990;
            }
            else
            {
                return 19990;
            }
        }
        return 0;
    }
    /// <summary>
    /// 获取与指定位置棋子相关的所有位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <returns></returns>
    private int GetRelatePos(int[,] position, int fromX, int fromY)
    {
        posCount = 0;
        int chessID;
        bool flag = false;
        int x, y;
        chessID = position[fromX, fromY];
        switch (chessID)
        {
            case 1://黑将
                for (x = 0; x < 3; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, fromX, fromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 8://红帅
                for (x = 7; x < 10; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, fromX, fromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 2://黑車
            case 9:
                //右
                x = fromX;
                y = fromY + 1;
                while (y < 9)
                {
                    if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
                    {
                        AddPos(x, y);
                    }
                    else//不为空格子
                    {
                        AddPos(x, y);
                        break;
                    }
                    y++;
                }
                //左
                x = fromX;
                y = fromY - 1;
                while (y >= 0)
                {
                    if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
                    {
                        AddPos(x, y);
                    }
                    else//不为空格子
                    {
                        AddPos(x, y);
                        break;
                    }
                    y--;
                }
                //下
                x = fromX + 1;
                y = fromY;
                while (x < 10)
                {
                    if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
                    {
                        AddPos(x, y);
                    }
                    else//不为空格子
                    {
                        AddPos(x, y);
                        break;
                    }
                    x++;
                }
                //上
                x = fromX - 1;
                y = fromY;
                while (x >= 0)
                {
                    if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
                    {
                        AddPos(x, y);
                    }
                    else//不为空格子
                    {
                        AddPos(x, y);
                        break;
                    }
                    x--;
                }
                break;
            case 3://黑马
            case 10:
                //竖日
                //右下
                x = fromX + 2;
                y = fromY + 1;
                if ((x < 10 && y < 9) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //右上
                x = fromX - 2;
                y = fromY + 1;
                if ((x >= 0 && y < 9) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下
                x = fromX + 2;
                y = fromY - 1;
                if ((x < 10 && y >= 0) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上
                x = fromX - 2;
                y = fromY - 1;
                if ((x >= 0 && y >= 0) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //横日
                //右下
                x = fromX + 1;
                y = fromY + 2;
                if ((x < 10 && y < 9) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //右上
                x = fromX - 1;
                y = fromY + 2;
                if ((x >= 0 && y < 9) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下
                x = fromX + 1;
                y = fromY - 2;
                if ((x < 10 && y >= 0) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上
                x = fromX - 1;
                y = fromY - 2;
                if ((x >= 0 && y >= 0) && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                break;
            case 4://黑炮
            case 11:
                //右
                x = fromX;
                y = fromY + 1;
                flag = false;
                while (y < 9)
                {
                    //是空格子
                    if (position[x, y] == 0)
                    {
                        //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                        //不可空翻
                        if (!flag)
                        {
                            AddPos(x, y);
                        }
                    }
                    //是棋子
                    else
                    {
                        //条件未满足时，开启条件的满足，可翻山
                        if (!flag)
                        {
                            flag = true;
                        }
                        //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                        //如果不是，则此子可吃，即可移动到此位置，则需显示
                        //结束当前遍历
                        else
                        {
                            AddPos(x, y);
                            break;
                        }
                    }
                    y++;
                }
                //左
                y = fromY - 1;
                flag = false;
                while (y >= 0)
                {
                    if (position[x, y] == 0)
                    {
                        if (!flag)
                        {
                            AddPos(x, y);
                        }
                    }
                    else
                    {
                        if (!flag)
                        {
                            flag = true;
                        }
                        else
                        {
                            AddPos(x, y);
                            break;
                        }
                    }
                    y--;
                }
                //下
                x = fromX + 1;
                y = fromY;
                flag = false;
                while (x < 10)
                {
                    //是空格子
                    if (position[x, y] == 0)
                    {
                        //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                        //不可空翻
                        if (!flag)
                        {
                            AddPos(x, y);
                        }
                    }
                    //是棋子
                    else
                    {
                        //条件未满足时，开启条件的满足，可翻山
                        if (!flag)
                        {
                            flag = true;
                        }
                        //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                        //如果不是，则此子可吃，即可移动到此位置，则需显示
                        //结束当前遍历
                        else
                        {
                            AddPos(x, y);
                            break;
                        }
                    }
                    x++;
                }
                //上
                x = fromX - 1;
                flag = false;
                while (x >= 0)
                {
                    //是空格子
                    if (position[x, y] == 0)
                    {
                        //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                        //不可空翻
                        if (!flag)
                        {
                            AddPos(x, y);
                        }
                    }
                    //是棋子
                    else
                    {
                        //条件未满足时，开启条件的满足，可翻山
                        if (!flag)
                        {
                            flag = true;
                        }
                        //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                        //如果不是，则此子可吃，即可移动到此位置，则需显示
                        //结束当前遍历
                        else
                        {
                            AddPos(x, y);
                            break;
                        }
                    }
                    x--;
                }
                break;
            case 5://黑士
                for (x = 0; x < 3; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, fromX, fromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 12://红仕
                for (x = 7; x < 10; x++)
                {
                    for (y = 3; y < 6; y++)
                    {
                        if (CanReach(position, fromX, fromY, x, y))
                        {
                            AddPos(x, y);
                        }
                    }
                }
                break;
            case 6://黑象
            case 13:
                //右下走
                x = fromX + 2;
                y = fromY + 2;
                if (x < 10 && y < 9 && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //右上走
                x = fromX - 2;
                y = fromY + 2;
                if (x >= 0 && y < 9 && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左下走
                x = fromX + 2;
                y = fromY - 2;
                if (x < 10 && y >= 0 && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                //左上走
                x = fromX - 2;
                y = fromY - 2;
                if (x >= 0 && y >= 0 && CanReach(position, fromX, fromY, x, y))
                {
                    AddPos(x, y);
                }
                break;
            case 7://黑卒
                x = fromX + 1;
                y = fromY;
                if (x < 10)
                {
                    AddPos(x, y);
                }
                //过河后
                if (fromX > 4)
                {
                    x = fromX;
                    y = fromY + 1;//右边
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = fromY - 1;//左边
                    if (y >= 0)
                    {
                        AddPos(x, y);
                    }
                }
                break;
            case 14://红兵
                x = fromX - 1;
                y = fromY;
                if (x > 0)
                {
                    AddPos(x, y);
                }
                //过河后
                if (fromX < 5)
                {
                    x = fromX;
                    y = fromY + 1;//右边
                    if (y < 9)
                    {
                        AddPos(x, y);
                    }
                    y = fromY - 1;//左边
                    if (y >= 0)
                    {
                        AddPos(x, y);
                    }
                }
                break;
            default:
                break;
        }
        return posCount;
    }
    /// <summary>
    /// 把当前传递进来的一个相关位置信息记录进相关位置数组里
    /// </summary>
    /// <returns></returns>
    private void AddPos(int x, int y)
    {
        relatePos[posCount].x = x;
        relatePos[posCount].y = y;
        posCount++;
    }
    /// <summary>
    /// 当前这个相关位置针对于我们指定棋子是否可以到达
    /// </summary>
    /// <returns></returns>
    private bool CanReach(int[,] position, int fromX, int fromY, int toX, int toY)
    {
        return gameManager.rules.IsVaild(position[fromX, fromY], position, fromX, fromY, toX, toY);
    }
    /// <summary>
    /// 小兵位置附加值计算方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private int GetBingValue(int x, int y, int[,] position)
    {
        //红兵
        if (position[x, y] == 14)
        {
            return b_bingValue[x, y];
        }
        //黑卒
        else if (position[x, y] == 7)
        {
            return r_bingValue[x, y];
        }
        return 0;
    }


}











