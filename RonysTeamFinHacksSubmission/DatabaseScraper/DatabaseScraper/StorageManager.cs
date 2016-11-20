using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.upload;
using System.IO;
using System.Net;

namespace Shooter_From_The_Sky
{
    class OnlineStorageHelper
    {
        //Variables for uploading files online
        private UploadService uploadService;
        private Upload upload;
        private App42Response response;


        public OnlineStorageHelper()
        {
        }


        public void Initialize()
        {
            //Initilizes API for uploading file
            App42API.Initialize("60e208ac266ee0cd030aac00255c1309934c0808ef0e6497c897e22a012daf89", "6b5fe1007f7dddc4ef873bacfca597751572592aa3d72d628c2b461199d43fda");
            uploadService = App42API.BuildUploadService();
        }

        
        public bool UpdateFile(string fileName, string filePath, string fileType, string description)
        {
            //Boolean for whether the file was updated or not on the server
            bool success = false;

            //Tries to update the file
            try
            {
                //If the file was removed
                if (RemoveOldFile(fileName))
                {
                    //Upload new one
                    upload = uploadService.UploadFile(fileName, filePath, fileType, description);
                    success = true;
                }
                //If the file was not removed
                else
                {
                    //The success of updating the file is set to false
                    success = false;
                }
            }
            //If there are any exceptions
            catch
            {
                //The success of updating the file is set to false
                success = false;
            }

            //Returns whether the file was updated
            return success;
        }


        public bool RemoveOldFile(string fileName)
        {
            //Boolean for whether the old file was removed
            bool success;

            //Tries to remove the file
            try
            {
                //Tries to remove the file
                response = uploadService.RemoveFileByName(fileName);

                //Stores whether the file was removed or not
                success = response.IsResponseSuccess();
            }
            //If there are any exceptions
            catch (Exception ex)
            {
                //If the exception message contains the text in the brackets
                if (ex.Message.Contains("The file with the name '" + fileName + "' does not exist."))
                {
                    //Set the file to removed because it never existed in the first place
                    success = true;
                }
                //If the exception message was anything else
                else
                {
                    //Sets the file to not removed
                    success = false;
                }
            }

            //Returns whether the file was removed
            return success;
        }


        public bool DownloadFile(string filePath, string fileName)
        {
            //If a downloaded version of the file currently exists 
            if (File.Exists(filePath))
            {
                //Deletes the downloaded version of the file
                File.Delete(filePath);
            }

            //Tries to download the file
            try
            {
                //A client is created to download the file using a URL
                using (var client = new WebClient())
                {
                    //Every file with the same name of the file that is being looked for is retrieved
                    upload = uploadService.GetFileByName(fileName);
                    IList<Upload.File> fileList = upload.GetFileList();

                    //Loop for every file in the file list
                    for (int i = 0; i < fileList.Count(); i++)
                    {
                        //The current file is downloaded using it's url and the name it is stored as
                        client.DownloadFile(fileList[i].GetUrl(), fileName);
                    }
                }

                //Returns that the file was downloaded
                return true;
            }
            //If there are any exceptions
            catch
            {
                //Returns that the file was not downloaded
                return false;
            }
        }


        public bool CheckFileExists(string fileName)
        {
            //Every file saved on the server is retrieved from the server (not downloaded)
            Upload tempUpload = uploadService.GetAllFiles();
            IList<Upload.File> fileList = tempUpload.GetFileList();

            //Loop for every file in the file list
            for (int i = 0; i < fileList.Count; i++)
            {
                //If the file that is being looked for exists on teh server
                if (fileList[i].GetName() == fileName)
                {
                    //Return that the file exists
                    return true;
                }
            }

            //Return that the file does not exist
            return false;
        }
    }
}
