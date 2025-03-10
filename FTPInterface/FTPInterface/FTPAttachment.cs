 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FTPInterface
{
    public class FTPAttachment
    {

        private static string FTPUserName = "CHM_USER";
        private static string FTPPassWord = "AttACHeMenT5&@BBMF@TIIvsDNR";
        private static string FtpBaseUrl = "ftp://196.191.244.132:21/";


        private static bool AttachementISRoom { get; set; }
        private static string RoomTypeCode { get; set; }
        public static string ORGUnitDefcode { get; set; }
        private static string CompanyTINNo { get; set; }
        private static string ftpfilelocation { get; set; }
        private static string ftpPMSReportlocation { get; set; }
        private static string ftpCompanyDirectory { get; set; }
        private static string ftpBranchDirectory { get; set; }
        private static string ftpImageTypeDirectory { get; set; }
        private static string ftpRommsDirectory { get; set; }
        private static string ftpGuestDirectory { get; set; }
        private static string ftpTransactionDirectory { get; set; }
        private static string ftpTransactionDefinitionDirectory { get; set; }

        private static string FtpComanayProfileName = "/CompanyProfile/"; 
        private static string FtpGslProfileName = "/GslProfile/";
        private static string FtpTransactionName = "/Transaction/";
        private static string FtpRommsName = "/Rooms/";
        private static string FtpPMSReport = "/Report/PMS Report";

        public static bool InitalizeFTPAttachment(string TINno)
        {
            //return false;
            CompanyTINNo = TINno;
            ftpCompanyDirectory = String.Format("{0}{1}", FtpBaseUrl, CompanyTINNo);
            string ftpCompanyTransactionDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpTransactionName);
            string ftpCompanyGSLDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpGslProfileName);
            string ftpCompanyprofileDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpComanayProfileName);
            if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
            {
                CreateFTPDirectory(ftpCompanyDirectory);
            }

            if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
            {
                MessageBox.Show("The FTP With " + CompanyTINNo + " Tin Number Don't have FTP Access Please Contact System Admin.");
                return false;
            }

            if (!CheckFTPDirectoryExist(ftpCompanyTransactionDirectory))
            {
                CreateFTPDirectory(ftpCompanyTransactionDirectory);
            }
            if (!CheckFTPDirectoryExist(ftpCompanyGSLDirectory))
            {
                CreateFTPDirectory(ftpCompanyGSLDirectory);
            }
            if (!CheckFTPDirectoryExist(ftpCompanyprofileDirectory))
            {
                CreateFTPDirectory(ftpCompanyprofileDirectory);
            }

            return true;
        }
        public static bool InitalizeFTPAttachment(string TINno, string OrganizationUnitDefCode, bool IsRoom = false, string RoomType = "")
        {
           // return false;
            CompanyTINNo = TINno;
            ORGUnitDefcode = OrganizationUnitDefCode;
            AttachementISRoom = IsRoom;
            RoomTypeCode = RoomType;
            ftpCompanyDirectory = String.Format("{0}{1}", FtpBaseUrl, CompanyTINNo);
            string ftpCompanyTransactionDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpTransactionName);
            string ftpCompanyGSLDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpGslProfileName);
            string ftpCompanyprofileDirectory = String.Format("{0}{1}", ftpCompanyDirectory, FtpComanayProfileName);

            if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
            {
                CreateFTPDirectory(ftpCompanyDirectory);
            }

            if (!CheckFTPDirectoryExist(ftpCompanyDirectory))
            {
                MessageBox.Show("The FTP With " + CompanyTINNo + " Tin Number Don't have FTP Access Please Contact System Admin.");
                return false;
            }
            if (!CheckFTPDirectoryExist(ftpCompanyTransactionDirectory))
            {
                CreateFTPDirectory(ftpCompanyTransactionDirectory);
            }
            if (!CheckFTPDirectoryExist(ftpCompanyGSLDirectory))
            {
                CreateFTPDirectory(ftpCompanyGSLDirectory);
            }
            if (!CheckFTPDirectoryExist(ftpCompanyprofileDirectory))
            {
                CreateFTPDirectory(ftpCompanyprofileDirectory);
            }

            return true;
        }


   

        public static string SendTransactionAttachement(int DefintionId, string FileLocation, int voucherid)
        {
            // string ImageName = Path.GetFileName(ImageLocation);
            FileInfo fi = new FileInfo(FileLocation);
            ftpTransactionDirectory = String.Format("{0}{1}{2}{3}/", FtpBaseUrl, CompanyTINNo, FtpTransactionName, ORGUnitDefcode);
            ftpTransactionDefinitionDirectory = String.Format("{0}{1}{2}{3}/{4}", FtpBaseUrl, CompanyTINNo, FtpTransactionName, ORGUnitDefcode, DefintionId);
            ftpfilelocation = String.Format("{0}{1}{2}{3}/{4}/{5}", FtpBaseUrl, CompanyTINNo, FtpTransactionName, ORGUnitDefcode, DefintionId, fi.Name);

            if (CreateFTPDirectory(ftpTransactionDirectory) && CreateFTPDirectory(ftpTransactionDefinitionDirectory) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, FileLocation);
            }

            return ftpfilelocation;
        }
        public static string SendConsigneePMSAttachement(int GslType, string Guestcode, string FileLocation)
        {
            // string ImageName = Path.GetFileName(ImageLocation);
            FileInfo fi = new FileInfo(FileLocation);
            ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType);
            ftpfilelocation = String.Format("{0}{1}{2}/{3}{4}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType, Guestcode, fi.Extension);
            if (CreateFTPDirectory(ftpGuestDirectory) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, FileLocation);
            }
            return ftpfilelocation;
        }

        public static string SendGSlFileAttachement(int GSLId, string FileLocation, int voucherid)
        {
            // string ImageName = Path.GetFileName(ImageLocation);
            FileInfo fi = new FileInfo(FileLocation);
            ftpTransactionDirectory = String.Format("{0}{1}{2}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName);
            ftpTransactionDefinitionDirectory = String.Format("{0}{1}{2}{3}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName, GSLId);
            ftpfilelocation = String.Format("{0}{1}{2}{3}/{4}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName, GSLId, fi.Name);

            if (CreateFTPDirectory(ftpTransactionDirectory) && CreateFTPDirectory(ftpTransactionDefinitionDirectory) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, FileLocation);
            }

            return ftpfilelocation;
        }

        public static string SendConsigneeImageAttachement(int GslType, string Guestcode, System.Drawing.Image Image)
        {
            ImageFormat format = ImageFormat.Png;
            MemoryStream ms = new MemoryStream();
            Image.Save(ms, ImageFormat.Png);

            // string ImageName = Path.GetFileName(ImageLocation); 
            ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType);
            ftpfilelocation = String.Format("{0}{1}{2}/{3}{4}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType, Guestcode, Guestcode+ ".jpg" );
            if (CreateFTPDirectory(ftpGuestDirectory) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, ms);
            }
            return ftpfilelocation;
        }
        public static string SendArticleImageAttachement(int GslType, string Guestcode, System.Drawing.Image Image)
        {
            ImageFormat format = ImageFormat.Png;
            MemoryStream ms = new MemoryStream();
            Image.Save(ms, ImageFormat.Png);

            // string ImageName = Path.GetFileName(ImageLocation); 
            ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType);
            ftpfilelocation = String.Format("{0}{1}{2}/{3}{4}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName + GslType, Guestcode, Guestcode + ".jpg");
            if (CreateFTPDirectory(ftpGuestDirectory) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, ms);
            }
            return ftpfilelocation;
        }


        public static string SendPMSReportAttachement(string FileLocation, DateTime Date)
        {
            // string ImageName = Path.GetFileName(ImageLocation);
            FileInfo fi = new FileInfo(FileLocation);
            //ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpPMSReport);
          string  ftpPMSReportlocation1 = String.Format("{0}{1}{2}/{3}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode);
            ftpPMSReportlocation = String.Format("{0}{1}{2}/{3}/{4:dd-MM-yyyy}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date);
            ftpfilelocation = String.Format("{0}{1}{2}/{3}//{4:dd-MM-yyyy}/{5}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date, fi.Name);
            if (CreateFTPDirectory(ftpPMSReportlocation1) && CreateFTPDirectory(ftpPMSReportlocation)  && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, FileLocation);
            }
            return ftpfilelocation;
        }
        public static string SendPMSReportAttachementStream(MemoryStream stream,string ReportName ,DateTime Date)
        {
            if (stream == null)
                return null;
            //ftpGuestDirectory = String.Format("{0}{1}{2}/", FtpBaseUrl, CompanyTINNo, FtpPMSReport);
            string ftpPMSReportlocation1 = String.Format("{0}{1}{2}/{3}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode);
            ftpPMSReportlocation = String.Format("{0}{1}{2}/{3}/{4:dd-MM-yyyy}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date);
            ftpfilelocation = String.Format("{0}{1}{2}/{3}//{4:dd-MM-yyyy}/{5}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date, ReportName+".pdf");
            if (CreateFTPDirectory(ftpPMSReportlocation1) && CreateFTPDirectory(ftpPMSReportlocation) && FTPFileDontExist(ftpfilelocation))
            {
                UpLoadFile(ftpfilelocation, stream);
            }
            return ftpfilelocation;
        }


        public static System.Drawing.Image GetPMSGuestImageFromDirectory(string Guestcode)
        {
            ftpGuestDirectory = String.Format("{0}{1}{2}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName);
            List<string> ImageList = GetDirectoryListFiles(ftpGuestDirectory);
            if (ImageList != null)
            {
                string ImageName = ImageList.FirstOrDefault(x => x.Contains(Guestcode));
                if (ImageName != null)
                {
                    ftpfilelocation = String.Format("{0}{1}{2}/{3}", FtpBaseUrl, CompanyTINNo, FtpGslProfileName, ImageName);
                    if (!FTPFileDontExist(ftpfilelocation))
                        return   DownloadImageFromFTP(ftpfilelocation);
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
                //request.UsePassive = false;
                //request.Timeout = 30000;
                //request.ReadWriteTimeout = 30000;

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

       

        //public string ftpfilelocation = "ftp://196.191.244.133/0000000001/CompanyProfile/OUD0000050/banner/Test2.jpg";
        //public string ftpBranchDirectory = "ftp://196.191.244.133/0000000001/CompanyProfile/banner/OUD0000050/";
        //public string ftpImageTypeDirectory = "ftp://196.191.244.133/0000000001/CompanyProfile/OUD0000050/banner";

        public static string SendFTPFile(string ImageType, string FileLocation)
        {
            string FileName = Path.GetFileName(FileLocation);

            if (!AttachementISRoom)
            {
                ftpBranchDirectory = String.Format("{0}{1}{2}{3}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode);
                ftpImageTypeDirectory = String.Format("{0}{1}{2}{3}/{4}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, ImageType);
                ftpfilelocation = String.Format("{0}{1}{2}{3}/{4}/{5}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, ImageType, FileName);

                if (CreateFTPDirectory(ftpBranchDirectory) && CreateFTPDirectory(ftpImageTypeDirectory) && FTPFileDontExist(ftpfilelocation))
                {
                    UpLoadFile(ftpfilelocation, FileLocation);
                }
            }
            else
            {
                ftpBranchDirectory = String.Format("{0}{1}{2}{3}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode);
                ftpRommsDirectory = String.Format("{0}{1}{2}{3}{4}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName);
                ftpImageTypeDirectory = String.Format("{0}{1}{2}{3}{4}{5}/", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName, RoomTypeCode);
                ftpfilelocation = String.Format("{0}{1}{2}{3}{4}{5}/{6}", FtpBaseUrl, CompanyTINNo, FtpComanayProfileName, ORGUnitDefcode, FtpRommsName, RoomTypeCode, FileName);

                if (CreateFTPDirectory(ftpBranchDirectory) && CreateFTPDirectory(ftpRommsDirectory) && CreateFTPDirectory(ftpImageTypeDirectory) && FTPFileDontExist(ftpfilelocation))
                {
                    UpLoadFile(ftpfilelocation, FileLocation);
                }
            }
            return ftpfilelocation;
        }

        public static System.Drawing.Image GetImageFromFTP(string FTPImageLocation)
        {
            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadImageFromFTP(FTPImageLocation);
            else
                return null;
        }

        public static System.Drawing.Image DownloadImageFromFTP(string ftpFilePath)
        {
            try
            {

                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
               // downloadRequest.UsePassive = false;
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = 30000;
                downloadRequest.ReadWriteTimeout = 30000;
                //  downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                //  downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;

                using (Stream ftpStream = downloadRequest.GetResponse().GetResponseStream())
                {
                    return System.Drawing.Image.FromStream(ftpStream);
                }
            }
            catch
            {
                return null;
            }
        }  public static MemoryStream GetFileStreamFromFTP(string vouchercode,int definition)
        {

            string FTPImageLocation = String.Format("{0}{1}{2}/{3}/{4}/{5}", FtpBaseUrl, CompanyTINNo, FtpTransactionName, ORGUnitDefcode, definition, vouchercode + ".pdf");

            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadFileFromFTP(FTPImageLocation);
            else
                return null;
        }
        public static MemoryStream GetFileStreamFromFTP(string FTPImageLocation)
        { 
            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadFileFromFTP(FTPImageLocation);
            else
                return null;
        }
        public static MemoryStream GetFileStreamFromFTP(string ReportName, DateTime Date)
        {
           
            string FTPImageLocation = String.Format("{0}{1}{2}/{3}/{4:dd-MM-yyyy}/{5}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date, ReportName +".pdf");

            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadFileFromFTP(FTPImageLocation);
            else
                return null;
        }

        public static byte[] GetByteFileStreamFromFTP(string ReportName, DateTime Date)
        {

            string FTPImageLocation = String.Format("{0}{1}{2}/{3}/{4:dd-MM-yyyy}/{5}", FtpBaseUrl, CompanyTINNo, FtpPMSReport, ORGUnitDefcode, Date, ReportName + ".pdf");

            if (!FTPFileDontExist(FTPImageLocation))
                return DownloadByteFileFromFTP(FTPImageLocation);
            else
                return null;
        }
        public static MemoryStream DownloadFileFromFTP(string ftpFilePath)
        {
            try
            {

                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                //downloadRequest.UsePassive = false;
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;
                //downloadRequest.Timeout = 30000;
                //downloadRequest.ReadWriteTimeout = 30000;

                Stream stream = downloadRequest.GetResponse().GetResponseStream();
                MemoryStream mStream = new MemoryStream();
                stream.CopyTo(mStream);

                return mStream;
            }
            catch
            {
                return null;
            }

        }

        public static byte[] DownloadByteFileFromFTP(string ftpFilePath)
        {
            try
            {

                byte[] buffer = new byte[10240];
                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;
                //downloadRequest.UsePassive = false;
                //downloadRequest.Timeout = 30000;
                //downloadRequest.ReadWriteTimeout = 30000;

                using (Stream stream = downloadRequest.GetResponse().GetResponseStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // process the chunk in "buffer"
                    }
                }
                return buffer;

            }
            catch
            {
                return null;
            }

        }
        public static void UpLoadFile(string ftpfile, MemoryStream stream)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpfile);
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                //req.UsePassive = false;
                //req.Timeout = 30000;
                //req.ReadWriteTimeout = 30000;

                byte[] fileData = stream.GetBuffer();

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

        public static void UpLoadFile(string ftpfile, string ImageLocation)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpfile);
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                //req.UsePassive = false;
                //req.Timeout = 30000;
                //req.ReadWriteTimeout = 30000;

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
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpfile);
                request.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
               // request.UsePassive = false;
                //request.Timeout = 30000; 
                //request.ReadWriteTimeout = 30000;

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

            }
            catch
            {
                return false;
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
                //requestDir.UsePassive = false;
                requestDir.UseBinary = true;
                //requestDir.KeepAlive = false;
                //requestDir.Timeout = 30000;
                //requestDir.ReadWriteTimeout = 30000;

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
                downloadRequest.UseBinary = true;
                downloadRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                downloadRequest.Credentials = new NetworkCredential(FTPUserName, FTPPassWord);
                downloadRequest.Timeout = System.Threading.Timeout.Infinite;
                downloadRequest.ReadWriteTimeout = System.Threading.Timeout.Infinite;
               // downloadRequest.UsePassive = false;
                //downloadRequest.Timeout = 30000;
                //downloadRequest.ReadWriteTimeout = 30000;

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
                //request.UsePassive = false; 
                //request.Timeout = 30000;
                //request.ReadWriteTimeout = 30000;
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
