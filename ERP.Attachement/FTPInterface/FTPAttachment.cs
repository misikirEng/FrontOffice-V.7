using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Attachement
{
   public class FTPAttachment
    {

        private static string FTPUserName = "CHM_USER";
       private static string FTPPassWord = "AttACHeMenT5&@BBMF@TIIvsDNR";
       private static string FtpBaseUrl = "ftp://196.191.244.133/";

        
       private static bool AttachementISRoom { get; set; }
       private static string RoomTypeCode { get; set; }
       private static string ORGUnitDefcode { get; set; }
       private static string CompanyTINNo { get; set; }
       private static string ftpfilelocation { get; set; }
       private static string ftpCompanyDirectory { get; set; }
       private static string ftpBranchDirectory { get; set; }
       private static string ftpImageTypeDirectory { get; set; }
       private static string ftpRommsDirectory { get; set; }
       private static string ftpGuestDirectory { get; set; }

       private static string FtpComanayProfileName = "/CompanyProfile/";
       private static string FtpGslProfileName = "/GslProfile/17";
       private static string FtpRommsName = "/Rooms/";
       public static bool InitalizePMSFTPAttachment(string TINno)
       {
           CompanyTINNo = TINno;
           ftpCompanyDirectory = String.Format("{0}{1}", FtpBaseUrl, CompanyTINNo);
           if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
           {
               return false;
           }
           return true;
       }
       public static string SendPMSAttachement(string Guestcode, string ImageLocation)
       {
           // string ImageName = Path.GetFileName(ImageLocation);
           FileInfo fi = new FileInfo(ImageLocation);
           ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpGslProfileName);
           ftpfilelocation = String.Format("{0}{1}{2}/{3}{4}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName, Guestcode, fi.Extension);
           if (CreateFTPDirectory(ftpGuestDirectory) && FTPFileDontExist(ftpfilelocation))
           {
               UpLoadImage(ftpfilelocation, ImageLocation);
           }
           return ftpfilelocation;
       }
       public static Image GetPMSGuestImageFromDirectory(string Guestcode)
       {
           ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpGslProfileName);
           List<string> ImageList = GetDirectoryListFiles(ftpGuestDirectory);
           if (ImageList != null)
           {
               string ImageName = ImageList.FirstOrDefault(x => x.Contains(Guestcode + "."));
               if (ImageName != null)
               {
                   ftpfilelocation = String.Format("{0}{1}{2}/{3}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName, ImageName);
                   if (!FTPFileDontExist(ftpfilelocation))
                       return DownloadImageFromFTP(ftpfilelocation);
                   else
                       return null;
               }
           }
           return null;
       }
       private static List<string> GetDirectoryListFiles(string ftpFolderlocation)
       {
           try
           {
               FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFolderlocation);
               request.Method = WebRequestMethods.Ftp.ListDirectory;

               request.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
               FtpWebResponse response = (FtpWebResponse)request.GetResponse();
               Stream responseStream = response.GetResponseStream();
               StreamReader reader = new StreamReader(responseStream);
               string names = reader.ReadToEnd();

               reader.Close();
               response.Close();

               return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
           }
           catch (Exception)
           {
               return null;
           }
       }  

       public static bool InitalizeFTPAttachment(string TINno, string OrganizationUnitDefCode,bool IsRoom, string RoomType )
       {
           CompanyTINNo = TINno;
           ORGUnitDefcode = OrganizationUnitDefCode;
           AttachementISRoom = IsRoom;
           RoomTypeCode = RoomType;
           ftpCompanyDirectory = String.Format("{0}{1}", FtpBaseUrl, CompanyTINNo);
           if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
           {
               XtraMessageBox.Show("The FTP With " + CompanyTINNo + " Tin Number Don't have FTP Access Please Contact System Admin.");
               return false;
           }
           return true;
       }

        //public string ftpfilelocation = "ftp://196.191.244.133/0000000001/CompanyProfile/OUD0000050/banner/Test2.jpg";
        //public string ftpBranchDirectory = "ftp://196.191.244.133/0000000001/CompanyProfile/banner/OUD0000050/";
        //public string ftpImageTypeDirectory = "ftp://196.191.244.133/0000000001/CompanyProfile/OUD0000050/banner";

       public static string SendFTPImage(string ImageType, string ImageLocation)
       {
           string ImageName = Path.GetFileName(ImageLocation);

           if (!AttachementISRoom)
           {
               ftpBranchDirectory = String.Format("{0}{1}{2}{3}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode);
               ftpImageTypeDirectory = String.Format("{0}{1}{2}{3}/{4}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, ImageType);
               ftpfilelocation = String.Format("{0}{1}{2}{3}/{4}/{5}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, ImageType, ImageName);


               if (CreateFTPDirectory(ftpBranchDirectory) && CreateFTPDirectory(ftpImageTypeDirectory) && FTPFileDontExist(ftpfilelocation))
               {
                   UpLoadImage(ftpfilelocation, ImageLocation);
               }

           }
           else
           {
               ftpBranchDirectory = String.Format("{0}{1}{2}{3}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode);
               ftpRommsDirectory = String.Format("{0}{1}{2}{3}{4}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName);
               ftpImageTypeDirectory = String.Format("{0}{1}{2}{3}{4}{5}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName, RoomTypeCode);
               ftpfilelocation = String.Format("{0}{1}{2}{3}{4}{5}/{6}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName, RoomTypeCode, ImageName);


               if (CreateFTPDirectory(ftpBranchDirectory) && CreateFTPDirectory(ftpRommsDirectory) && CreateFTPDirectory(ftpImageTypeDirectory) && FTPFileDontExist(ftpfilelocation))
               {
                   UpLoadImage(ftpfilelocation, ImageLocation);
               }

           }

           return ftpfilelocation;
       }



        public static Image GetImageFromFTP(string FTPImageLocation)
        {
            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadImageFromFTP(FTPImageLocation);
            else
                return null;
        }
        public static Image DownloadImageFromFTP(string ftpFilePath)
        {
            try
            {

                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                downloadRequest.UsePassive = true;
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;

                using (Stream ftpStream = downloadRequest.GetResponse().GetResponseStream())
                {
                    return Image.FromStream(ftpStream);
                }
            }
            catch
            {
                return null;
            }


        }

        public static void UpLoadImage(string ftpfile, string ImageLocation)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpfile);
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);

                byte[] fileData = File.ReadAllBytes(ImageLocation);

                req.ContentLength = fileData.Length;
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(fileData, 0, fileData.Length);
                reqStream.Close();
            }
            catch (WebException io)
            {

                FtpWebResponse response = (FtpWebResponse)io.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                  
                }
                else
                {
                    response.Close();
                 
                }

            }

        }

        public static bool FTPFileDontExist(string ftpfile)
        {
            var request = (FtpWebRequest)WebRequest.Create(ftpfile);
            request.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
            return false;
        }

        private static bool CreateFTPDirectory(string directory)
        {

            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }

        public static bool DeleteImageFromFTP(string ftpFilePath)
        {
            try
            {

                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                downloadRequest.UsePassive = true;
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;

                FtpWebResponse response = (FtpWebResponse)downloadRequest.GetResponse();
                response.Close();
            }
            catch (WebException ex)
            {

                return false;

            }
            return true;
        }

        private static bool CheckFTPDirectoryExist(string directory)
        {
            if (String.IsNullOrEmpty(directory))
                throw new ArgumentException("No directory was specified to check for");

            // Ensure directory is ended with / to avoid false positives
            if (!directory.EndsWith("/"))
                directory += "/";

            try
            {
                var request = (FtpWebRequest)WebRequest.Create(directory);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);

                using (request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }

        }
    }
}
