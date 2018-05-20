using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using FlickrOffloadr.Model;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.Net.Http;
using FlickrOffloadr.Model.FlickrModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace FlickrOffloadr.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly string _userUrl = "https://api.flickr.com/services/rest/?method=flickr.urls.lookupUser&api_key={0}&url=https%3A%2F%2Fwww.flickr.com%2Fphotos%2F{1}%2F&format=json&nojsoncallback=1";
        private readonly string _publicPhotosUrl = "https://api.flickr.com/services/rest/?method=flickr.people.getPublicPhotos&api_key={0}&user_id={1}&per_page=500&format=json&nojsoncallback=1&page={2}";
        private readonly string _photoSizesUrl = "https://api.flickr.com/services/rest/?method=flickr.photos.getSizes&api_key={0}&photo_id={1}&format=json&nojsoncallback=1";
        private readonly string _photoDetails = " https://api.flickr.com/services/rest/?method=flickr.photos.getInfo&api_key={0}&photo_id={1}&format=json&nojsoncallback=1";
        

        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly string _apiKeyValue = "flickrApiKey"; 
        private readonly string _lastUserKeyValue = "lastSearchedUser";
        private readonly string _targetFolderKeyValue = "targetFolderToken";
        private readonly string _useFolderStructureValue = "useFolderStructureBool";
        private string _targetFolderToken = "";

        #region Properties 

        private bool _isApiUiVisible = false;
        public bool IsApiUiVisible
        {
            get { return _isApiUiVisible; }
            set { Set(ref _isApiUiVisible, value); }
        }

        private bool _isApiLoading = false;
        public bool IsApiLoading
        {
            get { return _isApiLoading; }
            set { Set(ref _isApiLoading, value); }
        }

        private bool _isMainUiVisible = false;
        public bool IsMainUiVisible
        {
            get { return _isMainUiVisible; }
            set { Set(ref _isMainUiVisible, value); }
        }

        private string _apiKey = "";
        public string ApiKey
        {
            get { return _apiKey; }
            set { Set(ref _apiKey, value); }
        }

        private string _targetUser = "";
        public string TargetUser
        {
            get { return _targetUser; }
            set { Set(ref _targetUser, value); }
        }

        private FlickrUser _searchedUser;
        public FlickrUser SearchedUser
        {
            get { return _searchedUser; }
            set { Set(ref _searchedUser, value); }
        }

        private bool _isTargetFolderSelected = false;
        public bool IsTargetFolderSelected
        {
            get { return _isTargetFolderSelected; }
            set { Set(ref _isTargetFolderSelected, value); }
        }

        private StorageFolder _targetFolder;
        public StorageFolder TargetFolder
        {
            get { return _targetFolder; }
            set { Set(ref _targetFolder, value); }
        }

        private ObservableCollection<Photo> _publicPhotos = new ObservableCollection<Photo>();
        public ObservableCollection<Photo> PublicPhotos
        {
            get { return _publicPhotos; }
            set { Set(ref _publicPhotos, value); }
        }

        private ObservableCollection<Photo> _photosToSave = new ObservableCollection<Photo>();
        public ObservableCollection<Photo> PhotosToSave
        {
            get { return _photosToSave; }
            set { Set(ref _photosToSave, value); }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { Set(ref _currentPage, value); }
        }

        private int _pageCount = 0;
        public int PageCount
        {
            get { return _pageCount; }
            set { Set(ref _pageCount, value); }
        }

        #region Progress Properties 

        private int _totalPhotoCount= 0;
        public int TotalPhotoCount
        {
            get { return _totalPhotoCount; }
            set { Set(ref _totalPhotoCount, value); }
        }

        private int _downloadedFilesCount = 0;
        public int DownloadedFilesCount
        {
            get { return _downloadedFilesCount; }
            set { Set(ref _downloadedFilesCount, value); }
        }

        private string _estimatedDownloadTime = "";
        public string EstimatedDownloadTime
        {
            get { return _estimatedDownloadTime; }
            set { Set(ref _estimatedDownloadTime, value); }
        }

        private string _remainingDownloadTime = "";
        public string RemainingDownloadTime
        {
            get { return _remainingDownloadTime; }
            set { Set(ref _remainingDownloadTime, value); }
        }

        private TimeSpan _downloadDelay = new TimeSpan(0, 0, 0, 0, 1800);
        public TimeSpan DownloadDelay
        {
            get { return _downloadDelay; }
            set { Set(ref _downloadDelay, value); }
        }

        private string _downloadStatus = "";
        public string DownloadStatus
        {
            get { return _downloadStatus; }
            set { Set(ref _downloadStatus, value); }
        }

        private string _gatherStatus = "";
        public string GatherStatus
        {
            get { return _gatherStatus; }
            set { Set(ref _gatherStatus, value); }
        }

        private string _runningTime = "";
        public string RunningTime
        {
            get { return _runningTime; }
            set { Set(ref _runningTime, value); }
        }


        private double _percentDone = 0.0;
        public double PercentDone
        {
            get { return _percentDone; }
            set { Set(ref _percentDone, value); }
        }

        private bool _isDownloading = false;
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { Set(ref _isDownloading, value); }
        }

        private bool _isCheckingPhotos = false;
        public bool IsCheckingPhotos
        {
            get { return _isCheckingPhotos; }
            set { Set(ref _isCheckingPhotos, value); }
        }

        private string _downloadingPhoto= "";
        public string DownloadingPhoto
        {
            get { return _downloadingPhoto; }
            set { Set(ref _downloadingPhoto, value); }
        }

        private bool _isCancelled = false;
        public bool IsCancelled
        {
            get { return _isCancelled;  }
            set { Set(ref _isCancelled, value); } 
        }

        private bool _useFolderStructure = true;
        public bool UseFolderStructure
        {
            get { return _useFolderStructure; }
            set
            {
                Set(ref _useFolderStructure, value);

                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values[_useFolderStructureValue] = _useFolderStructure;
            }
        }

        #endregion

        #endregion

        public MainViewModel(
            IDataService dataService,
            INavigationService navigationService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Initialize();
        }
        
        private async Task Initialize()
        {
            try
            {
                await LoadSettings();

                IsApiUiVisible = String.IsNullOrEmpty(ApiKey);
                IsMainUiVisible = !IsApiUiVisible;

                if (TargetFolder != null)
                {
                    IsTargetFolderSelected = true;
                }

                var item = await _dataService.GetData();
            }
            catch (Exception ex)
            {
            }
        }

         #region Commands

        #region SetApiKeyCommand
        private RelayCommand _setApiCommand;

        /// <summary>
        /// Gets the SetApiKeyCommand.
        /// </summary>
        public RelayCommand SetApiKeyCommand
        {
            get
            {
                return _setApiCommand
                    ?? (_setApiCommand = new RelayCommand(
                    () =>
                    {
                        SaveSettings();
                        IsApiUiVisible = String.IsNullOrEmpty(ApiKey);
                        IsMainUiVisible = !IsApiUiVisible;
                        // TODO: test the api key to make sure it is valid
                    }));
            }
        }
        #endregion

        #region SetFolderStructureCommand
        private RelayCommand _setFolderStructureCommand;

        /// <summary>
        /// Gets the SetApiKeyCommand.
        /// </summary>
        public RelayCommand SetFolderStructureCommand
        {
            get
            {
                return _setFolderStructureCommand
                    ?? (_setFolderStructureCommand = new RelayCommand(
                    () =>
                    {
                        SaveSettings();
                    }));
            }
        }
        #endregion

        #region SelectTargetFolderCommand
        private RelayCommand _selectTargetFolder;

        /// <summary>
        /// Gets the SelectTargetFolder.
        /// </summary>
        public RelayCommand SelectTargetFolderCommand
        {
            get
            {
                return _selectTargetFolder
                    ?? (_selectTargetFolder = new RelayCommand(
                    async () =>
                    {
                        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                        var picker = new Windows.Storage.Pickers.FolderPicker();
                        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                        picker.FileTypeFilter.Add("*");

                        var folder = await picker.PickSingleFolderAsync();
                        //  https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-using-file-and-folder-pickers
                        if(folder != null)
                        {
                            TargetFolder = folder;
                            SaveSettings();
                            IsTargetFolderSelected = true;
                        }
                    }));
            }
        }
        #endregion

        #region GetFlickrUserCommand
        private RelayCommand _getFlickrUser;

        /// <summary>
        /// Gets the GetFlickrUser.
        /// </summary>
        public RelayCommand GetFlickrUserCommand
        {
            get
            {
                return _getFlickrUser
                    ?? (_getFlickrUser = new RelayCommand(
                    async () =>
                    {
                        string userUrl = String.Format(_userUrl, _apiKey, TargetUser);
                        using (var client = new HttpClient())
                        {
                            var result = await client.GetStringAsync(new Uri(userUrl));

                            UserCall typedResult = JsonConvert.DeserializeObject<UserCall>(result);
                            if (typedResult.Stat == "ok")
                            {
                                _searchedUser = typedResult.User;
                            }
                            else
                            {
                                return;
                            }
                        }
                        SaveSettings();

                        // OK, we've got our user, let's grab the public photos

                        GetPublicImagesCommand.Execute(null);

                    }));
            }
        }
        #endregion

        #region GetPublicImagesCommand
        private RelayCommand _getPublicImagesCommand;

        /// <summary>
        /// Gets the GetPublicImagesCommand.
        /// </summary>
        public RelayCommand GetPublicImagesCommand
        {
            get
            {
                return _getPublicImagesCommand
                    ?? (_getPublicImagesCommand = new RelayCommand(
                    async () =>
                    {
                        await GetPublicImages();
            }));
            }
        }

        private async Task GetPublicImages()
        {
            PublicPhotos.Clear();
            string publicPhotoUrl = String.Format(_publicPhotosUrl, _apiKey, _searchedUser.Id, CurrentPage);
            PublicPhotos publicPhotos = null;
            IsCheckingPhotos = true;
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(new Uri(publicPhotoUrl));

                publicPhotos = JsonConvert.DeserializeObject<PublicPhotos>(result);
            }

            if (publicPhotos != null)
            {
                TotalPhotoCount = System.Convert.ToInt32(publicPhotos.Photos.Total);
                PageCount = publicPhotos.Photos.Pages;
            }

            // Look at the already downloaded files in that directory.

            ////  Remove them from the list of files to download 
            //IReadOnlyList<StorageFile> fileList = await TargetFolder.GetFilesAsync();
            GatherStatus = "Checking Existing Photos... ";
            List<string> downloadedFiles = await GetFilesInDirectory(TargetFolder);
            DownloadedFilesCount = 0;
            //foreach (StorageFile file in fileList)
            //{
            //    downloadedFiles.Add(file.Name);
            //}

            for (int i = 1; i <= PageCount; i++)
            {
                Debug.WriteLine("Loading Photos from page " + i.ToString());
                GatherStatus = String.Format("Loading page {0} of {1}", i, PageCount); 

                string photoPageUrl = String.Format(_publicPhotosUrl, _apiKey, _searchedUser.Id, i);
                PublicPhotos publicPhotosHolder = null;
                using (var client = new HttpClient())
                {
                    var result = await client.GetStringAsync(new Uri(photoPageUrl));

                    publicPhotosHolder = JsonConvert.DeserializeObject<PublicPhotos>(result);
                }
                // select all the photos we haven't already downloaded
                foreach (Photo photo in publicPhotosHolder.Photos.PhotoList)
                {
                    photo.PhotoDetailsUrl = String.Format(_photoSizesUrl, _apiKey, photo.Id);
                    photo.PhotoThumb = String.Format("https://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg",
                                                                                photo.Farm,
                                                                                photo.Server,
                                                                                photo.Id,
                                                                                photo.Secret);
                    PublicPhotos.Add(photo);

                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        photo.Title = photo.Title.Replace(c, '_');
                    }

                    var existingFile = downloadedFiles.FirstOrDefault(f => f.Contains(photo.Title + "_" + photo.Secret));
                    if (existingFile == null)
                    {
                        PhotosToSave.Add(photo);
                    }
                    else
                    {
                        DownloadedFilesCount++;
                        //Debug.WriteLine("Already Downloaded: " + photo.Title);
                    }
                }

                await Task.Delay(100);
            }
            IsCheckingPhotos = false;

            TimeSpan estTime = new TimeSpan(0, 0, 0, 0, System.Convert.ToInt32(PhotosToSave.Count * (_downloadDelay.TotalMilliseconds + 2000)));
            EstimatedDownloadTime = "Estimated Download Time: " + estTime.ToString(@"hh\:mm\:ss");
        }
        #endregion

        #region SelectAllPagesCommand
        private RelayCommand _downloadAllPagesCommand;

        /// <summary>
        /// Gets the SelectAllPagesCommand.
        /// </summary>
        public RelayCommand DownloadAllPagesCommand
        {
            get
            {
                return _downloadAllPagesCommand
                ?? (_downloadAllPagesCommand = new RelayCommand(
                async () =>
                {
                    IsDownloading = true;

                    if (IsCancelled)
                    {
                        IsCancelled = false;
                        await GetPublicImages();
                    }


                    int photoDownloadCount = PhotosToSave.Count;
                    int currentCount = 0;
                    DateTime startTime = DateTime.Now;
                    for (int i = 0; i < PhotosToSave.Count; i++)
                    {
                        if (IsCancelled)
                            break;

                        currentCount++;
                        DownloadStatus = "Downloading Photo " + currentCount + " of " + PhotosToSave.Count;

                        DownloadingPhoto = PhotosToSave[i].PhotoThumb;

                        try
                        {
                            // Get the appropriate format and date info for the photo
                            using (var client = new HttpClient())
                            {
                                string photoFormatUrl = String.Format(_photoDetails, _apiKey, PhotosToSave[i].Id);
                                var detailsResult = await client.GetStringAsync(new Uri(photoFormatUrl));
                                var photoWithFormat = JsonConvert.DeserializeObject<PhotoDetails>(detailsResult);
                                PhotosToSave[i].Format = photoWithFormat.Photo.Format;
                                PhotosToSave[i].Dates = photoWithFormat.Photo.Dates;
                            }

                            PhotosToSave[i].PhotoDetailsUrl = String.Format(_photoSizesUrl, _apiKey, PhotosToSave[i].Id);
                            // Get the links to the original file
                            using (var client = new HttpClient())
                            {
                                var result = await client.GetStringAsync(new Uri(PhotosToSave[i].PhotoDetailsUrl));
                                var photoDetailsResult = JsonConvert.DeserializeObject<PhotoSizes>(result);
                                PhotosToSave[i].Details = photoDetailsResult.Sizes;
                            }

                            TimeSpan remainingTime = new TimeSpan(0, 0, 0, 0, System.Convert.ToInt32((PhotosToSave.Count - i) * (_downloadDelay.TotalMilliseconds+2000)));
                            RemainingDownloadTime = "Download Complete in: " + remainingTime.ToString(@"hh\:mm\:ss");

                            TimeSpan runningTime = DateTime.Now - startTime;
                            RunningTime = "Running Time: " + runningTime.ToString(@"hh\:mm\:ss"); 

                            PercentDone = System.Convert.ToDouble(i) / System.Convert.ToDouble(PhotosToSave.Count);

                            await SaveFileFromPhotoDetails(PhotosToSave[i]);

                            await Task.Delay(DownloadDelay);

                        }
                        catch
                        {
                            Debug.WriteLine("Failed to load " + PhotosToSave[i].Id + " (" + PhotosToSave[i].Title + ")");
                        }

                    }

                    IsDownloading = false;

                }));
            }
        }
        #endregion

        #region CancelDownloadCommand
        private RelayCommand _cancelDownloadCommand;

        /// <summary>
        /// Gets the SelectAllPagesCommand.
        /// </summary>
        public RelayCommand CancelDownloadCommand
        {
            get
            {
                return _cancelDownloadCommand
                ?? (_cancelDownloadCommand = new RelayCommand(
                async () =>
                {
                    IsCancelled = true;
                }));
            }
        }

        #endregion

        #endregion
        private async Task<bool> SaveFileFromPhotoDetails(Photo photo)
        {
            if (photo.Details != null && photo.Details.CanDownload)
            {
                try
                {
                    var originalPhoto = photo.Details.Sizes.FirstOrDefault(p => p.Label.ToLower() == "original");
                    string photoFormat = ".jpg";

                    if (!String.IsNullOrEmpty(photo.Format)) photoFormat = "." + photo.Format;

                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        photo.Title = photo.Title.Replace(c, '_');
                    }

                    StorageFile photoFile = null;

                    // Look for a year / month folder for the target folder to download into
                    if (photo.Dates.DateTaken != null && UseFolderStructure)
                    {
                        var yearFolder = await TargetFolder.CreateFolderAsync(photo.Dates.DateTaken.Year.ToString(), CreationCollisionOption.OpenIfExists);
                        var monthFolder = await yearFolder?.CreateFolderAsync(photo.Dates.DateTaken.Month.ToString(), CreationCollisionOption.OpenIfExists);
                        photoFile = await monthFolder?.CreateFileAsync(photo.Title + "_" + photo.Secret + photoFormat, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    }
                    
                    if(photoFile == null)
                    {
                        photoFile = await TargetFolder?.CreateFileAsync(photo.Title + "_" + photo.Secret + photoFormat, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    }

                    if (originalPhoto != null && photoFile != null)
                    {
                        HttpClient client = new HttpClient();
                        Debug.WriteLine("Downloading:");
                        Debug.WriteLine(originalPhoto.Source);
                        byte[] buffer = await client.GetByteArrayAsync(originalPhoto.Source); // Download file
                        using (Stream stream = await photoFile.OpenStreamForWriteAsync())
                        {
                            stream.Write(buffer, 0, buffer.Length); // Save
                        }
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        #region Settings Operations

        private void SaveSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[_apiKeyValue] = _apiKey;
            localSettings.Values[_lastUserKeyValue] = _targetUser;

            if(_targetFolder != null)
            { 
                localSettings.Values[_targetFolderKeyValue] = StorageApplicationPermissions.FutureAccessList.Add(_targetFolder);
            }
        }

        private async Task LoadSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(_useFolderStructureValue))
            {
                UseFolderStructure = (bool)localSettings.Values[_useFolderStructureValue];
            }

            if (localSettings.Values.ContainsKey(_apiKeyValue))
            {
                ApiKey = (string)localSettings.Values[_apiKeyValue];
            }

            if (localSettings.Values.ContainsKey(_lastUserKeyValue))
            {
                TargetUser = (string)localSettings.Values[_lastUserKeyValue];
            }

            if (localSettings.Values.ContainsKey(_targetFolderKeyValue))
            {
                string token = (string)localSettings.Values[_targetFolderKeyValue];
                TargetFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            }            
        }

        private async Task<List<string>> GetFilesInDirectory(StorageFolder folder)
        {
            List<string> allFiles = new List<string>();

            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                allFiles.Add(file.Name);
            }

            var subfolders = await folder.GetFoldersAsync();
            foreach (var f in subfolders)
            {
                var subFiles = await GetFilesInDirectory(f);
                allFiles.AddRange(subFiles);
            }

            return allFiles;
        }


        #endregion
    }
}