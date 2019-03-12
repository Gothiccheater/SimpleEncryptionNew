using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace EncryptionWPF.Tools
{
    class Assistant
    {

        #region Variablen
        private string iv, password;
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
            password = text;
            while (password.Length < 32)
            {
                password += password;
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
                StreamWriter sw = File.AppendText("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log");
                sw.Write(DateTime.Now + " " + input + Environment.NewLine);
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
                if (File.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log"))
                {
                    File.Delete("C:\\Users\\" + Environment.UserName + "\\Documents\\SEData\\Logs\\SEF.log");
                }
            }
            catch (Exception err)
            {
                WriteLog("Konnte Log nicht löschen! Fehler: " + err.Message);
            }
        }
        public string RandomGen(int count)
        {
            Random rnd = new Random();
            List<char> str = new List<char>();
            for (int i = 0; i < count; i++)
            {
                char c = (char)rnd.Next(33, 126);
                str.Add(c);
            }
            return new string(str.ToArray());
        }
    }
}
