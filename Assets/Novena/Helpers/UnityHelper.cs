using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Novena.Helpers 
{
  public class UnityHelper : MonoBehaviour
  {
    /// <summary>
    /// Destroy objects in list.
    /// </summary>
    /// <param name="objects"></param>
    public static void DestroyObjects(List<GameObject> objects)
    {
      for (int i = 0; i < objects.Count; i++)
      {
        Destroy(objects[i]);
      }
    }
    
    /// <summary>
    /// Destroy objects in list. And clear list.
    /// </summary>
    /// <param name="objects">List of objects</param>
    public static void DestroyObjects(object objects)
    {
      try
      {
        IList list = (IList)objects;

        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
          try
          {
            var monoBehaviour = list[i] as MonoBehaviour;
            Destroy(monoBehaviour.gameObject);
          }
          catch (Exception e)
          {
            Debug.LogException(e);
          }
        }
        
        list.Clear();
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }

    /// <summary>
    /// Get color from hex. Return black if conversion unsuccessful.
    /// </summary>
    /// <param name="hex">#010101 Or 010101</param>
    /// <returns>Color</returns>
    public static Color GetColor(string hex)
    {
      if (hex.Contains("#") == false)
      {
        hex = "#" + hex;
      }
      
      Color newColor = Color.black;

      if (ColorUtility.TryParseHtmlString(hex, out newColor))
      {
        return newColor;
      }
      else
      {
        Debug.LogWarning("Color conversion unsuccessuful: " + hex);
      }
      
      return newColor;
    }
    
    
  }
}
