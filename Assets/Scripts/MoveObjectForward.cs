using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectForward : MonoBehaviour
{
    [SerializeField]
    float m_SpeedMod = 2.0f;
    
    void Update()
    {
        transform.Translate(m_SpeedMod * Time.deltaTime * transform.forward, Space.World);
    }
}
