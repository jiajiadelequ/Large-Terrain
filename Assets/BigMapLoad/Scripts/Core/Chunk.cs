using UnityEngine;
using System.Collections;

public class Chunk
{
    /// <summary>
    /// 在块列表中所处的位置
    /// </summary>
    ChunkVector2 m_position;

    /// <summary>
    /// 块的游戏物体
    /// </summary>
    GameObject m_body;

    /// <summary>
    /// 块的资源路径
    /// </summary>
    string m_resPath;

    /// <summary>
    /// 块当前的状态
    /// </summary>
    ChunkState m_currentState = ChunkState.UnLoad;


    /// <summary>
    /// 创建一个块对象
    /// </summary>
    /// <param name="rowNum">在块列表中的第几行</param>
    /// <param name="colNum">在块列表中的第几列</param>
    public Chunk(int rowNum, int colNum)
    {
        m_position = new ChunkVector2(rowNum, colNum);
        m_resPath = string.Format("TerrainPrefab/Terrain_Slice_{0}_{1}", (rowNum + 1), (colNum + 1));
    }
    public Chunk(ChunkVector2 position)
        : this(position.rowNum, position.colNum)
    {
    }

    /// <summary>
    /// 展示出来
    /// </summary>
    /// <returns></returns>
    IEnumerator Display()
    {
        if (m_body == null)
        {
            var request = Resources.LoadAsync<GameObject>(m_resPath);

            yield return request;
            m_body= request.asset as GameObject;
            m_body = GameObject.Instantiate(m_body);
        }
        m_body.SetActive(true);
    }

    /// <summary>
    /// 卸载
    /// </summary>
    public void Unload()
    {
        GameObject.Destroy(m_body);
        m_body = null;
    }

    /// <summary>
    /// 缓存
    /// </summary>
    public void Cache()
    {
        if (m_body == null)
        {
            m_body = Resources.Load<GameObject>(m_resPath);
            m_body = GameObject.Instantiate(m_body);
        }
        m_body.SetActive(false);
    }

    /// <summary>
    /// 更新自身状态
    /// </summary>
    /// <param name="state"></param>
    public void Update(ChunkState state)
    {
        if (m_currentState == state)
        {
            Debug.LogErrorFormat(" {0} is already {1} ", m_position, m_currentState);
            return;
        }
        switch (state)
        {
            case ChunkState.Display:
                MonoThread.Instance.Excute(Display());
                break;
            case ChunkState.Cache:
                Cache();
                break;
            case ChunkState.UnLoad:
                Unload();
                break;

        }
    }

}