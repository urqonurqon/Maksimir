using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using Novena.Admin.Controller.GuideList;
using Novena.DAL.Model;
using Debug = UnityEngine.Debug;

namespace Novena.DAL.Entity {
	public class GuidesEntity : IDisposable {
		private SqliteConnection _db_connection;

		public GuidesEntity()
		{
			_db_connection = new SqliteConnection(Database.Database.Instance.GetConnectionString());
			_db_connection.Open();
		}

		/// <summary>
		/// Insert new or update existing.
		/// </summary>
		/// <param name="guideData"></param>
		public void InsertUpdate(GuideData guideData)
		{
			string sql =
				$"INSERT OR REPLACE INTO Guides (Id, GuideId, Json, TemplateId, Active) " +
				$"VALUES ((SELECT Id FROM Guides WHERE GuideId = {guideData.Guide.Id}), {guideData.Guide.Id}, @pJson, " +
				$"{guideData.Template.Id}, {guideData.IsActive})";

			IDbCommand cmnd = _db_connection.CreateCommand();
			cmnd.CommandText = sql;

			SqliteParameter pJson = new SqliteParameter("@pJson", DbType.String);
			pJson.Value = guideData.GuideJson;

			cmnd.Parameters.Add(pJson);

			cmnd.ExecuteNonQuery();
		}

		/// <summary>
		/// Update guide state!
		/// </summary>
		/// <param name="isActive"></param>
		/// <param name="guideId"></param>
		public void SetActive(bool isActive, int guideId)
		{
			string sql = $"UPDATE Guides SET Active = @pisActive " +
									 $"WHERE GuideId = {guideId}";

			IDbCommand cmnd = _db_connection.CreateCommand();
			cmnd.CommandText = sql;

			SqliteParameter pisActive = new SqliteParameter("@pisActive", DbType.Boolean);
			pisActive.Value = isActive;

			cmnd.Parameters.Add(pisActive);

			var res = cmnd.ExecuteNonQuery();

			Debug.Log($"GUIDE SET ACTIVE: {res}  {guideId}  {isActive}");
		}

		public void DeleteAll()
		{
			string sql = "DELETE FROM Guides";

			IDbCommand cmnd = _db_connection.CreateCommand();

			cmnd.CommandText = sql;

			cmnd.ExecuteNonQuery();
		}

		/// <summary>
		/// Get all guides.
		/// </summary>
		/// <returns>List of LocalGuide. Empty list if nothing found!</returns>
		public List<LocalGuide> GetAll()
		{
			List<LocalGuide> output = new List<LocalGuide>();

			string sql = "SELECT * FROM Guides";

			IDbCommand cmnd = _db_connection.CreateCommand();

			cmnd.CommandText = sql;

			IDataReader reader = cmnd.ExecuteReader();

			while (reader.Read())
			{
				LocalGuide localGuide = new LocalGuide();

				localGuide.Id = reader.GetInt32(0);
				localGuide.GuideId = reader.GetInt32(1);
				localGuide.TemplateId = reader.GetInt32(2);
				localGuide.Json = reader.GetString(3);
				localGuide.Active = reader.GetInt32(4) == 1;

				output.Add(localGuide);
			}

			return output;
		}

		/// <summary>
		/// Get guide by GuideId
		/// </summary>
		/// <param name="guideId"></param>
		/// <returns></returns>
#nullable enable
		public LocalGuide? GetGuideById(int guideId)
		{
			LocalGuide? output = null;

			IDbCommand dbcmd = _db_connection.CreateCommand();

			string sql = $"SELECT * FROM Guides WHERE GuideId = {guideId}";

			dbcmd.CommandText = sql;

			IDataReader reader = dbcmd.ExecuteReader();

			while (reader.Read())
			{
				try
				{
					output = new LocalGuide();

					output.Id = reader.GetInt32(0);
					output.GuideId = reader.GetInt32(1);
					output.TemplateId = reader.GetInt32(2);
					output.Json = reader.GetString(3);
					output.Active = reader.GetInt32(4) == 1;
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}

			return output;
		}
#nullable disable

		#region Template

		/// <summary>
		/// Insert new or update existing.
		/// </summary>
		/// <param name="json"></param>
		/// <param name="guideId"></param>
		/// <param name="active"></param>
		public void InsertUpdateTemplate(string json, int templateId)
		{
			string sql =
				$"INSERT OR REPLACE INTO Templates (Id, TemplateId, Json) VALUES " +
				$"((SELECT Id FROM Templates WHERE TemplateId = {templateId}), {templateId}, @pJson)";

			IDbCommand cmnd = _db_connection.CreateCommand();
			cmnd.CommandText = sql;

			SqliteParameter pJson = new SqliteParameter("@pJson", DbType.String);
			pJson.Value = json;

			cmnd.Parameters.Add(pJson);

			cmnd.ExecuteNonQuery();
		}
#nullable enable
		public LocalTemplate? GetTemplateById(int templateId)
		{
			LocalTemplate? output = null;

			IDbCommand dbcmd = _db_connection.CreateCommand();

			string sql = $"SELECT * FROM Templates WHERE TemplateId = {templateId}";

			dbcmd.CommandText = sql;

			IDataReader reader = dbcmd.ExecuteReader();

			while (reader.Read())
			{
				try
				{
					output = new LocalTemplate();

					output.Id = Convert.ToInt32(reader[0]);
					output.TemplateId = Convert.ToInt32(reader[1]);
					output.Json = Convert.ToString(reader[2]);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}

				return output;
			}

			return output;
		}

#nullable disable
		#endregion

		public void Dispose()
		{
			_db_connection?.Close();
			_db_connection?.Dispose();
		}
	}
}