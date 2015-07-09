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
using System.Diagnostics;
using System.Net;

namespace File_Downloader_Manager
{
    public partial class File_Downloader_Manager : Form
    {
        public File_Downloader_Manager()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        string file;
        //http://fs41.filehippo.com/7489/c8d4bcf7a0f54cc08070e08f16d48aba/fdminst.exe
        private void downloadbtn_Click(object sender, EventArgs e)
        {
            file = richTextBox1.Text;
            backgroundWorker1.RunWorkerAsync();
        }

        private void exitbtn_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy.Equals(true))
            {
                backgroundWorker1.CancelAsync();
            }
            Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo fileinfo = new FileInfo((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\"));
            HttpWebRequest wreq =(HttpWebRequest) HttpWebRequest.Create(file);
            HttpWebResponse wres = (HttpWebResponse)wreq.GetResponse();
            Debug.WriteLine(wres.ResponseUri);
            Debug.WriteLine(Path.GetFileName(wres.ResponseUri.AbsolutePath));
            Debug.WriteLine(wres.ContentLength);
            string filename = Path.GetFileName(wres.ResponseUri.AbsolutePath);
            Debug.WriteLine(Path.GetExtension(filename));
            Stream str = wres.GetResponseStream();
            FileStream fs = new FileStream(fileinfo + filename, FileMode.Append, FileAccess.Write);
            byte[] buffer = new byte[1024];
            int bytesize = 0;
            Int64 test = 0;
            while ((bytesize = str.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (backgroundWorker1.CancellationPending.Equals(true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    fs.Write(buffer, 0, bytesize);
                    test += bytesize;
                    double index = (double)test;
                    double total = (double)wres.ContentLength;
                    double percentage = (index / total);
                    int per = (int)(percentage * 100);
                    backgroundWorker1.ReportProgress(per);
                }
            }
            fs.Close();
            str.Close();
            wres.Close();
            wreq = null;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("File download complete");
        }

        private void pausebtn_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy.Equals(true))
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void stopbtn_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy.Equals(true))
            {
                backgroundWorker1.CancelAsync();
            }
        }
    }
}
