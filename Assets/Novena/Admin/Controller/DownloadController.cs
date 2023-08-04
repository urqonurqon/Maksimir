using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Doozy.Engine.Progress;
using Newtonsoft.Json;
using Novena.Admin.Controller.GuideList;
using Novena.Admin.FileSave;
using Novena.Admin.Utility;
using Novena.DAL.Entity;
using Novena.DAL.Model;
using Novena.Networking;
using Novena.UiUtility.Base;
using TMPro;
using UnityEngine;

namespace Novena.Admin.Controller
{
  public class DownloadController : UiController
  {
    public static List<GuideData> GuideDataList { get; set; }

    #region Serialiezed fields

    [Header("Download info")]
    [SerializeField] private TMP_Text _guideName;
    [SerializeField] private TMP_Text _downloadSize;
    [SerializeField] private TMP_Text _fileCount;
    
    
    [Header("Downloader components")]
    [Header("Total progress")]
    [SerializeField] private TMP_Text _totalSizeTmp;
    [SerializeField] private TMP_Text _currentDownloadTmp;
    [SerializeField] private Progressor _progressor;

    [Header("Guide progress")]
    [SerializeField] private TMP_Text _guideTotalSizeTmp;
    [SerializeField] private TMP_Text _guideCurrentDownloadTmp;
    [SerializeField] private Progressor _guideProgressor;

    [Header("File progress")]
    [SerializeField] private TMP_Text _fileTotalSizeTmp;
    [SerializeField] private TMP_Text _fileCurrentDownloadTmp;
    [SerializeField] private TMP_Text _filePathTmp;
    [SerializeField] private Progressor _fileProgressor;


    [SerializeField] private TMP_Text _messageInfo;
    
    #endregion

    #region Private fields
    
    private float _currentFileSize = 0;
    private double _curentlyDownloaded = 0;
    private double _totalCurentlyDownloaded = 0;
    private string _currentFilePath = string.Empty;
    private int _currentDownloadIndex = 0;

    private GuideData _activeGuideData;
    private float _currentProgress;
    private float _oldProgress;
    
    private CancellationTokenSource _cancelSource;

    #endregion

    public override void Awake()
    {
      GuideDataList = new List<GuideData>();
      base.Awake();
    }

    public override void OnShowViewStart()
    {
      StartDownload();
    }

    public override void OnHideViewFinished()
    {
      GuideDataList.Clear();
      _cancelSource.Cancel();
    }

    private void StartDownload()
    {
      _currentFileSize = 0;
      _currentDownloadIndex = 0;
      _currentFilePath = string.Empty;
      _currentProgress = 0;
      _curentlyDownloaded = 0;
      _totalCurentlyDownloaded = 0;
      _oldProgress = 0;
          
      DownloadFiles().Forget();
    }
    
    private void SetDownloadData(GuideData guideData)
    {
      _guideName.text = guideData.Guide.Name;
      _downloadSize.text = guideData.Files.DownloadSize.ToString();
      _fileCount.text = guideData.Files.FilesToDownload.Count.ToString();
    }

    private async UniTaskVoid DownloadFiles()
    {
      _cancelSource = new CancellationTokenSource();
      var token = _cancelSource.Token;

      SetTotalDownloadProggres();

      foreach (var guideData in GuideDataList)
      {
        SetGuideProgressData(guideData);
        
        using (FilesEntity filesEntity = new FilesEntity())
        {
          for (int i = 0; i < guideData.Files.FilesToDownload.Count; i++)
          {
            var fileToDownload = guideData.Files.FilesToDownload[i];

            var url = fileToDownload.Path;

            _currentFileSize = fileToDownload.Size;

            SetFileData(fileToDownload);
            SetProgressorValue(_fileProgressor, _currentFileSize);

            _currentFilePath = url;

            var downloaded = await Downloader.Download(url, SetFileProgress, token);

            //If file is downloaded and saved to disk let's record it to db
            if (downloaded)
            {
              File file = new File();
              file.FilePath = fileToDownload.Path;
              file.LocalPath = Api.GetFullPath(fileToDownload.Path);
              file.TimeStamp = fileToDownload.TimeStamp;
              file.GuideId = guideData.Guide.Id;

              //Get file from db.
              var f = filesEntity.GetByFilePath(fileToDownload.Path);

              //If file exist just update time stamp.
              if (f != null)
              {
                filesEntity.Update(file);
              }
              else
              {
                //Store to db
                filesEntity.Insert(file);  
              }
              Debug.Log("File downloaded: " + fileToDownload.Path);
            }
            else
            {
              Debug.LogError("Not downloaded: " + url);
            }

            _oldProgress = 0;
          }

          _curentlyDownloaded = 0;
        }
        
        //Let's store new guide JSON!
        using (GuidesEntity guidesEntity = new GuidesEntity())
        {
          //Save guide to db
          guidesEntity.InsertUpdate(guideData);

          //Save template to db
          string templateJson = JsonConvert.SerializeObject(guideData.Template);
          guidesEntity.InsertUpdateTemplate(templateJson, guideData.Template.Id);
        }
      }

      FileSave.Files.DeleteUnusedFiles().Forget();
      AdminUtility.LoadGuideScene();
    }

    private void SetTotalDownloadProggres()
    {
      ulong totalDownloadSize = 0;
      foreach (var guide in GuideDataList)
      {
          totalDownloadSize += guide.Files.DownloadSize;
      }

      _totalSizeTmp.text = GetSizeToMB(totalDownloadSize);
      _progressor.SetMax(totalDownloadSize);
    }

    private void SetGuideProgressData(GuideData guideData)
    {
      _guideName.text = guideData.Guide.Name;
      _guideTotalSizeTmp.text = GetSizeToMB(guideData.Files.DownloadSize);
      _downloadSize.text = GetSizeToMB(guideData.Files.DownloadSize);
      _fileCount.text = guideData.Files.FilesToDownload.Count.ToString();
      SetProgressorValue(_guideProgressor, 0, guideData.Files.DownloadSize);
    }
    
    /// <summary>
    /// Set progress of download in percentage
    /// </summary>
    /// <param name="value">Current download progress</param>
    /// <param name="maxValue">Max value for percentage calculation</param>
    private void SetProgressorValue(Progressor progressor, float value, float maxValue = 0)
    {
      progressor.SetValue(value);

      if (maxValue > 0)
      {
        //This is to set how progressor calculates percentage
        progressor.SetMax(maxValue);
      }
    }

    private void SetFileData(GuideFile file)
    {
      _fileTotalSizeTmp.text = GetSizeToMB(file.Size);
    }

    private void SetFileProgress(float progress)
    {
      _fileProgressor.SetValue(progress);
      _filePathTmp.text = _currentFilePath;
      _fileCurrentDownloadTmp.text = GetProgressInMB(progress, _currentFileSize);
      CurrentGuideProgress(progress);
    }

    private void SetGuideProgress(GuideData guideData)
    {

    }

    /// <summary>
    /// Convert current progress to MB
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="maxValue"></param>
    private string GetProgressInMB(float progress, float maxValue)
    {
      var current = (maxValue * progress) / 100;
      return GetSizeToMB(current);
    }

    /// <summary>
    /// Update progress for each file download
    /// </summary>
    /// <param name="progress"></param>
    private void CurrentGuideProgress(float progress)
    {
      progress *= 100;
      _currentProgress = progress - _oldProgress;
      _oldProgress = progress;
      
      _curentlyDownloaded += (_currentFileSize * _currentProgress) / 100;
      _totalCurentlyDownloaded += (_currentFileSize * _currentProgress) / 100;

      _guideProgressor.SetValue((float) _curentlyDownloaded);
      _progressor.SetValue((float)_totalCurentlyDownloaded);
      _guideCurrentDownloadTmp.text = GetSizeToMB((float) _curentlyDownloaded);
      _currentDownloadTmp.text = GetSizeToMB((float) _totalCurentlyDownloaded);
    }
    
    /// <summary>
    /// Set TMP of current download progress in MB.
    /// </summary>
    /// <param name="value"></param>
    private void SetCurrentDownloadSize(float value)
    {
      _currentDownloadTmp.text = ((value / 1024) / 1024).ToString("0.##") + " MB";
    }
    
    /// <summary>
    /// Set's text to total size in MB
    /// </summary>
    private void SetTotalDownloadSize(float value)
    {
      _totalSizeTmp.text = ((value / 1024) / 1024).ToString("0.##") + " MB";
    }

    /// <summary>
    /// Convert bytes to MB
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string GetSizeToMB(float value)
    {
      return ((value / 1024) / 1024).ToString("0.##") + " MB";
    }
  }
}