using UnityEngine;
using System.Collections;

public class Chunk
{
    ChunkVector2 m_position;
    GameObject m_body;


    string m_resPath;
    ChunkState m_currentState = ChunkState.UnLoad;



    public Chunk(int rowNum, int colNum)
    {
        m_position = new ChunkVector2(rowNum, colNum);
        m_resPath = string.Format("TerrainPrefab/Terrain_Slice_{0}_{1}", (rowNum + 1), (colNum + 1));
    }
    public Chunk(ChunkVector2 position)
        : this(position.rowNum, position.colNum)
    {
    }

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
    public void Unload()
    {
        GameObject.Destroy(m_body);
        m_body = null;
    }

    public void Cache()
    {
        if (m_body == null)
        {
            m_body = Resources.Load<GameObject>(m_resPath);
            m_body = GameObject.Instantiate(m_body);
        }
        m_body.SetActive(false);
    }


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