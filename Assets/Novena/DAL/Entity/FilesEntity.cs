using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using Mono.Data.Sqlite;
using Novena.DAL.Model;

namespace Novena.DAL.Entity
{
  public class FilesEntity : IDisposable
  {
    private SqliteConnection _db_connection;

    public FilesEntity()
    {
      _db_connection = new SqliteConnection(Database.Database.Instance.GetConnectionString());
      _db_connection.Open();
    }
    
    /// <summary>
    /// Insert new file to database 
    /// </summary>
    /// <param name="file"></param>
    public void Insert(File file)
    {
      string sql =
        "INSERT INTO Files (GuideId, FilePath, LocalPath, TimeStamp)" +
         "VALUES ( '" + file.GuideId + "', '"
                       + file.FilePath + "', '"
                       + file.LocalPath + "', '"
                       + file.TimeStamp + "' )";

      IDbCommand cmnd = _db_connection.CreateCommand();

      cmnd.CommandText = sql;
      cmnd.ExecuteNonQuery();
    }

    /// <summary>
    /// Update existing record.
    /// </summary>
    /// <param name="file"></param>
    public void Update(File file)
    {
      string sql = $"UPDATE Files SET TimeStamp='{file.TimeStamp}' WHERE FilePath='{file.FilePath}';";

      IDbCommand cmnd = _db_connection.CreateCommand();

      cmnd.CommandText = sql;
      cmnd.ExecuteNonQuery();
    }

    [CanBeNull]
    public File GetByFilePath(string filePath)
    {
      File file = null;

      IDbCommand dbcmd = _db_connection.CreateCommand();
      
      string sql = $"SELECT * FROM Files WHERE FilePath = '{filePath}'";
      
      dbcmd.CommandText = sql;

      IDataReader reader = dbcmd.ExecuteReader();
      
      while (reader.Read())
      {
        file = new File();
        
        file.Id = Convert.ToInt32(reader[0]);
        file.GuideId = Convert.ToInt32(reader[1]);
        file.FilePath = reader[2].ToString();
        file.LocalPath = reader[3].ToString();
        file.TimeStamp = reader[4].ToString();
        
        //Debug.Log("GetByFilePath " + file.LocalPath);
      }
      
      return file;
    }

    /// <summary>
    /// Get all files from db.
    /// </summary>
    /// <returns>List of files or empty list if nothing found!</returns>
    public List<File> GetAll()
    {
      List<File> files = new List<File>();

      IDbCommand dbcmd = _db_connection.CreateCommand();
      
      string sql = "SELECT * FROM Files";
      
      dbcmd.CommandText = sql;

      IDataReader reader = dbcmd.ExecuteReader();
      
      while (reader.Read())
      {
        var file = new File();
        
        file.Id = Convert.ToInt32(reader[0]);
        file.GuideId = Convert.ToInt32(reader[1]);
        file.FilePath = reader[2].ToString();
        file.LocalPath = reader[3].ToString();
        file.TimeStamp = reader[4].ToString();
        
        //Debug.Log("GetByFilePath " + file.LocalPath);
        files.Add(file);
      }
      
      return files;
    }
    
    public int DeleteById(int id)
    {
      IDbCommand dbcmd = _db_connection.CreateCommand();

      string sql = $"DELETE FROM Files WHERE Id = {id}";

      dbcmd.CommandText = sql;

      return dbcmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
      _db_connection?.Close();
      _db_connection?.Dispose();
    }
  }
}