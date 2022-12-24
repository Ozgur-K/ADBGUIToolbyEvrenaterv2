﻿using ADBGUIToolbyEvrenater.ProcessCreating;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace ADBGUIToolbyEvrenater.ApkInstalling
{
    class ApkInstaller
    {
        Form apkInstallerWindow;
        TableLayoutPanel tableLayoutPanel;
        CheckBox grantCB, replaceCB, installsdCB;
        Button installButton, cancelButton;
        Label resultLabel;
        BackgroundWorker backgroundWorker1;

        string[] dragDropFile;
        string[] CBOptions;

        public ApkInstaller()
        {
            backgroundWorker1 = new();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

            CreateWindow();
        }

        public void CreateWindow()
        {
            apkInstallerWindow = new Form();

            tableLayoutPanel = new TableLayoutPanel();

            grantCB = new CheckBox();
            replaceCB = new CheckBox();
            installsdCB = new CheckBox();

            installButton = new Button();
            cancelButton = new Button();

            resultLabel = new Label();


            apkInstallerWindow.Text = "Apk Installer";
            grantCB.Text = "Grant All Runtime Permission";
            replaceCB.Text = "Replace Existing Application";
            installsdCB.Text = "Try Installing to External Storage";
            installButton.Text = "Install";
            resultLabel.Text = "Drag the APK File(s) to This Window";
            cancelButton.Text = "Cancel";

            grantCB.AutoSize = true;
            replaceCB.AutoSize = true;
            installsdCB.AutoSize = true;
            tableLayoutPanel.AllowDrop = true;
            apkInstallerWindow.AllowDrop = true;

            installButton.AutoSize = true;
            installButton.Enabled = false;
            cancelButton.Enabled = false;
            resultLabel.AutoSize = true;
            cancelButton.AutoSize = true;
            cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;


            tableLayoutPanel.Size = apkInstallerWindow.Size;

            tableLayoutPanel.DragEnter += new DragEventHandler(apkInstallerWindow_DragEnter);
            tableLayoutPanel.DragDrop += new DragEventHandler(apkInstallerWindow_DragDrop);
            installButton.Click += new EventHandler(installButton_Click);
            resultLabel.Layout += DragHereLabel_Layout;
            apkInstallerWindow.FormClosed += ApkInstallerWindow_FormClosed;
            cancelButton.Click += CancelButton_Click;
            apkInstallerWindow.DragEnter += ApkInstallerWindow_DragEnter;

            // startAsyncButton.Click += startAsyncButton_Click;
            // cancelButton.Click += cancelAsyncButton_Click;


            tableLayoutPanel.Controls.Add(grantCB);
            tableLayoutPanel.Controls.Add(replaceCB);
            tableLayoutPanel.Controls.Add(installsdCB);
            tableLayoutPanel.Controls.Add(installButton);
            tableLayoutPanel.Controls.Add(resultLabel);
            tableLayoutPanel.Controls.Add(cancelButton);

            apkInstallerWindow.Controls.Add(tableLayoutPanel);

            apkInstallerWindow.ShowDialog();
        }

        private void ApkInstallerWindow_DragEnter(object sender,
System.Windows.Forms.DragEventArgs e)
        {
            //MessageBox.Show("entered");
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Yes;
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                Debug.WriteLine("Cancel Button Clicked");
                // Cancel the asynchronous operation.
                dialogResult = MessageBox.Show(apkInstallerWindow, "You have to reconnect your device over Wi-Fi " +
                                    "or physically reconnect over USB after cancellation. Do you want to cancel?",
                                    "Terminate", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);


                backgroundWorker1.CancelAsync();
            }
            if (dialogResult.Equals(DialogResult.No))
            {
            }
            else
            {
                ProcessCreate.Command2("adb kill-server");
            }
        }

        private void SendComand(object sender, DoWorkEventArgs e, string CBOption0, string CBOption1, string CBOption2, string[] dragDropFile)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                // Perform a time consuming operation and report progress.
                foreach (string file in dragDropFile)
                    ProcessCreate.Command("adb install " + CBOptions[0] + CBOptions[1] + CBOptions[2] + " \"" + file + "\" ");
            }
        }

        public void GetFile(string[] files)
        {
            string temp = null; foreach (string file in files) temp = file;

            if (temp.Contains(".apk"))
            {
                installButton.Enabled = true;
                cancelButton.Enabled = false;
                installButton.Text = "Install";
                resultLabel.Text = null;
                foreach (string file in files)
                {
                    dragDropFile = files;

                    resultLabel.Text += file.Substring((file.LastIndexOf("\\") + 1),
                        ((file.LastIndexOf('.') - 1) - (file.LastIndexOf("\\")))) + "\n";
                }
            }
            else
            {
                apkInstallerWindow.TopMost = false;
                MessageBox.Show("APK File(s) Required", "ADB GUI Tool");
                installButton.Enabled = false;
                cancelButton.Enabled = false;
                resultLabel.Text = "Drag the APK File(s) to This Window";
            }

        }

        public void InstallApk()
        {
            resultLabel.Text = "";

            CBOptions = CheckBoxControl();

            foreach (string file in dragDropFile)
            {
                installButton.Text = "Installing...";
                installButton.Enabled = false;
                cancelButton.Enabled = true;


                // Command send
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    //resultLabel.Text = "Please Wait...";
                    backgroundWorker1.RunWorkerAsync();
                }

                if (!backgroundWorker1.IsBusy)
                {


                }
            }

        }

        public string[] CheckBoxControl()
        {
            string[] CBOptions = new string[3];
            if (grantCB.Checked)
            {
                CBOptions[0] = " -g ";
            }
            else
            {
                CBOptions[0] = " ";
            }


            if (replaceCB.Checked)
            {
                CBOptions[1] = " -r ";
            }
            else
            {
                CBOptions[1] = " ";
            }


            if (installsdCB.Checked)
            {
                CBOptions[2] = " -s ";
            }
            else
            {
                CBOptions[2] = " ";
            }
            return CBOptions;
        }

        public void installButton_Click(object sender, EventArgs e)
        {
            InstallApk();
        }

        private void apkInstallerWindow_DragEnter(object sender, DragEventArgs e)
        {
           // MessageBox.Show("entered");
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void apkInstallerWindow_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            GetFile(files);
        }

        private void DragHereLabel_Layout(object sender, LayoutEventArgs e)
        {
            if (!(resultLabel.Width < 300))
                tableLayoutPanel.Width = resultLabel.Width + 50;
        }

        private void startAsyncButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void cancelAsyncButton_Click(object sender, EventArgs e)
        {

        }

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SendComand(sender, e, CBOptions[0], CBOptions[1], CBOptions[2], dragDropFile);
        }
        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                // Change statuslabel begin
                if (ProcessCreate.cmdOutput.Contains("Success"))
                {
                    resultLabel.Text += "Installing Successfull\n";
                    installButton.Text = "Installed";
                    installButton.Enabled = false;
                    cancelButton.Enabled = false;
                }
                else if (ProcessCreate.cmdOutput.Contains(" no devices/"))
                {
                    resultLabel.Text += "No Device Found";
                    cancelButton.Enabled = false;
                    installButton.Text = "Install";

                }
                else
                {
                    // print error
                    resultLabel.Text += ProcessCreate.cmdOutput;
                    installButton.Text = "Install";
                    installButton.Enabled = false;
                    cancelButton.Enabled = false;
                }
                // Change statuslabel end


                if (String.IsNullOrEmpty(ProcessCreate.cmdOutput))
                {
                    //resultLabel.Text = "Done!";
                }
                else
                {
                    // resultLabel.Text = "Done, Isn't NullOrEmpty.";// ProcessCreate.cmdOutput;
                }
            }
        }
        private void ApkInstallerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /* private void ApkInstallerWindow_FormClosing(object sender, FormClosingEventArgs e)
         {
             DialogResult dialogResult = DialogResult.Yes;
             if (backgroundWorker1.WorkerSupportsCancellation == true)
             {
                 Debug.WriteLine("Cancel Button Clicked");
                 // Cancel the asynchronous operation.
                 dialogResult = MessageBox.Show(apkInstallerWindow, "You have to reconnect your device over Wi-Fi " +
                                     "or physically reconnect over USB after cancellation. Do you want to cancel?",
                                     "Abort Installing", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);


                 backgroundWorker1.CancelAsync();
             }
             if (dialogResult.Equals(DialogResult.No))
             {
                 e.Cancel = true;
             }
             else
             {
                 e.Cancel = false;

                 Debug.WriteLine("canceltrue");
                 ProcessCreate.Command2("adb kill-server");
             }
         }
  */
    }
}