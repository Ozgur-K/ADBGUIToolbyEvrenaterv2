using ADBGUIToolbyEvrenater.ProcessCreating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADBGUIToolbyEvrenater.Screenshot
{
    class ScreenCapture
    {
        Form form1;
        TableLayoutPanel tableLayoutPanel;
        Label resultLabel;
        Button screenshotButton, cancelButton,openLocationButton, openScreenshotButton;
        BackgroundWorker backgroundWorker1;
        PictureBox pictureBox1;

        string imageFile;
        string screenshotLocation;

        public ScreenCapture()
        {
            form1 = new();
            tableLayoutPanel = new();
            resultLabel = new();
            screenshotButton = new();
            cancelButton = new();
            backgroundWorker1 = new();
            pictureBox1 = new();
            openLocationButton = new();
            openScreenshotButton= new();
            form1.SuspendLayout();
            screenshotLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            form1.Text = "Screenshot";
            resultLabel.Text = "Screenshot Capture";
            screenshotButton.Text = "Screenshot";
            cancelButton.Text = "Cancel";
            openLocationButton.Text = "Open Screenshot Location";
            openScreenshotButton.Text = "Open Screenshot with...";

            resultLabel.AutoSize = true;
            screenshotButton.AutoSize = true;
            cancelButton.AutoSize = true;
            openLocationButton.AutoSize = true;
            openScreenshotButton.AutoSize= true;
            cancelButton.Enabled = false;
            backgroundWorker1.WorkerSupportsCancellation = true;

            screenshotButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            screenshotButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            openScreenshotButton.AutoSizeMode= AutoSizeMode.GrowAndShrink;
            tableLayoutPanel.Width = form1.Width - 20;
            tableLayoutPanel.Height = form1.Height - 20;
            pictureBox1.Width = 100;
            pictureBox1.Height = 200;

            resultLabel.AutoSizeChanged += ResultLabel_AutoSizeChanged;
            resultLabel.SizeChanged += ResultLabel_SizeChanged;
            screenshotButton.Click += screenshotButton_Click;
            cancelButton.Click += CancelButton_Click;
            openLocationButton.Click += OpenButton_Click;
            openScreenshotButton.Click += OpenScreenshotButton_Click;
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            form1.Load += Form1_Load;
            form1.SizeChanged += Form1_SizeChanged;

            tableLayoutPanel.Controls.AddRange(new Control[] { resultLabel, screenshotButton, cancelButton, openLocationButton,
                                                    openScreenshotButton, pictureBox1});
            form1.Controls.Add(tableLayoutPanel);
            form1.ResumeLayout();
            form1.ShowDialog();

        }



        #region downloads folder path x(will be used documents folder)x
        public static string GetHomePath()
        {
            // Not in .NET 2.0
            // System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                return System.Environment.GetEnvironmentVariable("HOME");

            return System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }


        public static string GetDownloadFolderPath()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string pathDownload = System.IO.Path.Combine(GetHomePath(), "Downloads");
                return pathDownload;
            }

            return System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                     @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty
                )
            );
        }
        #endregion

        private void Screenshot()
        {
            screenshotButton.Text = "Capturing...";
            screenshotButton.Enabled = false;
            cancelButton.Enabled = true;

            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.

                backgroundWorker1.RunWorkerAsync();
            }
        }

        #region


        public void OpenFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = folderName;
                startInfo.FileName = "explorer.exe";

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderName));
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                resultLabel.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                resultLabel.Text = "Error: " + e.Error.Message;
            }
            else
            {
                // Done!
                screenshotButton.Text = "Screenshot";
                resultLabel.Text = "Screenshot Location:\r\n"
                                    + screenshotLocation
                                    + "\\screenshot<time>.png";
                screenshotButton.Enabled = true;
                cancelButton.Enabled = false;


                string[] lines = ProcessCreate.cmdOutput.Split(Environment.NewLine,
                                                                                StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    /*  if (((line.IndexOf('a') == 0) && (line.IndexOf('d') == 1)
                          && (line.IndexOf('b') == 2) && (line.IndexOf(':') == 3))
                          || (((line.IndexOf('E') == 0) || (line.IndexOf('e') == 0)) && (line.IndexOf('r') == 1)
                          && (line.IndexOf('o') == 3) && (line.IndexOf(':') == 5))
                          || line.Contains("no devices")) ;
                      {
                          resultLabel.Text = line;
                      }
                      else
                      {
                      }*/

                    if (lines.Length == 0)
                    {
                        // Success

                        screenshotButton.Text = "Screenshot";
                        screenshotButton.Enabled = true;
                        cancelButton.Enabled = false;
                        // tableLayoutPanel.BackgroundImage = Image.FromFile(imageFile);
                        Image image = Image.FromFile(imageFile);
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox1.Image = image;
                    }
                    else if (line.IndexOf('*') != 0)
                    {
                        // Failed
                        resultLabel.Text = line;
                        screenshotButton.Text = "Screenshot";
                        screenshotButton.Enabled = true;
                        cancelButton.Enabled = false;

                    }
                }
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                // Perform a time consuming operation and report progress.


                DateTime dateTime = new DateTime();
                dateTime = DateTime.Now;
                string date = dateTime.Hour + "-" + dateTime.Minute + "-" + dateTime.Second + "-" + dateTime.Millisecond;

                imageFile = screenshotLocation
                                        + "\\screenshot" + date + ".png";



                ProcessCreate.Command("adb exec-out screencap -p > " + imageFile);
                Debug.WriteLine("home download path: " + GetDownloadFolderPath());
                Debug.WriteLine("home download path: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            }
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = false;


            DialogResult dialogResult = DialogResult.Yes;
            dialogResult = MessageBox.Show(form1, "You may need to reconnect your device over Wi-Fi " +
                                "or physically reconnect over USB after cancellation. I suggest clicking no and" +
                                "disconnecting the phone. Do you want to cancel?",
                                "Terminate", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult.Equals(DialogResult.No))
            {
            }
            else
            {
                ProcessCreate.Command2("adb kill-server");

                form1.Dispose();
                form1.Close();
                if (backgroundWorker1.WorkerSupportsCancellation == true)
                {
                    // Cancel the asynchronous operation.
                    // 
                    backgroundWorker1.CancelAsync();
                }
            }
        }
        private void screenshotButton_Click(object sender, EventArgs e)
        {
            Screenshot();
        }

        private void OpenButton_Click(object? sender, EventArgs e)
        {
            OpenFolder(screenshotLocation);


            
        }

        private void OpenScreenshotButton_Click(object? sender, EventArgs e)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + imageFile;
            Process.Start("rundll32.exe", args);
        }


        private void ResultLabel_AutoSizeChanged(object sender, EventArgs e)
        {
            tableLayoutPanel.Width = resultLabel.Width + 20;
        }

        private void ResultLabel_SizeChanged(object sender, EventArgs e)
        {
            tableLayoutPanel.Width = resultLabel.Width + 20;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            tableLayoutPanel.Size = form1.Size;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tableLayoutPanel.Size = form1.Size;
            //Debug.WriteLine();
        }
        #endregion
    }
}
