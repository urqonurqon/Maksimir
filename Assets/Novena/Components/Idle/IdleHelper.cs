using System;
using System.Linq;
using Doozy.Engine.Nody;
using UnityEngine;

namespace Novena.Components.Idle
{
  public static class IdleHelper
  {
    private static GraphController _graphController;
    public static void GoToIdleNode(string nodeName)
    {
      try
      {
        GetAndSetController();
        
        if (_graphController is { })
        {
          //If node is all ready active dont invoke it again
          if (_graphController.Graph.ActiveNode.Name == nodeName) return;
          _graphController.Graph.SetActiveNodeByName(nodeName);
        }
      }
      catch (Exception e)
      {
        Debug.Log(e);
      }
    }
    
    private static void GetAndSetController()
    {
      if (_graphController != null) return;
      
      _graphController = GraphController.Database
        .FirstOrDefault(d => d.ControllerName == "Navigation");
    }
  }
}