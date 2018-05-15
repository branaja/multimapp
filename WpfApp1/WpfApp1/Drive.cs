using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace WpfApp1
{
    public static class Drive
    {
        private static double TotalSize;

        public static DriveService AuthenticateOauth()
        {
            string[] scopes = new string[] {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile};

            var clientId = "993083871396-v5o1gahg26mlia9u7s51d1scj999ed6d.apps.googleusercontent.com";      // From https://console.developers.google.com

            //Environment.UserName; ide pod authorizeAsync
            var clientSecret = "aO4aIEZcyRtGccKCeFsVPPsW";          // From https://console.developers.google.com
                                                                    // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            scopes, "multim1718@estudent.hr", CancellationToken.None, new FileDataStore("MyAppsToken")).Result;
            //Once consent is recieved, your token will be stored locally on the AppData directory, so that next time you wont be prompted for consent. 

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MultimApp",
                ApiKey = "AIzaSyA32OUipN54fjDcL6igEguBwBykPxSByDk"
            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(60);
            //Long Operations like file uploads might timeout. 100 is just precautionary value, can be set to any reasonable value depending on what you use your service for.
            return service;
        }

        public static DriveService AuthenticateServiceAccount(string serviceAccountEmail, string serviceAccountCredentialFilePath)
        {
            try
            {
                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                string[] scopes = new string[] {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile};

                GoogleCredential credential;
                using (var stream = new FileStream(serviceAccountCredentialFilePath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                         .CreateScoped(scopes);
                }

                // Create the  Analytics service.
                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "MultimApp",
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Create service account DriveService failed" + ex.Message);
                throw new Exception("CreateServiceAccountDriveFailed", ex);
            }
        }


        public static string uploadFile(DriveService _service, string _uploadFile, string _parent, string _descrp = "Automatski generirani fajl.")
        {
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = System.IO.Path.GetFileName(_uploadFile);
            body.Description = _descrp;
            body.MimeType = GetMimeType(_uploadFile);

            var stream = new System.IO.FileStream(_uploadFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            TotalSize = stream.Length;

            FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, GetMimeType(_uploadFile));

            request.ProgressChanged += UploadProgessEvent;
            request.ChunkSize = FilesResource.InsertMediaUpload.MinimumChunkSize; // Minimum ChunkSize allowed by Google is 256*1024 bytes. ie 256KB.
            var test = request.Upload();

            if (test.Exception == null) return null;
            else return test.Exception.Message;
        }

        private static void UploadProgessEvent(IUploadProgress obj)
        {
            var test = (obj.BytesSent / TotalSize).ToString() + "%";
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
