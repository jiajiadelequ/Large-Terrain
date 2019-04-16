using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ChunkManagerExample : MonoBehaviour
{

    //共多少行
    public int m_row;
    //共多少列
    public int m_col;

    /// <summary>
    /// 所有的块
    /// </summary>
    public Dictionary<ChunkVector2, ChunkExample> m_chunkMap ;
    //public Chunk current;
    public Transform player;

     ChunkVector2  m_currentVector;
    /// <summary>
    /// 期望的块列表
    /// </summary>
    [SerializeField]
    List<ChunkVector2> expectChunkVectorList = new List<ChunkVector2>(25);
    /// <summary>
    /// 当前的块列表
    /// </summary>
    [SerializeField]
    List<ChunkExample> currentCacheChunkList = new List<ChunkExample>();

    //单个块的边长
    public float chunkLength;

    public GameObject chunkPrefab;
    void Start()
    {
        m_chunkMap = new Dictionary<ChunkVector2, ChunkExample>();

        for (int i = 0; i < 25; i++)
        {
            expectChunkVectorList.Add(new ChunkVector2());
        }

        InitMap();
    }

    protected virtual void InitMap()
    {
        for (int i = 0; i < m_row; i++)
        {
            for (int j = 0; j < m_col; j++)
            {
                Vector3 vect = new Vector3();
                vect.x = j * chunkLength;
                vect.z = i * chunkLength;
                GameObject go = Instantiate(chunkPrefab, vect, Quaternion.identity) as GameObject;
                go.name = string.Format("chunk[{0}][{1}]", i, j);

                ChunkExample chunk = go.GetComponent<ChunkExample>();
                ChunkVector2 position = new ChunkVector2(i, j);
                chunk.position = position;
                m_chunkMap[position] = chunk;
            }
        }
    }

    void Update()
    {
        var currentVector = GetCurrentChunkVector(player.position);
        if (IsChange(currentVector))//当前块位置发生改变 则更新当前块列表
        {
            var list = GetActualChunkList(currentVector);
            UpdateCurrentChunkList(list, currentVector);
            //m_chunkMap[currentVector].DisplayCurrent();
        }
    }

    bool IsChange(ChunkVector2 current)
    {
        if (m_currentVector != current)
        {
            m_currentVector = current;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取实际块列表
    /// </summary>
    /// <param name="currentVector">当前中心块位置</param>
    /// <returns></returns>
    List<ChunkExample> GetActualChunkList(ChunkVector2 currentVector)
    {

        int currentRow = currentVector.rowNum;
        int currentCol = currentVector.colNum;

        int index = 0;
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                var temp = expectChunkVectorList[index];
                temp.rowNum = currentRow + i;
                temp.colNum = currentCol + j;
                expectChunkVectorList[index] = temp;
                index++;
            }
        }
        List<ChunkExample> actulDisplayChunkList = new List<ChunkExample>();
        for (int i = 0; i < expectChunkVectorList.Count; i++)
        {
            if (m_chunkMap.ContainsKey(expectChunkVectorList[i]))
            {
                var chunk = m_chunkMap[expectChunkVectorList[i]];
                actulDisplayChunkList.Add(chunk);
            }
        }
        return actulDisplayChunkList;
    }

    /// <summary>
    /// 更新当前块列表
    /// </summary>
    /// <param name="actulChunkList">实际块列表</param>
    /// <param name="currentVector">当前中心块位置</param>
    private void UpdateCurrentChunkList(List<ChunkExample> actulChunkList, ChunkVector2 currentVector)
    {
        for (int i = 0; i < currentCacheChunkList.Count; i++)
        {
            ChunkExample chunk = currentCacheChunkList[i];
            if (!actulChunkList.Contains(chunk))//实际列表里若不存在当前列表的指定元素 则卸载删除
            {
                chunk.DisplayUnload();//卸载不存在于实际块列表的块

                currentCacheChunkList.RemoveAt(i);//移除当前块列表中不存在与实际列表的块

                i--;//在遍历列表时删除列表元素 记得索引-1 否则无法正确遍历
            }
            else
            {
                actulChunkList.Remove(chunk);//实际块列表移除和当前块列表中相同的元素 注：移除完毕后，实际块列表中的元素
                //先获取chunk的实际状态
                ChunkState actualState = GetChunkStateByRelativePosition(chunk, currentVector);
                chunk.Excute(actualState);
            }

        }

        for (int i = 0; i < actulChunkList.Count; i++)
        {
            ChunkExample chunk = actulChunkList[i];
            //先获取chunk的实际状态
            ChunkState actualState = GetChunkStateByRelativePosition(chunk, currentVector);
            chunk.Excute(actualState);
            currentCacheChunkList.Add(chunk);//这里添加完以后，当前块列表将与实际块列表保持一致

        }

    }

    /// <summary>
    /// 获取块坐标
    /// </summary>
    /// <param name="position">块的具体vector3位置</param>
    /// <returns></returns>
    ChunkVector2 GetCurrentChunkVector(Vector3 position)
    {
         int col = (int)(position.x / 10);
        int row = (int)(position.z / 10);

        return new ChunkVector2(row, col);
    }

    /// <summary>
    /// 获取指定块的相对状态
    /// </summary>
    /// <param name="chunk">指定块</param>
    /// <param name="relativeVector">参照块坐标</param>
    /// <returns>相对块状态</returns>
    ChunkState GetChunkStateByRelativePosition(ChunkExample chunk,ChunkVector2 relativePosition)
    {
        ChunkVector2 vector = chunk.position;


        int rowAmount =Mathf.Abs( vector.rowNum - relativePosition.rowNum);
        int colAmount=Mathf.Abs(vector.colNum - relativePosition.colNum);

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
