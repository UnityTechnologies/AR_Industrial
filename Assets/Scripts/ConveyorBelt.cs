using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
   [SerializeField]
   GameObject m_ConveyorBeltCube;
   
   public GameObject conveyorBeltCube
   {
      get => m_ConveyorBeltCube;
      set => m_ConveyorBeltCube = value;
   }

   [SerializeField]
   Transform m_SpawnPoint;
   
   public Transform spawnPoint
   {
      get => m_SpawnPoint;
      set => m_SpawnPoint = value;
   }

   [SerializeField]
   float m_SpawnDelay = 1.0f;

   public float spawnDelay
   {
      get => m_SpawnDelay;
      set => m_SpawnDelay = value;
   }

   float m_Time = 0.0f;
   bool m_MovingBelt = false;

   void Update()
   {
      if (m_MovingBelt)
      {
         m_Time += Time.deltaTime;

         if (m_Time >= m_SpawnDelay)
         {
            SpawnObject();
            m_Time = 0.0f;
         }
      }
   }

   void OnTriggerEnter(Collider other)
   {
      Destroy(other.gameObject);
   }

   void SpawnObject()
   {
      Instantiate(m_ConveyorBeltCube, m_SpawnPoint.position, m_SpawnPoint.rotation);
   }

   public void StartConveyorBelt()
   {
      SpawnObject();
      m_MovingBelt = true;
   }
}
