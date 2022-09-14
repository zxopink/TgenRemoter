using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using TgenNetProtocol;
using TgenNetProtocol.WinForms;
using TgenSerializer;
using TgenRemoteCodes;

namespace TgenRemoter
{
    using static NetworkCodes;
    public partial class Menu : FormNetworkBehavour
    {
        ClientManager clientManager;
        string domain = "yoavhaik.com";
        string ip;
        const int port = 7777; //Main server port

        string myPass = string.Empty; //Will be filled later

        private void UpdateSettings()
        {
            try
            {
                string cfgFilePath = @".\settings.cfg"; //cfg == configurations, thanks Valve :)
                using (FileStream settingsFileStream = new FileStream(cfgFilePath, FileMode.Create))
                {
                    RemoteSettingsObj obj = new RemoteSettingsObj(AllowNetworkFiles, FolderPath);
                    Formatter.Serialize(settingsFileStream, obj, CompressionFormat.String);
                }
            }
            catch (Exception)
            {
                //Don't take risks, the saving may fail.
                //In such case it's best to lose the settings file than the whole software
            }
        }

        bool AllowNetworkFiles { get => RemoteSettings.CanSendFiles;  set { RemoteSettings.CanSendFiles = value; UpdateSettings(); } }
        string FolderPath { get => RemoteSettings.FolderPath; set { RemoteSettings.FolderPath = value; UpdateSettings(); } }
        public Menu()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TgenLog.Reset();
            FormClosed += Form1_FormClosed;

            LoadToolTips();

            //Change target ip name to the file's name if the name is an ip
            string exeName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
            IPAddress ipAddress = null;
            bool isValidIp = IPAddress.TryParse(exeName, out ipAddress);
            ip = Dns.GetHostAddresses(domain)[0].ToString();
            if (isValidIp)
                ip = exeName;

            clientManager = new ClientManager();
            //clientManager.OnDisconnect += ClientManager_OnDisconnect;
            clientManager.Connect(ip, port, "PINK");
            if (!clientManager.Connected)
            {
                MessageBox.Show("Could not connect to the main server!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            try
            {
                //CheckForSpace(); //Check if there's enough memory in the system for the program to function properly
            }
            catch (Exception)
            {
                //No access to drivers, not a big deal
            }
            

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //Don't take risks, the saving may fail. 
            //In such case it's best to lose the settings file than the whole software
            //Note: Remember they block many premissions at school
            try
            {
                string cfgFilePath = @".\settings.cfg"; //cfg == configurations, thanks Valve :)
                using (FileStream settingsFileStream = new FileStream(cfgFilePath, FileMode.OpenOrCreate))
                {
                    if (new FileInfo(cfgFilePath).Length == 0) //File empty, no save file
                    {
                        //Initiate default values
                        bool defaultCanSendFile = CheckFileTransformation.Checked;
                        string defaultFilesPath = KnownFolders.GetPath(KnownFolder.Downloads);

                        RemoteSettingsObj obj = new RemoteSettingsObj(defaultCanSendFile, defaultFilesPath);
                        Formatter.Serialize(settingsFileStream, obj, CompressionFormat.String);

                        RemoteSettings.CanSendFiles = defaultCanSendFile;
                        RemoteSettings.FolderPath = defaultFilesPath;
                    }
                    else
                    {
                        //Settings file has content, establish it
                        RemoteSettingsObj settings = (RemoteSettingsObj)Formatter.Deserialize(settingsFileStream, CompressionFormat.String);
                        if (!Directory.Exists(settings.FolderPath)) //Target doesn't exist
                        {
                            //Report and get default values
                            TgenLog.Log($"{settings.FolderPath} doesn't exist!");
                            string defaultFilesPath = KnownFolders.GetPath(KnownFolder.Downloads);
                            RemoteSettings.FolderPath = defaultFilesPath;

                            //Clean the settings file
                            settingsFileStream.Close();
                            File.WriteAllText(cfgFilePath, string.Empty);
                            FileStream settingsCleanStream = new FileStream(cfgFilePath, FileMode.OpenOrCreate);

                            //Write new settings file
                            RemoteSettingsObj obj = new RemoteSettingsObj(settings.CanSendFiles, defaultFilesPath);
                            Formatter.Serialize(settingsCleanStream, obj, CompressionFormat.String);
                        }
                        else //Is all good
                            RemoteSettings.FolderPath = settings.FolderPath;

                        RemoteSettings.CanSendFiles = settings.CanSendFiles;
                    }
                }
                CheckFileTransformation.Checked = RemoteSettings.CanSendFiles;
            }
            catch (Exception)
            {
                bool defaultCanSendFile = CheckFileTransformation.Checked;
                string defaultFilesPath = KnownFolders.GetPath(KnownFolder.Downloads);

                RemoteSettings.CanSendFiles = defaultCanSendFile;
                RemoteSettings.FolderPath = defaultFilesPath;
            }
        }

        private void ClientManager_OnDisconnect() =>
            MessageBox.Show("Lost connection to main server!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <exception cref="IOException">Thrown when an issue with the driver occurres</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when there's no access to the driver</exception>
        private void CheckForSpace()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var driver in drives)
            {
                long gigaByte = 1000000000;
                int requiredGigas = 1;
                if (driver.AvailableFreeSpace < gigaByte * requiredGigas)
                {
                    string message = $"{driver.Name} has less than {requiredGigas} free gigabytes of memory.\n\n" +
                        $"Such issue is crucial for the program and might result in unintentioned behaviour";
                    MessageBox.Show(message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LoadToolTips()
        {
            toolTip.SetToolTip(inConnCheckBox, "If checked, allows users to control this computer by entering your code");
            toolTip.SetToolTip(CopyPassBtn, "Copys your code to the clipboard (ctrl + c)");
            toolTip.SetToolTip(Connect, "Tries to connect another user by their code");
            toolTip.SetToolTip(PassLabel, "Your code");
            toolTip.SetToolTip(CheckFileTransformation, "If checked, allows partner to share files");
            toolTip.SetToolTip(FilePathBtn, "Folder where files sent by your partner will be dropped to");
            toolTip.SetToolTip(CreditsLabel, "That's me :D");
        }

        [ClientReceiver]
        public void GotEvent(PassCode codes)
        {
            string message = codes.passCode;

            if (myPass == string.Empty)
            {
                myPass = message;
                PassLabel.Text = "Code: " + myPass;
                return;
            }

            if (!Visible) return;
            Console.WriteLine(message);
            MessageBox.Show(message, "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [ClientReceiver]
        public void FoundPartner(Session session) =>
            StartSession(session.Mode, session.PartnerEP);

        [ClientReceiver]
        /// <summary>Called when server detects the partner is from this local network</summary>
        public void FoundPartner(LocalSession session)
        {
            var localIp = LiteNetLib.NetUtils.GetLocalIp(LiteNetLib.LocalAddrType.IPv4);
            IPEndPoint partnerEP = new IPEndPoint(IPAddress.Parse(localIp), session.PartnerPort);
            StartSession(session.Mode, partnerEP);
        }

        public void StartSession(Mode mode,IPEndPoint partnerEP)
        {
            NetTimer.Enabled = false;
            clientManager.Close();
            switch (mode)
            {
                case Mode.Controlled:
                    Controlled controlledForm = new Controlled(partnerEP);
                    controlledForm.Show();
                    break;
                case Mode.Controller:
                    Controller controllerForm = new Controller(partnerEP);
                    controllerForm.Show();
                    break;
            }
            Hide();
        }

        [ClientReceiver]
        public void GotPing(Ping ping) =>
            clientManager.Send(new Ping());

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            clientManager.Close();
            Application.Exit();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (partnerIp.Text == "")
                return;

            clientManager.Send(new PassCode(partnerIp.Text));
        }

        private void CheckFileTransformation_CheckedChanged(object sender, EventArgs e)
        {
            AllowNetworkFiles = CheckFileTransformation.Checked;
            FilePathBtn.Enabled = CheckFileTransformation.Checked;
        }

        private void ButtonFilePath_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog folderBrowser = new CommonOpenFileDialog())
            {
                Console.WriteLine(FolderPath);
                folderBrowser.Title = "Path For Network Files";
                folderBrowser.IsFolderPicker = true;
                folderBrowser.InitialDirectory = FolderPath;
                CommonFileDialogResult result = folderBrowser.ShowDialog();

                if (result == CommonFileDialogResult.Ok) // Test result.
                {
                    FolderPath = folderBrowser.FileName;
                }
            }
        }

        private void FilesInfoBtn_Click(object sender, EventArgs e)
        {
            string message = $"You can use the '{FilePathBtn.Text}' to choose where you want files that your partner sends " +
                $"to be located. \n\n" +
                $"If you don't want files to be transferred by the client simply uncheck " +
                $"{CheckFileTransformation.Text}. \n\n" +
                $"If your partner chose to enable file transformation, " +
                $"simply drop the file you want to send into the window and the file will be sent " +
                $"(If you partner enabled file transformation). \n\n" +
                $"Would you like to receive files from your partner?";

            DialogResult result = MessageBox.Show(message, "Note", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            switch (result)
            {
                case DialogResult.Yes:
                    CheckFileTransformation.Checked = true;
                    break;
                case DialogResult.No:
                    CheckFileTransformation.Checked = false;
                    break;
                default:
                    break;
            }
        }

        private void CopyPassBtn_Click(object sender, EventArgs e) => Clipboard.SetText(myPass);

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void NetTimer_Tick(object sender, EventArgs e) =>
            clientManager.PollEvents();
    }
}
