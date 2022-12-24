using System;
using System.Diagnostics;
using System.IO;

namespace ADBGUIToolbyEvrenater.CleanUnnecessaryFiles
{
    public static class DeleteFilesAndFolders
    {
        private static string workingDirectory = /*Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + */"platform-tools\\ADBGUITool\\";

        private static string[] folders;
        private static string[] files;
        public static void Delete()
        {
            if (Directory.Exists(workingDirectory))
            {
                folders = Directory.GetDirectories(workingDirectory);
                files = Directory.GetFiles(workingDirectory);
           

            try
            {
                foreach (string folder in folders)
                {
                    if (File.Exists(folder))
                    Directory.Delete(folder, true);
                }
                foreach (string file in files)
                {
                    if(File.Exists(file))
                    File.Delete(file);
                    Debug.WriteLine("Deleting " + file);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
