using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using EncryptionWPF.Tools;
using System.Threading;
using System.Windows.Threading;

namespace EncryptionWPF
{
    /// <summary>
    /// Interaktionslogik für LogViewer.xaml
    /// </summary>
    public partial class LogViewer : Window
    {
        Assistant assistant = new Assistant();
        public LogViewer()
        {
            InitializeComponent();
            if (File.Exists(assistant.GetLogPath()))
            {
                TextBoxLog.Text = File.ReadAllText(assistant.GetLogPath());
            }
            assistant.WriteLog("LogViewer ist offen");
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("LogViewer geschlossen");
            this.Close();
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("Log im LogViewer neu geladen.");
            if (File.Exists(assistant.GetLogPath()))
            {
                TextBoxLog.Text = File.ReadAllText(assistant.GetLogPath());
            }
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Log-Datei|*.log|Textdatei|*.txt|Alle Dateien|*.*";
                saveFileDialog.FileName = "SEF";
                saveFileDialog.DefaultExt = ".log";
                if (saveFileDialog.ShowDialog() == true)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                    sw.Write("Export vom: " + DateTime.Now + Environment.NewLine + "User: " + Environment.UserName + Environment.NewLine + Environment.NewLine + TextBoxLog.Text);
                    sw.Close();
                    assistant.WriteLog("Log exportiert");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    "Log konnte nicht exportiert werden!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler beim Exportieren: " + err.Message);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            assistant.DeleteLog();
        }

        private async void CheckLoadLog_Checked(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("Log wird automatisch ausgelesen!");
            TextBoxLog.ScrollToEnd();
            while (CheckLoadLog.IsChecked == true)
            {
                if (File.Exists(assistant.GetLogPath()))
                {
                    TextBoxLog.Text = File.ReadAllText(assistant.GetLogPath());
                }
                else
                {
                    TextBoxLog.Text = "Keine Log-Datei gefunden...";
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }

        private void TextBoxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxLog.ScrollToEnd();
        }
    }
}
