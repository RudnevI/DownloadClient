using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadClient
{
    static class ErrorMessages
    {
        public static string ArgumentNullMessage = "Download Failed: please check whether uri and proxy server address are valid";
        public static string WebErrorMessage = "Download Failed: please check your connection to the internet and whether download uri is valid";
        public static string InvalidOperationMessage = "Download Failed: please check whether you have the rights to perform writing to disk on this computer";
    }
}
