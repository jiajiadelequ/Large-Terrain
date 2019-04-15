using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoThread : MonoSingleton<MonoThread>
{
    public void Excute(IEnumerator en)
    {
        StartCoroutine(en);
    }

}
