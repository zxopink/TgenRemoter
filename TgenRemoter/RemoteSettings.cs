using System;

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

    [Serializable]
    public class RemoteSettingsObj
    {
        /// <summary>
        /// Does the client agree to get files?
        /// </summary>
        public bool CanSendFiles;

        /// <summary>
        /// Download path for remoted files
        /// </summary>
        public string FolderPath;

        public RemoteSettingsObj(bool canSendFiles, string folderPath)
        {
            this.CanSendFiles = canSendFiles;
            this.FolderPath = folderPath;
        }
    }
}
