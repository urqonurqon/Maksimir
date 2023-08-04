using System;

namespace Novena.Settings
{
  [Serializable]
  public class SettingItem
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
  }
}