using System;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;


namespace Novena.DAL.Database
{
  public class Database : MonoBehaviour
  {
    public static Database Instance { get; private set; }

    [SerializeField] private bool _deleteTablesOnStart;

    #region Private fields

    private string _persistentDataPath = String.Empty;

    #endregion
    
    private void Awake()
    {
      //This is for some async operations.
      //Cannot access Application.persistentDataPath on Non MainThread
      _persistentDataPath = Application.persistentDataPath;
      Instance = this;
      CreateDatabase();
    }

    public string GetConnectionString()
    {
      return "URI=file:" + _persistentDataPath + "/AppDb.db";
    }
    
    /// <summary>
    /// Create database and tables if not exist
    /// </summary>
    private void CreateDatabase()
    {
      string connectionString = GetConnectionString();
      
      string sql = string.Empty;

      //Delete tables for debug mainly
      if (_deleteTablesOnStart)
      {
        sql += "DROP TABLE IF EXISTS Files;";
        sql += "DROP TABLE IF EXISTS OnBoarding;";
        sql += "DROP TABLE IF EXISTS Guides;";
        sql += "DROP TABLE IF EXISTS Templates;";
      }
      
      //Table for Files
      sql += "CREATE TABLE IF NOT EXISTS Files (Id INTEGER , GuideId INTEGER, FilePath TEXT, LocalPath TEXT, TimeStamp TEXT, PRIMARY KEY(Id AUTOINCREMENT));";
      
      //Table for OnBoarding
      sql += "CREATE TABLE IF NOT EXISTS OnBoarding (Id INTEGER , IsOnBoarded INTEGER, PRIMARY KEY(Id AUTOINCREMENT));";

      //Table for Guides
      sql +=
        "CREATE TABLE IF NOT EXISTS Guides(Id INTEGER, GuideId INTEGER, TemplateId INTEGER, Json TEXT, Active INTEGER , PRIMARY KEY (Id AUTOINCREMENT));";

      //Table for templates
      sql += "CREATE TABLE IF NOT EXISTS Templates(Id INTEGER, TemplateId INTEGER, Json TEXT , PRIMARY KEY (Id AUTOINCREMENT))";
      
      
      using (IDbConnection connection = new SqliteConnection(connectionString))
      {
        connection.Open();
        
        IDbCommand dbcmd;
        dbcmd = connection.CreateCommand();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteReader();
        
        connection.Close();
      }
    }
  }
}