using UnityEngine;

namespace Novena.Log
{
  public static class Log
  {
    public static void LogError(string log)
    {
      Debug.LogError(log);
    }

    public static void LogMessage(string log)
    {
      Debug.Log(log);
    }
  }
}