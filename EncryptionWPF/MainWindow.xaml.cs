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
using System.Diagnostics;

namespace EncryptionWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Variablen
        Assistant assistant = new Assistant();
        MyAes aes = new MyAes();
        Random rnd = new Random();
        string log;
        string iv = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs"))
            {
                Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs");
                log = "Log Verzeichnis erstellt!";
                assistant.WriteLog(log);
            }
            log = "Programm gestartet!";
            assistant.WriteLog(log);
            textBoxLastLog.Text = log;
        }

        private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxPW.Text == null || textBoxPW.Text == "" || textBoxPW.Text == " ")
            {
                MessageBox.Show(
                    "Passwort darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Passwort leer!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
            else if (textBoxIN.Text == null || textBoxIN.Text == "" || textBoxIN.Text == " ")
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Eingabe leer!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
            else
            {
                try
                {
                    if (iv == null)
                    {
                        iv = assistant.RandomGen(16);
                        log = "IV erstellt!";
                        assistant.WriteLog(log);
                        textBoxLastLog.Text = log;
                    }
                    assistant.SetPW(textBoxPW.Text);
                    assistant.SetIV(iv);
                    aes.SetIVfromPW(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Encrypt(textBoxIN.Text);
                    log = "Text verschlüsselt!";
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                        "Kann nicht Verschlüsseln!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    log = "Fehler: Verschlüsseln fehlgeschlagen! " + err.Message;
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
                iv = null;
            }
        }

        private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxPW.Text == null || textBoxPW.Text == "" || textBoxPW.Text == " ")
            {
                MessageBox.Show(
                    "Passwort darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Passwort leer!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
            else if (textBoxIN.Text == null || textBoxIN.Text == "" || textBoxIN.Text == " ")
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Eingabe leer!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
            else
            {
                try
                {
                    assistant.SetPW(textBoxPW.Text);
                    assistant.SetIV(iv);
                    aes.SetIVfromPW(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Decrypt(textBoxIN.Text);
                    log = "Text entschlüsselt!";
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                        "Falsches Passwort!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    log = "Fehler: Entschlüsseln fehlgeschlagen! " + err.Message;
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxOUT.Text == null || textBoxOUT.Text == "" || textBoxOUT.Text == " ")
            {
                MessageBox.Show(
                    "Bitte zuerst einen Text verschlüsseln!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Ausgabe leer!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
            else
            {
                try
                {
                    if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves"))
                    {
                        Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves");
                        log = "Saves Ordner erstellt!";
                        assistant.WriteLog(log);
                        textBoxLastLog.Text = log;
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
                        log = "Datei gespeichert!";
                        assistant.WriteLog(log);
                        textBoxLastLog.Text = log;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(
                    "Konnte Datei nicht speichern!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    log = "Fehler: Speichern fehlgeschlagen! " + err.Message;
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
            }
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
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
                    log = "Datei geladen!";
                    assistant.WriteLog(log);
                    textBoxLastLog.Text = log;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    "Konnte Datei nicht lesen!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = "Fehler: Laden fehlgeschlagen! " + err.Message;
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            textBoxPW.Text = "12345678";
            iv = null;
            textBoxIN.Clear();
            textBoxOUT.Clear();
            log = "Felder resettet!";
            assistant.WriteLog(log);
            textBoxLastLog.Text = log;
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            log = "Programm geschlossen!";
            assistant.WriteLog(log);
            textBoxLastLog.Text = log;
            this.Close();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Erstellt von Tobias Nies." + Environment.NewLine
                + "Dieses Programm nutzt die AES-256 Verschlüsselung, welche auch als Military Standard bekannt ist." + Environment.NewLine
                + "Version 1.1",
                "SimpleEncryption",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void buttonGen_Click(object sender, RoutedEventArgs e)
        {
            textBoxPW.Text = assistant.RandomGen(12);
            log = "Passwort generiert!";
            assistant.WriteLog(log);
            textBoxLastLog.Text = log;
        }

        private void buttonDelLog_Click(object sender, RoutedEventArgs e)
        {
            assistant.DeleteLog();
            textBoxLastLog.Text = "Log wurde gelöscht!";
        }

        private void buttonShowLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log = "Log Datei ausgelesen!";
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
                if (File.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log"))
                {
                    Process.Start(
                        @"C:\Windows\System32\Notepad.exe",
                        "C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log");
                }

            }
            catch (Exception err)
            {
                log = "Konnte Log Datei nicht lesen! " + err.Message;
                assistant.WriteLog(log);
                textBoxLastLog.Text = log;
                MessageBox.Show(
                    "Fehler beim Öffnen der Log Datei!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
