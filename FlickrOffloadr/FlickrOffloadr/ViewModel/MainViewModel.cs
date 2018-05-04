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

                        // OK, we've got our user, let's grab the public photos

                        GetPublicImagesCommand.Execute(null);

                    }));
            }
        }
        #endregion

        #endregion

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

            if (localSettings.Values.ContainsKey(_apiKeyValue))
            {
                ApiKey = (string)localSettings.Values[_apiKeyValue];
            }

            if (localSettings.Values.ContainsKey(_apiKeyValue))
            {
                TargetUser = (string)localSettings.Values[_lastUserKeyValue];
            }

            if (localSettings.Values.ContainsKey(_targetFolderKeyValue))
            {
                string token = (string)localSettings.Values[_targetFolderKeyValue];
                TargetFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            }            
        }


        #endregion
    }
}