using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickrOffloadr.AppStates
{
    public enum MainPageState
    {
        ApiEntry, 
        TestingApi, 
        UserEntry, 
        LoadingPhotos, 
        GenericLoading
    }
}
