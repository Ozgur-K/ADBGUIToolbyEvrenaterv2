using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADBGUIToolbyEvrenater.Disclaimers
{
    class Disclaimer
    {
        Form form1;
        FlowLayoutPanel flowLayoutPanel;
        Label disclaimerLabel;
        RichTextBox richTextBox;
        Button linkButton;

        public Disclaimer()
        {
            form1 = new();
            flowLayoutPanel = new();
            disclaimerLabel = new();
            richTextBox = new();
            linkButton = new();

            form1.SuspendLayout();

            form1.Text = "Disclaimer";
            disclaimerLabel.Text = "Disclaimer";
            richTextBox.Text = "No Responsibility\r\n\r\nUse At Your Own Risk\r\n\r\nevrenater@gmail.com";
            linkButton.Text = "Github";

            richTextBox.ReadOnly = true;
            linkButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            flowLayoutPanel.Size = form1.Size;
            richTextBox.Width = form1.Width - 20;

            form1.SizeChanged += Form1_SizeChanged;
            linkButton.Click += LinkButton_Click;

            form1.ResumeLayout();
            flowLayoutPanel.Controls.AddRange(new Control[] { disclaimerLabel, richTextBox, linkButton});
            form1.Controls.Add(flowLayoutPanel);
            form1.ShowDialog();
        }

        private void LinkButton_Click(object? sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Ozgur-K/ADBGUIToolbyEvrenaterv2",
                UseShellExecute = true
            });
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            flowLayoutPanel.Size = form1.Size;
            richTextBox.Width = form1.Width - 20;
        }
    }
}
