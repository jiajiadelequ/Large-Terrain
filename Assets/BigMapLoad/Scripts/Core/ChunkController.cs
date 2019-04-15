using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkController : MonoBehaviour
{

    /// <summary>
    /// 共多少行
    /// </summary>
    public int m_row;

    /// <summary>
    /// 共多少列
    /// </summary>
    public int m_col;

    /// <summary>
    /// 所有的块
    /// </summary>
    Dictionary<ChunkVector2, Chunk> m_chunkMap= new Dictionary<ChunkVector2, Chunk>();

    /// <summary>
    /// 当前玩家
    /// </summary>
    [SerializeField]
    Transform m_player;

    /// <summary>
    /// 当前玩家所在块位置
    /// </summary>
    ChunkVector2 m_currentPos;


   

    /// <summary>
    /// 当前的块列表
    /// </summary>
    [SerializeField]
    List<ChunkVector2> m_currentChunkList = new List<ChunkVector2>();

    /// <summary>
    /// 单个块的边长
    /// </summary>
    [SerializeField]
    float m_chunkLength;


    void Start()
    {
        for (int i = 0; i < m_row; i++)
        {
            for (int j = 0; j < m_col; j++)
            {
                ChunkVector2 vector = new ChunkVector2(i,j);
                m_chunkMap[vector] = new Chunk(vector);
            }
        }


        InitMap();
    }

    protected virtual void InitMap()
    {
        //先确定玩家位置，得到玩家所在块的位置  
        ChunkVector2 currentPos = GetCurrentChunkVector(m_player.position);
        m_currentPos = currentPos;
        //利用块的位置获取实际块列表
        List<ChunkVector2> actChunkList = GetActualChunkList(currentPos);
        //再加载实际列表中的所有块
        UpdateCurrentChunkList(actChunkList, currentPos);

    }

    void Update()
    {
        var realtimePos = GetCurrentChunkVector(m_player.position);
        if (IsChange(realtimePos))//当前块位置发生改变 则更新当前块列表
        {
            var list = GetActualChunkList(realtimePos);
            UpdateCurrentChunkList(list, realtimePos);
        }
    }

 



    /// <summary>
    /// 玩家所在块是否发生改变
    /// </summary>
    /// <param name="realtimePos"></param>
    /// <returns></returns>
    bool IsChange(ChunkVector2 realtimePos)
    {
        if (m_currentPos != realtimePos)
        {
            m_currentPos = realtimePos;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取实际块列表
    /// </summary>
    /// <param name="currentVector">当前中心块位置</param>
    /// <returns></returns>
    List<ChunkVector2> GetActualChunkList(ChunkVector2 currentVector)
    {
        ChunkVector2[] m_expectChunkPosList = new ChunkVector2[25];
        for (int i = 0; i < 25; i++)
        {
            m_expectChunkPosList[i]=new ChunkVector2();
        }

        int currentRow = currentVector.rowNum;
        int currentCol = currentVector.colNum;

        int index = 0;
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                var temp = m_expectChunkPosList[index];
                temp.rowNum = currentRow + i;
                temp.colNum = currentCol + j;
                m_expectChunkPosList[index] = temp;
                index++;
            }
        }
        List<ChunkVector2> actulChunkPosList = new List<ChunkVector2>();
        for (int i = 0; i < m_expectChunkPosList.Length; i++)
        {
            if (m_chunkMap.ContainsKey(m_expectChunkPosList[i]))
            {
                actulChunkPosList.Add(m_expectChunkPosList[i]);
            }
        }
        return actulChunkPosList;
    }

    /// <summary>
    /// 对比当前块列表与实际块列表，并更新当前块列表
    /// </summary>
    /// <param name="actulChunkList">实际块列表</param>
    /// <param name="currentPos">当前中心块位置</param>
    private void UpdateCurrentChunkList(List<ChunkVector2> actulChunkList, ChunkVector2 currentPos)
    {
        for (int i = 0; i < m_currentChunkList.Count; i++)
        {
            ChunkVector2 pos = m_currentChunkList[i];
            Chunk chunk = m_chunkMap[pos];
            if (!actulChunkList.Contains(pos))//真实列表里若不存在当前列表的指定元素 则卸载删除
            {
                chunk.Unload();//卸载不存在于真实块列表的块

                m_currentChunkList.RemoveAt(i);//移除当前块列表中不存在与真实列表的块

                i--;//在遍历列表时删除列表元素 记得索引-1 否则无法正确遍历
            }
            else
            {
                actulChunkList.Remove(pos);//实际块列表移除和当前块列表中相同的元素 注：移除完毕后，实际块列表中的元素
                //先获取chunk的实际状态
                ChunkState actualState = GetChunkStateByRelativePosition(pos, currentPos);

                chunk.Update(actualState);
                
            }

        }

        for (int i = 0; i < actulChunkList.Count; i++)
        {
            ChunkVector2 pos = actulChunkList[i];
            Chunk chunk = m_chunkMap[pos];
            //先获取chunk的实际状态
            ChunkState actualState = GetChunkStateByRelativePosition(pos, currentPos);
            //使用实际状态去更新当前状态
            chunk.Update(actualState);

            m_currentChunkList.Add(pos);//这里添加完以后，当前块列表将与实际块列表保持一致

        }

        Resources.UnloadUnusedAssets();

    }

    /// <summary>
    /// 获取块坐标
    /// </summary>
    /// <param name="position">块的具体vector3位置</param>
    /// <returns></returns>
    ChunkVector2 GetCurrentChunkVector(Vector3 position)
    {
        int col = (int)(position.x / m_chunkLength);
        int row = (int)(position.z / m_chunkLength);

        return new ChunkVector2(row, col);
    }

    /// <summary>
    /// 获取指定块的相对状态
    /// </summary>
    /// <param name="chunk">指定块</param>
    /// <param name="relativeVector">参照块坐标</param>
    /// <returns>相对块状态</returns>
    ChunkState GetChunkStateByRelativePosition(ChunkVector2 chunk, ChunkVector2 relativePosition)
    {
        int rowAmount = Mathf.Abs(chunk.rowNum - relativePosition.rowNum);
        int colAmount = Mathf.Abs(chunk.colNum - relativePosition.colNum);

        if (rowAmount > 2 || colAmount > 2)
        {
            return ChunkState.UnLoad;
        }
        if (rowAmount == 2 || colAmount == 2)
        {
            return ChunkState.Cache;
        }
        if (rowAmount <= 1 || colAmount <= 1)
        {
            return ChunkState.Display;
        }

        return ChunkState.UnLoad;
    }


}