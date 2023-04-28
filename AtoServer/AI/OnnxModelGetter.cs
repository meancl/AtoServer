using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AtoServer.AI
{
    public class OnnxModelGetter
    {
        string _ftpID = "ftp_user";
        string _ftpPassword = "jin9409";
        string _ftpServer = "221.149.119.60";
        string _port = "2021";
        string _onnx_path = AtoServer.AI.OnnxPath.onnx_path;
        string _serverUri;

        public OnnxModelGetter()
        {
            _serverUri = $"ftp://{_ftpServer}:{_port}/onnx";
        }

        public void DownloadOnnxModels(string[] sModelNames)
        {
            for (int i =0; i< sModelNames.Length; i++)
            {
                using(var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_ftpID, _ftpPassword);
                    client.DownloadFile($"{_serverUri}/{sModelNames[i]}", $"{_onnx_path}{sModelNames[i]}");
                }
            }
        }
    }
}
