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
        private string iv, password;
        public void SetIV(string input)
        {
            iv = input;
            while(iv.Length < 16)
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
            while(password.Length < 32)
            {
                password += password;
            }
            while(password.Length > 32)
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
            string content = null;
            if (File.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs\\SEF.log"))
            {
                content = File.ReadAllText("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs\\SEF.log");
            }
            StreamWriter sw = new StreamWriter("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs\\SEF.log");
            sw.WriteAsync(content + input);
            sw.Close();
        }
        public void DeleteLog()
        {
            if (File.Exists("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs\\SEF.log"))
            {
                File.Delete("C:\\Users\\" + Environment.UserName + "\\Documents\\SEFdata\\Logs\\SEF.log");
            }
        }
    }
}
