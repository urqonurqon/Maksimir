using System;
using UnityEngine;

namespace Novena.DAL
{
  public class DataAccess : MonoBehaviour
  {
    public static DataAccess Instance;
    
    [SerializeField] public string downloadCode;

    private void Awake()
    {
      Instance = this;
    }
  }
}