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
        Assistant assistant = new Assistant();
        MyAes aes = new MyAes();
        Random rnd = new Random();
        string log;
        string iv = "0";
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs"))
            {
                Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs");
                log = DateTime.Now + " Log Verzeichnis erstellt\n";
                assistant.WriteLog(log);
            }
            log = DateTime.Now + " Programm gestartet!\n";
            assistant.WriteLog(log);
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
                log = DateTime.Now + " Fehler: Passwort leer!\n";
                assistant.WriteLog(log);
            }
            else if (textBoxIN.Text == null || textBoxIN.Text == "" || textBoxIN.Text == " ")
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = DateTime.Now + " Fehler: Eingabe leer!\n";
                assistant.WriteLog(log);
            }
            else
            {
                try
                {
                    if(iv == "0")
                    {
                        List<char> str = new List<char>();
                        for (int i = 0; i <= 16; i++)
                        {
                                char c = (char)rnd.Next(33, 122);
                                str.Add(c);
                        }
                        iv = new string(str.ToArray());
                        log = DateTime.Now + " IV erstellt\n";
                        assistant.WriteLog(log);
                    }
                    assistant.SetPW(textBoxPW.Text);
                    assistant.SetIV(iv);
                    aes.SetIVfromPW(assistant.GetIV());
                    aes.Generator(assistant.GetPW());
                    textBoxOUT.Text = aes.Encrypt(textBoxIN.Text);
                    log = DateTime.Now + " Text verschlüsselt\n";
                    assistant.WriteLog(log);
                }
                catch(Exception err)
                {
                    MessageBox.Show(
                        "Kann nicht Verschlüsseln!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    log = DateTime.Now + " Fehler: Verschlüsseln fehlgeschlagen! " + err.Message + "\n";
                    assistant.WriteLog(log);
                }
                iv = "0";
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
                log = DateTime.Now + " Fehler: Passwort leer!\n";
                assistant.WriteLog(log);
            }
            else if(textBoxIN.Text == null || textBoxIN.Text == "" || textBoxIN.Text == " ")
            {
                MessageBox.Show(
                    "Eingabefeld darf nicht leer sein!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = DateTime.Now + " Fehler: Eingabe leer!\n";
                assistant.WriteLog(log);
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
                    log = DateTime.Now + " Text entschlüsselt!\n";
                    assistant.WriteLog(log);
                }
                catch(Exception err)
                {
                    MessageBox.Show(
                        "Falsches Passwort!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    log = DateTime.Now + " Fehler: Entschlüsseln fehlgeschlagen! " + err.Message + "\n";
                    assistant.WriteLog(log);
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
                log = DateTime.Now + " Fehler: Ausgabe leer!\n";
                assistant.WriteLog(log);
            }
            else
            {
                try
                {
                    if(!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Saves"))
                    {
                        Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Saves");
                        log = DateTime.Now + " Saves Ordner erstellt!\n";
                        assistant.WriteLog(log);
                    }
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "SimpleEncryptionFile|*.sfd|Alle Dateien|*.*";
                    saveFileDialog.FileName = "MeinText";
                    saveFileDialog.DefaultExt = ".sfd";
                    saveFileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Saves";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                        sw.WriteAsync(textBoxOUT.Text + "||" + assistant.GetIV());
                        sw.Close();
                        log = DateTime.Now + " Datei gespeichert!\n";
                        assistant.WriteLog(log);
                    }
                }
                catch(Exception err)
                {
                    MessageBox.Show(
                    "Konnte Datei nicht speichern!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    log = DateTime.Now + " Fehler: Speichern fehlgeschlagen! " + err.Message + "\n";
                    assistant.WriteLog(log);
                }
            }
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SimpleEncryptionFile|*.sfd|Alle Dateien|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    content = File.ReadAllText(openFileDialog.FileName);
                    textBoxIN.Text = content.Substring(0, content.LastIndexOf("||"));
                    iv = content.Remove(0, content.LastIndexOf("||") + 2);
                    log = DateTime.Now + " Datei geladen!\n";
                    assistant.WriteLog(log);
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(
                    "Konnte Datei nicht lesen!",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                log = DateTime.Now + " Fehler: Laden fehlgeschlagen! " + err.Message + "\n";
                assistant.WriteLog(log);
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            textBoxPW.Text = "12345678";
            textBoxIN.Clear();
            textBoxOUT.Clear();
            log = DateTime.Now + " Felder resettet!\n";
            assistant.WriteLog(log);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            log = DateTime.Now + " Programm geschlossen!\n";
            assistant.WriteLog(log);
            this.Close();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Ein Programm zur Verschlüsselung",
                "SimpleEncryption",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void buttonGen_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            List<char> str = new List<char>();
            while (i < 10)
            {
                char c = (char)rnd.Next(33, 125);
                str.Add(c);
                i++;
            }
            textBoxPW.Text = new string(str.ToArray());
            log = DateTime.Now + " Passwort generiert!\n";
            assistant.WriteLog(log);
        }

        private void buttonDelLog_Click(object sender, RoutedEventArgs e)
        {
            assistant.DeleteLog();
        }
    }
}
