using ADBGUIToolbyEvrenater.ProcessCreating;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ADBGUIToolbyEvrenater.DeviceDetecting
{
    public static class DetectDevice
    {
        static Form detectDeviceForm, detectWireless11Form;
        static TableLayoutPanel flowLayoutPanel, flowLayoutPanel2;
        static Label connectStatusLabel, connectStatusLabel2;
        static TextBox addressTextBox, addressTextBox2, pairCodeTextBox;
        static Button connectNetworkButton, connectUSBButton, connectNetworkButton2;
        static CheckBox androidVersionCheckBox;

        public static string detectDeviceButtonText = "Detect Device";


        public static void ShowDetectForm()
        {
            detectDeviceForm = new Form();
            flowLayoutPanel = new TableLayoutPanel();
            connectStatusLabel = new Label();
            addressTextBox = new TextBox();
            connectNetworkButton = new Button();
            connectUSBButton = new Button();
            androidVersionCheckBox = new CheckBox();

            detectDeviceForm.SuspendLayout();

            detectDeviceForm.Text = "Detect Device";
            connectStatusLabel.Text = "Enter Address for Wireless Connecting:";
            addressTextBox.Text = "192.168.1.x:5555";
            addressTextBox.PlaceholderText = "192.168.1.x:5555";
            connectNetworkButton.Text = "Connect Over Network";
            connectUSBButton.Text = "Connect Over USB";
            androidVersionCheckBox.Text = "Android Version is 11+";

            connectStatusLabel.AutoSize = true;
            connectNetworkButton.AutoSize = true;
            connectNetworkButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            connectUSBButton.AutoSize = true;
            connectUSBButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            androidVersionCheckBox.AutoSize = true;

            detectDeviceForm.Layout += DetectDeviceForm_Layout;
            connectStatusLabel.Layout += WirelessStatus_Layout;
            connectNetworkButton.Click += ConnectNetworkButton_Click;
            connectUSBButton.Click += ConnectUSBButton_Click;
            addressTextBox.KeyDown += AddressTextBox_KeyDown;

            flowLayoutPanel.Controls.AddRange(new Control[] { connectStatusLabel, addressTextBox, androidVersionCheckBox,
                                                                connectNetworkButton, connectUSBButton});
            detectDeviceForm.Controls.Add(flowLayoutPanel);

            detectDeviceForm.ResumeLayout();

            detectDeviceForm.ShowDialog();
        }

        static void ConnectAndGetDeviceModelOverUSB()
        {
            try
            {
                connectUSBButton.Text = "Connecting...";
                connectUSBButton.Enabled = false;
                connectNetworkButton.Enabled = false;
                detectDeviceForm.Refresh();

                ProcessCreate.Command("adb kill-server");
                Debug.WriteLine("1" + ProcessCreate.cmdOutput);
                // System.Threading.Thread.Sleep(500);
                ProcessCreate.Command("adb usb");
                Debug.WriteLine("2" + ProcessCreate.cmdOutput);
                System.Threading.Thread.Sleep(1500);

                ProcessCreate.Command("adb shell getprop ro.product.model");
                if (ProcessCreate.cmdOutput.Contains("unauthorized"))
                {
                    ProcessCreate.Command("adb reconnect offline");
                    connectStatusLabel.Text = "Allow USB Debugging On Your Phone";
                    connectUSBButton.Text = "Connect Over USB";
                    detectDeviceButtonText = "Detect Device";
                    connectUSBButton.Enabled = true;
                    connectNetworkButton.Enabled = true;
                    detectDeviceForm.Refresh();
                }
                else if (ProcessCreate.cmdOutput.Contains("no devices/emulators")
                            || ProcessCreate.cmdOutput.Contains("device offline"))
                {
                    connectStatusLabel.Text = "No Devices Found";
                    connectUSBButton.Text = "Connect Over USB";
                    detectDeviceButtonText = "Detect Device";
                    connectUSBButton.Enabled = true;
                    connectNetworkButton.Enabled = true;
                    detectDeviceForm.Refresh();
                }
                else
                {
                    ProcessCreate.Command("adb devices -l");

                    if (ProcessCreate.cmdOutput.Contains("device product:"))
                    {
                        connectStatusLabel.Text = "Connected";
                        connectUSBButton.Text = "Connected";
                        connectUSBButton.Enabled = false;
                        connectNetworkButton.Enabled = false;
                        detectDeviceForm.Refresh();

                        ProcessCreate.Command("adb shell getprop ro.product.model");
                        Debug.WriteLine("connected-model:" + ProcessCreate.cmdOutput + "OverUSB");
                        string tempModelName = null;
                        if (ProcessCreate.cmdOutput.Contains("more than one device"))
                        {
                            tempModelName = "Model Name";
                        }
                        else
                        {
                            tempModelName = ProcessCreate.cmdOutput.Trim();
                        }
                        ProcessCreate.Command("adb root");
                        if (ProcessCreate.cmdOutput.Contains("already running") || ProcessCreate.cmdOutput.Contains("restarting"))
                            tempModelName += " - Root";
                        detectDeviceButtonText = tempModelName + " - Over USB ";

                        detectDeviceForm.Close();
                    }
                    else
                    {
                        connectStatusLabel.Text = ProcessCreate.cmdOutput;
                        connectUSBButton.Text = "Connect Over USB";
                        detectDeviceButtonText = "Detect Device";
                        connectUSBButton.Enabled = true;
                        connectNetworkButton.Enabled = true;
                        detectDeviceForm.Refresh();
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        static void ConnectAndGetDeviceModelOverNetwork()
        {
            connectNetworkButton.Text = "Connecting Over Network...";
            connectUSBButton.Enabled = false;
            connectNetworkButton.Enabled = false;
            detectDeviceForm.Refresh();

            ProcessCreate.Command("adb connect " + addressTextBox.Text);
            if (ProcessCreate.cmdOutput.Contains("authenticate"))
            {
                connectStatusLabel.Text = "Allow USB Debugging On Your Phone";
                ProcessCreate.Command("adb reconnect offline");
                connectNetworkButton.Text = "Connect Over Network";
                detectDeviceButtonText = "Detect Device";
                connectUSBButton.Enabled = true;
                connectNetworkButton.Enabled = true;
                detectDeviceForm.Refresh();
            }
            else if (ProcessCreate.cmdOutput.Contains("cannot connect to"))
            {
                connectStatusLabel.Text = "No Devices Found";
                connectNetworkButton.Text = "Connect Over Network";
                detectDeviceButtonText = "Detect Device";
                connectUSBButton.Enabled = true;
                connectNetworkButton.Enabled = true;
                detectDeviceForm.Refresh();
            }
            else if (ProcessCreate.cmdOutput.Contains("cannot resolve host"))
            {
                connectStatusLabel.Text = "Cannot resolve host - The address should be like 192.168.1.20:5555";
                connectNetworkButton.Text = "Connect Over Network";
                detectDeviceButtonText = "Detect Device";
                connectUSBButton.Enabled = true;
                connectNetworkButton.Enabled = true;
                detectDeviceForm.Refresh();
            }
            else if (ProcessCreate.cmdOutput.Contains("connected to "))
            {
                ProcessCreate.Command("adb devices");
                if (ProcessCreate.cmdOutput.Contains("unauthorized"))
                {
                    connectStatusLabel.Text = "Allow USB Debugging On Your Phone";
                    ProcessCreate.Command("adb reconnect offline");
                    connectNetworkButton.Text = "Connect Over Network";
                    detectDeviceButtonText = "Detect Device";
                    connectUSBButton.Enabled = true;
                    connectNetworkButton.Enabled = true;
                    detectDeviceForm.Refresh();
                }
                else
                {
                    connectStatusLabel.Text = "Connected";
                    connectNetworkButton.Text = "Connected";
                    connectUSBButton.Enabled = false;
                    connectNetworkButton.Enabled = false;
                    detectDeviceForm.Refresh();

                    ProcessCreate.Command("adb shell getprop ro.product.model");
                    //more than one device wxeprion in modle name
                    Debug.WriteLine("connected-model:" + ProcessCreate.cmdOutput + "OverNetwoork");
                    string tempModelName = null;
                    if (ProcessCreate.cmdOutput.Contains("more than one device"))
                    {
                        tempModelName = "Model Name";
                    }
                    else
                    {
                        tempModelName = ProcessCreate.cmdOutput.Trim();
                    }
                    ProcessCreate.Command("adb root");
                    if (ProcessCreate.cmdOutput.Contains("already running") || ProcessCreate.cmdOutput.Contains("restarting"))
                        tempModelName += " - Root";
                    detectDeviceButtonText = tempModelName + " - Over Network ";

                    detectDeviceForm.Close();

                }
            }
            else
            {
                connectStatusLabel.Text = ProcessCreate.cmdOutput;
                connectNetworkButton.Text = "Connect Over Network";
                detectDeviceButtonText = "Detect Device";
                connectUSBButton.Enabled = true;
                connectNetworkButton.Enabled = true;
                detectDeviceForm.Refresh();
            }
        }

        static void ConnectAndGetDeviceModelOverNetwork11()
        {
            detectWireless11Form = new();
            flowLayoutPanel2 = new();
            connectStatusLabel2 = new();
            addressTextBox2 = new();
            pairCodeTextBox = new();
            connectNetworkButton2 = new();

            detectWireless11Form.SuspendLayout();

            detectWireless11Form.Text = "Detect Device 11+";
            connectStatusLabel2.Text = "Open 'Connect With Pairing Code' Screen:";
            addressTextBox2.Text = "192.168.1.21:41359";
            addressTextBox2.PlaceholderText = "192.168.1.21:41359";
            pairCodeTextBox.PlaceholderText = "869754";
            connectNetworkButton2.Text = "Pair Over Network";

            connectStatusLabel2.AutoSize = true;
            connectNetworkButton2.AutoSize = true;
            connectNetworkButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            detectWireless11Form.Layout += DetectWireless11Form_Layout; 
            connectStatusLabel2.Layout += ConnectStatusLabel2_Layout;
            connectNetworkButton2.Click += ConnectNetworkButton2_Click;
            addressTextBox2.KeyDown += AddressTextBox2_KeyDown;
            pairCodeTextBox.KeyDown += PairCodeTextBox_KeyDown;

            flowLayoutPanel2.Controls.AddRange(new Control[] { connectStatusLabel2, addressTextBox2, pairCodeTextBox,
                                                                connectNetworkButton2});
            detectWireless11Form.Controls.Add(flowLayoutPanel2);

            detectWireless11Form.ResumeLayout();

            detectWireless11Form.ShowDialog();
        }



        #region Events

        private static void PairCodeTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.Enter))
                connectNetworkButton2.PerformClick();
        }
        private static void AddressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.Enter))
                connectNetworkButton.PerformClick();
        }

        private static void ConnectUSBButton_Click(object sender, EventArgs e)
        {
            ConnectAndGetDeviceModelOverUSB();
        }

        private static void ConnectNetworkButton_Click(object sender, EventArgs e)
        {
            if (androidVersionCheckBox.Checked)
            {
                ConnectAndGetDeviceModelOverNetwork11();
            }
            else
            {
                ConnectAndGetDeviceModelOverNetwork();
            }
        }

        private static void WirelessStatus_Layout(object sender, LayoutEventArgs e)
        {
            if (!(connectStatusLabel.Width < 300))
                detectDeviceForm.Width = connectStatusLabel.Width + 50;
        }

        private static void DetectDeviceForm_Layout(object sender, LayoutEventArgs e)
        {
            Debug.WriteLine("resized");
            flowLayoutPanel.Size = detectDeviceForm.Size;
        }
        private static void DetectWireless11Form_Layout(object? sender, LayoutEventArgs e)
        {
            flowLayoutPanel2.Size = detectWireless11Form.Size;
        }

        private static void AddressTextBox2_KeyDown(object? sender, KeyEventArgs e)
        {

        }

        private static void ConnectNetworkButton2_Click(object? sender, EventArgs e)
        {
            //MessageBox.Show("adb pair " + addressTextBox2.Text + " " + pairCodeTextBox.Text);
            ProcessCreate.Command("adb pair " + addressTextBox2.Text + " " + pairCodeTextBox.Text);
            connectNetworkButton2.Text = "Pairing...";
            connectNetworkButton2.Enabled = false;
            if (ProcessCreate.cmdOutput.Contains("Successfully"))
            {
                connectStatusLabel2.Text = "Paired. You can connect now.";
                connectNetworkButton2.Text = "Paired";
                detectWireless11Form.Refresh();
                detectWireless11Form.Close();
                androidVersionCheckBox.Checked = false;
                androidVersionCheckBox.Text = " Paired. You can connect with IP Address & Port.";
                androidVersionCheckBox.Enabled = false;
            }
            else
            {
                connectStatusLabel2.Text = ProcessCreate.cmdOutput;
                connectNetworkButton2.Text = "Pair Over Network";
                connectNetworkButton2.Enabled = true;
                detectDeviceForm.Refresh();
            }
        }

        private static void ConnectStatusLabel2_Layout(object? sender, LayoutEventArgs e)
        {
            flowLayoutPanel2.Size = detectWireless11Form.Size;
        }
        #endregion
    }
}
