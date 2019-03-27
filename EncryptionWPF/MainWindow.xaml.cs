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
            assistant.CreateDirectories();
            assistant.WriteLog("Programm gestartet!");
            UpdateLogText();
            IsUserOwner();
        }

        private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            iv = null;
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
                    assistant.CreateSalt();
                    assistant.SetIV(iv);
                    assistant.SetPW(textBoxPW.Text);
                    aes.SetIV(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Encrypt(textBoxIN.Text);
                    if (CheckBoxAddIV.IsChecked == true)
                    {
                        textBoxOUT.Text += assistant.GetIV();
                    }
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
                    assistant.SetIV(iv);
                    assistant.SetPW(textBoxPW.Text);
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
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "SimpleEncryptionFile|*.sef|Alle Dateien|*.*";
                    saveFileDialog.FileName = "Text";
                    saveFileDialog.DefaultExt = ".sfd";
                    saveFileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\UserData";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                        sw.Write(textBoxOUT.Text + Environment.NewLine + "IV: " + assistant.GetIV() + Environment.NewLine + "Salt: " + assistant.GetSalt());
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
                openFileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\UserData";
                if (openFileDialog.ShowDialog() == true)
                {
                    content = File.ReadAllText(openFileDialog.FileName);
                    textBoxIN.Text = content.Substring(0, content.LastIndexOf("IV: "));
                    iv = content.Substring(content.LastIndexOf("IV: ") + 4, 16);
                    assistant.SetSalt(content.Substring(content.LastIndexOf("Salt: ") + 6));
                    assistant.WriteLog("Datei geladen!");
                    if (CheckBoxAddIV.IsChecked == true)
                    {
                        textBoxIN.Text += iv;
                    }
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
            logViewer = null;
            textBoxIN.Clear();
            textBoxOUT.Clear();
            assistant.ResetValues();
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
                + "Dieses Programm nutzt die AES-256 Verschlüsselung." + Environment.NewLine
                + "Version 1.2",
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

        private void ButtonLoadOther_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Textdatei|*.txt|Alle Dateien|*.*";
                openFileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Documents\\";
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
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
        }

        private void CheckBoxAddIV_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult boxResult = MessageBox.Show(
                 "Diese Funktion sollte nur benutzt werden," +
                 " wenn ein Text mehrfach verschlüsselt werden soll" +
                 " und ist aktuell noch experimentell." + Environment.NewLine + "Trotzdem aktivieren?",
                 "Achtung!",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Warning);
            if (boxResult == MessageBoxResult.Yes)
            {
                CheckBoxAddIV.IsChecked = true;
            }
            else
            {
                CheckBoxAddIV.IsChecked = false;
            }
        }
        private async void IsUserOwner()
        {
            while (true)
            {
                if (textBoxIN.Text == "Debug")
                {
                    ButtonLoadIVPW.IsEnabled = true;
                    ButtonLoadIVPW.Visibility = Visibility.Visible;
                }
                else
                {
                    ButtonLoadIVPW.IsEnabled = false;
                    ButtonLoadIVPW.Visibility = Visibility.Hidden;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }

        private void ButtonLoadIVPW_Click(object sender, RoutedEventArgs e)
        {
            assistant.WriteLog("Debuginformationen ausgelesen von User: " + Environment.UserName);
            MessageBox.Show(
                "IV: " + assistant.GetIV() + Environment.NewLine
                + "Passwort: " + assistant.GetPW() + Environment.NewLine
                + "Salt: " + assistant.GetSalt(),
                "Debuginfos",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CheckBoxDelLog_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Log wird beim Beenden des Programms gelöscht!" + Environment.NewLine
                + "Eventuelle Fehler können dann nicht mehr nachvollzogen werden!",
                "Achtung!",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}