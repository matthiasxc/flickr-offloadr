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

        private string _lastUser = "";
        public string LastUser
        {
            get { return _lastUser; }
            set { Set(ref _lastUser, value); }
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
                

                var item = await _dataService.GetData();
            }
            catch (Exception ex)
            {
            }
        }

        #region Commands

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
                    }));
            }
        }

        #endregion

        #region Settings Operations

        private void SaveSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[_apiKeyValue] = _apiKey;
            localSettings.Values[_lastUserKeyValue] = _lastUser;
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
                LastUser = (string)localSettings.Values[_lastUserKeyValue];
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