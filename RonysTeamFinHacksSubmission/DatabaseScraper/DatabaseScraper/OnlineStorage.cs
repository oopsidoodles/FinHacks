using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using com.shephertz.app42.paas.sdk.csharp.upload;
using SimpleJSON;

namespace DatabaseScraper
{
    static class OnlineStorage
    {
        //I heard that you're supposed to keep stuff like secret keys encrypted and/or stored away somewhere, never hardcoded or else people can decompile it, oh well
        private const string KEY_API = "28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9";
        private const string KEY_SECRET = "11b1c14582e5df3a0cab8fb290b8b8f1c79855bed83121cd7e1e7a6a8566d7d0";

        public const string FILE_EXTENSION = ".txt";

        //This is the service used to upload and download files, the name is just a bit deceiving
        private static UploadService uploadService;



        //Pre: N/A
        //Post: OnlineStorage class is initialized
        //Description: This method initializes the online storage class.
        public static void Initialize()
        {
            App42API.Initialize(KEY_API, KEY_SECRET);

            ServiceAPI api = new ServiceAPI(KEY_API, KEY_SECRET);
            uploadService = api.BuildUploadService();
        }

        //Pre: File name
        //Post: Bool for whether file exists returned
        //Description: This method checks whether the specified file exists on the storage.
        public static bool CheckFileExists(string fileName)
        {
            bool exists = true;
            //The library throws an exception if the file does not exist, so I try to get the file and if an exception is thrown it doesn't exist
            try
            {
                Upload fileList = uploadService.GetFileByName(fileName);

            }
            catch (App42NotFoundException exp)
            {
                exists = false;
                Console.WriteLine(exp.Message);
            }
            return exists;
        }

        //Pre: Name of file, contents of file
        //Post: File uploaded to storage
        //Description: This method uploads to the online storage.
        public static void Upload(string fileName, string contents)
        {
            File.WriteAllText(fileName + FILE_EXTENSION, contents);
            uploadService.UploadFile(fileName, fileName + FILE_EXTENSION, UploadFileType.TXT, "file");
            File.Delete(fileName + FILE_EXTENSION);
        }

        public static void Overwrite(string filename, string contents)
        {
            /*if (CheckFileExists(filename))
            {
                DeleteFile(filename);
            }
            Upload(filename, contents);*/
        }

        //Pre: File name
        //Post: Contents of file returned
        //Description: This method returns the contents of the specified file name
        public static string GetFile(string fileName)
        {
            string contents = "";
            Upload file = uploadService.GetFileByName(fileName);
            IList<Upload.File> fileData = file.GetFileList();

            //The library returns the URL of the file, so just use WebClient class to download as a string
            using (WebClient client = new WebClient())
            {
                contents = client.DownloadString(fileData[0].GetUrl());
            }

            return contents;
        }

        //Pre: File name
        //Post: File is deleted
        //Description: This method deletes a file on the cloud.
        public static void DeleteFile(string fileName)
        {
            uploadService.RemoveFileByName(fileName);
        }
    }
}
