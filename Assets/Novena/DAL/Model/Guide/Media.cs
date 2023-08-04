using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using JetBrains.Annotations;
using Novena.Admin.FileSave.Attribute;
using Novena.Networking;

namespace Novena.DAL.Model.Guide {
	[Serializable]
	public class Media {
		public int Id { get; set; }
		public string Name { get; set; }
		public int Rank { get; set; }
		public int MediaTypeId { get; set; }
		public Photo[] Photos { get; set; }
		public string Text { get; set; }
		[Admin.FileSave.Attribute.File(FileAttributeType.Path)]
		public string ContentPath { get; set; }

		[Admin.FileSave.Attribute.File(FileAttributeType.Timestamp)]
		public string ContentTimestamp { get; set; }

		[Admin.FileSave.Attribute.File(FileAttributeType.Size)]
		public ulong ContentSize { get; set; }

		private string _fullOnlinePath;
		private string _fullLocalPath;

		/// <summary>
		/// Get full online path to file.
		/// </summary>
		public string FullOnlinePath
		{
			get => GetFullOnlinePath();
			protected set => _fullOnlinePath = value;
		}

		/// <summary>
		/// Get full local path to file.
		/// </summary>
		/// <example>
		/// PersistentDataPath/DownloadCode/ContentPath
		/// </example>
		public string FullLocalPath
		{
			get => GetFullLocalPath();
			protected set => _fullLocalPath = value;
		}

		/// <summary>
		/// Get full online path to file.
		/// </summary>
		/// <remarks>
		/// Removes ~ from ContentPath.
		/// </remarks>
		/// <returns>File url. Removes ~ from ContentPath. Empty string if ContentPath is null!</returns>
		private string GetFullOnlinePath()
		{
			string output = "";

			if (string.IsNullOrWhiteSpace(ContentPath) == false)
			{
				output = Api.DOWNLOAD_FILES + ContentPath.Replace("~", "");
			}

			return output;
		}

		/// <summary>
		/// Get full local path to file.
		/// </summary>
		/// <example>
		/// PersistentDataPath/DownloadCode/ContentPath
		/// </example>
		/// <remarks>
		/// Removes ~ from ContentPath.
		/// </remarks>
		/// <returns>File url. Empty string if ContentPath is null!</returns>
		private string GetFullLocalPath()
		{
			string output = "";

			if (string.IsNullOrWhiteSpace(ContentPath) == false)
			{
				output = Api.GetFullPath(ContentPath.Replace("~", ""));
			}

			return output;
		}


		#region Helper methods

		/// <summary>
		/// Get list of photos ordered by rank!
		/// </summary>
		/// <returns>Ordered list of photos or NULL if nothing found.</returns>
#nullable enable
		[CanBeNull]
		public List<Photo>? GetPhotos()
		{
			if (Photos == null) return null;

			var orderedPhotos = Photos.OrderBy(photo => photo.Rank).ToList();

			return orderedPhotos;
		}
#nullable disable
		#endregion
	}

	[Serializable]
	public class Photo {
		public int Id { get; set; }
		public int Rank { get; set; }
		public string Name { get; set; }

		[Admin.FileSave.Attribute.File(FileAttributeType.Path)]
		public string Path { get; set; }

		[Admin.FileSave.Attribute.File(FileAttributeType.Timestamp)]
		public string Timestamp { get; set; }

		[Admin.FileSave.Attribute.File(FileAttributeType.Size)]
		public ulong Size { get; set; }
		public string Description { get; set; }

		private string _fullPath;

		/// <summary>
		/// Get full path to image either on internet or local file.
		/// </summary>
		public string FullPath
		{
			get => GetFullPath();
			protected set => _fullPath = value;
		}

		private string GetFullPath()
		{
			return Api.GetFullLocalPath(Path);
		}
	}
}