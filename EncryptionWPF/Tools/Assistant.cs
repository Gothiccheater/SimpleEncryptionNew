﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Security.Cryptography;

namespace EncryptionWPF.Tools
{
    class Assistant
    {

        #region Variablen
        private string iv, password, salt;
        private readonly string logPath = "C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log";
        SHA1 sha1 = SHA1.Create();
        Random rnd = new Random();
        #endregion

        public void SetIV(string input)
        {
            iv = input;
            while (iv.Length < 16)
            {
                iv += iv;
            }
            while (iv.Length > 16)
            {
                iv = iv.Remove(16, 1);
            }
        }
        public string GetIV()
        {
            return iv;
        }
        public void SetPW(string text)
        {
            password = string.Empty;
            while (password.Length < 32)
            {
                byte[] pwbytes = Encoding.Default.GetBytes(text + GetSalt());
                byte[] sha = sha1.ComputeHash(pwbytes);
                string hash = Encoding.Default.GetString(sha);
                password += hash;
            }
            while (password.Length > 32)
            {
                password = password.Remove(32, 1);
            }
        }
        public string GetPW()
        {
            return password;
        }
        public void WriteLog(string input)
        {
            try
            {
                StreamWriter sw = File.AppendText(logPath);
                sw.Write("[" + DateTime.Now + "] " + input + Environment.NewLine);
                sw.Close();
            }
            catch
            {
                //nothing
            }
        }
        public void DeleteLog()
        {
            try
            {
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }
            }
            catch (Exception err)
            {
                WriteLog("Konnte Log nicht löschen! Fehler: " + err.Message);
            }
        }
        public string RandomGen(int count)
        {
            List<char> str = new List<char>();
            for (int i = 0; i < count; i++)
            {
                char c = (char)rnd.Next(33, 126);
                str.Add(c);
            }
            return new string(str.ToArray());
        }
        public string GetLogPath()
        {
            return logPath;
        }
        public void ResetValues()
        {
            iv = null;
            password = null;
        }
        public void CreateDirectories()
        {
            try
            {
                if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs"))
                {
                    Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs");
                    WriteLog("Log Verzeichnis erstellt!");
                }
                if (!Directory.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Saves"))
                {
                    Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\UserData");
                    WriteLog("UserData Verzeichnis erstellt!");
                }
            }
            catch
            {
                //Nothing
            }
        }
        public void CreateSalt()
        {
            salt = RandomGen(rnd.Next(100, 1000));
        }
        public string GetSalt()
        {
            return salt;
        }
        public void SetSalt(string text)
        {
            salt = text;
        }
    }
}
