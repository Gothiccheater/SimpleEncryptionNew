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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EncryptionWPF.Tools;
using EncryptionWPF.Encryption;
using System.IO;
using Microsoft.Win32;

namespace EncryptionWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Variablen
        LogViewer logViewer;
        Assistant assistant = new Assistant();
        MyAes aes = new MyAes();
        private string iv = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs"))
            {
                Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs");
                assistant.WriteLog("Log Verzeichnis erstellt!");
            }
            assistant.WriteLog("Programm gestartet!");
            UpdateLogText();
        }

        private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPW.Text))
            {
                MessageBox.Show(
                    "Passwort darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Passwort leer!");
            }
            else if (string.IsNullOrWhiteSpace(textBoxIN.Text))
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Eingabe leer!");
            }
            else
            {
                try
                {
                    if (iv == null)
                    {
                        iv = assistant.RandomGen(16);
                        assistant.WriteLog("IV erstellt!");
                    }
                    assistant.SetPW(textBoxPW.Text);
                    assistant.SetIV(iv);
                    aes.SetIV(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Encrypt(textBoxIN.Text);
                    assistant.WriteLog("Text verschlüsselt!");
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                        "Kann nicht Verschlüsseln!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    assistant.WriteLog("Fehler: Verschlüsseln fehlgeschlagen! " + err.Message);
                }
                iv = null;
            }
        }

        private void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPW.Text))
            {
                MessageBox.Show(
                    "Passwort darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Passwort leer!");
            }
            else if (string.IsNullOrWhiteSpace(textBoxIN.Text))
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Eingabe leer!");
            }
            else
            {
                try
                {
                    assistant.SetPW(textBoxPW.Text);
                    assistant.SetIV(iv);
                    aes.SetIV(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Decrypt(textBoxIN.Text);
                    assistant.WriteLog("Text entschlüsselt!");
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                        "Falsches Passwort!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    assistant.WriteLog("Fehler: Entschlüsseln fehlgeschlagen! " + err.Message);
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxOUT.Text))
            {
                MessageBox.Show(
                    "Bitte zuerst einen Text verschlüsseln!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Ausgabe leer!");
            }
            else
            {
                try
                {
                    if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves"))
                    {
                        Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves");
                        assistant.WriteLog("Saves Ordner erstellt!");
                    }
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "SimpleEncryptionFile|*.sef|Alle Dateien|*.*";
                    saveFileDialog.FileName = "Text";
                    saveFileDialog.DefaultExt = ".sfd";
                    saveFileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                        sw.Write(textBoxOUT.Text + assistant.GetIV());
                        sw.Close();
                        assistant.WriteLog("Datei gespeichert!");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                    "Konnte Datei nicht speichern!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    assistant.WriteLog("Fehler: Speichern fehlgeschlagen! " + err.Message);
                }
            }
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SimpleEncryptionFile|*.sef|Alle Dateien|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    content = File.ReadAllText(openFileDialog.FileName);
                    textBoxIN.Text = content.Substring(0, content.Length - 16);
                    iv = content.Substring(content.Length - 16);
                    assistant.WriteLog("Datei geladen!");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    "Konnte Datei nicht lesen!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Laden fehlgeschlagen! " + err.Message);
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            textBoxPW.Text = "12345678";
            iv = null;
            textBoxIN.Clear();
            textBoxOUT.Clear();
            assistant.WriteLog("Felder resettet!");
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("Programm geschlossen!");
            if (CheckBoxDelLog.IsChecked == true)
            {
                assistant.DeleteLog();
            }
            if (logViewer == null)
            {
                this.Close();
            }
            else
            {
                logViewer.Close();
                this.Close();
            }
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("Informationen über das Programm eingesehen");
            MessageBox.Show(
                "Erstellt von Tobias Nies." + Environment.NewLine
                + "Dieses Programm nutzt die AES-256 Verschlüsselung, welche auch als Military Standard bekannt ist." + Environment.NewLine
                + "Version 1.1",
                "SimpleEncryption",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ButtonGen_Click(object sender, RoutedEventArgs e)
        {
            textBoxPW.Text = assistant.RandomGen(12);
            assistant.WriteLog("Passwort generiert!");
        }

        private void ButtonShowLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                assistant.WriteLog("LogViewer wird geöffnet!");
                logViewer = new LogViewer();
                logViewer.Show();
            }
            catch (Exception err)
            {
                assistant.WriteLog("Konnte LogViewer nicht öffnen! " + err.Message);
                MessageBox.Show(
                    "Fehler beim Öffnen des LogViewers!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void buttonLoadOther_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Textdatei|*.txt|Alle Dateien|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    content = File.ReadAllText(openFileDialog.FileName);
                    textBoxIN.Text = content;
                    assistant.WriteLog("Textdatei geladen!");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    "Konnte Datei nicht lesen!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                assistant.WriteLog("Fehler: Laden fehlgeschlagen! " + err.Message);
            }
        }
        private async void UpdateLogText()
        {
            while (true)
            {
                if (File.Exists(assistant.GetLogPath()))
                {
                    textBoxLastLog.Text = File.ReadLines(assistant.GetLogPath()).Last();
                }
                else
                {
                    textBoxLastLog.Text = "Keine Log-Datei gefunden...";
                }
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }
    }
}