[System.Serializable]
public class Weather {
  public Location location { get; set; }
  public Current current { get; set; }
}
[System.Serializable]
public class Location {
  public string name { get; set; }
  public string region { get; set; }
  public string country { get; set; }
  public double lat { get; set; }
  public float lon { get; set; }
  public string tz_id { get; set; }
  public float localtime_epoch { get; set; }
  public string localtime { get; set; }
}


[System.Serializable]
public class Current {
  public float last_updated_epoch { get; set; }
  public string last_updated { get; set; }
  public float temp_c { get; set; }
  public float temp_f { get; set; }
  public int is_day { get; set; }
  public Condition condition { get; set; }
  public double wind_mph { get; set; }
  public double wind_kph { get; set; }
  public float wind_degree { get; set; }
  public string wind_dir { get; set; }
  public float pressure_mb { get; set; }
  public double pressure_in { get; set; }
  public double precip_mm { get; set; }
  public double precip_in { get; set; }
  public float humidity { get; set; }
  public float cloud { get; set; }
  public float feelslike_c { get; set; }
  public float feelslike_f { get; set; }
  public float vis_km { get; set; }
  public float vis_miles { get; set; }
  public float uv { get; set; }
  public double gust_mph { get; set; }
  public double gust_kph { get; set; }
}

[System.Serializable]
public class Condition {
  public string text { get; set; }
  public string icon { get; set; }
  public float code { get; set; }
}