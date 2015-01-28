using System;
using Ocean.Core.Plugins.Upload;
using Ocean.Core.Configuration;
namespace Ocean.Core.Plugins.FTP
{
    public interface IFTP
    {
        long BytesTotal { get; }
        bool ChangeDir(string path);
        bool Connect();
        void Connect(string server, int port, string user, string pass);
        void Connect(string server, string user, string pass);
        void Disconnect();
        long DoDownload();
        long DoUpload();
        long FileSize { get; }
        DateTime GetFileDate(string fileName);
        string GetFileDateRaw(string fileName);
        long GetFileSize(string filename);
        string GetWorkingDirectory();
        bool IsConnected { get; }
        System.Collections.ArrayList List();
        System.Collections.ArrayList ListDirectories();
        System.Collections.ArrayList ListFiles();
        void MakeDir(string dir);
        string Messages { get; }
        string ErrorMessage { get; }
        bool MessagesAvailable { get; }
        void OpenDownload(string filename);
        void OpenDownload(string filename, bool resume);
        void OpenDownload(string remote_filename, string local_filename, bool resume);
        void OpenDownload(string remote_filename, string localfilename);
        bool OpenUpload(string filename);
        bool OpenUpload(string filename, bool resume);
        bool OpenUpload(string filename, string remote_filename, bool resume);
        bool OpenUpload(string filename, string remotefilename);
        bool PassiveMode { get; set; }
        void RemoveDir(string dir);
        void RemoveFile(string filename);
        void RenameFile(string oldfilename, string newfilename);
        string ResponseString { get; }
        IConfigInfo FtpConfig { get; set; }
    }
}
