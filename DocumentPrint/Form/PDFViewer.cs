using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DocumentPrint.Forms
{
    public partial class PDFViewer : DevExpress.XtraEditors.XtraForm
    {
        public PDFViewer()
        {
            InitializeComponent();
        }
        private MemoryStream _filestream;
        public MemoryStream filestream
        {
            get { return _filestream; }
            set { _filestream = value; }
        }
        private string _filePath;
        public string filePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        private string _consignee;
        public string Consignee
        {
            get { return _consignee; }
            set { _consignee = value; }
        }
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        public bool OpenFile()
        {
            bool status = false;
            if (filestream == null)
            {
                if (string.IsNullOrEmpty(filePath))
                    return status;

                if (!File.Exists(filePath))
                    return status;

                DocViewer.LoadDocument(filePath);

            }
            else
                DocViewer.LoadDocument(filestream);


            status = true;
            return status;

        }

        private void bbiClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DocViewer.CloseDocument();
            this.Close();
        }

        private void bsiEmail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //FileStream fs = new FileStream(filePath, FileMode.Open,FileAccess.Read,FileShare.ReadWrite);

            //using (MemoryStream ms = new MemoryStream()) 
            //{
            //    fs.CopyTo(ms);
            //    List<string> extension = new List<string>();
            //    extension.Add(".pdf");
            //    List<Byte[]> Stream = new List<byte[]>();
            //    Stream.Add(ms.ToArray());
            //    List<string> fileName = new List<string>();
            //    fileName.Add("Registration Document");
            //    CNETMail.CNET_Mail mail = new CNETMail.CNET_Mail(LoginPage.Authentication.GetAuthorizedUser().person, Consignee, Stream, fileName, extension);
            //    mail.ShowDialog();
            //}
        }
    }
}
