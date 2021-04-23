using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgenRemoter
{
    public static class RemoteSettings
    {
        /// <summary>
        /// Does the client agree to get files?
        /// </summary>
        public static bool CanSendFiles;

        /// <summary>
        /// Download path for remoted files
        /// </summary>
        public static string FolderPath;
    }
}
