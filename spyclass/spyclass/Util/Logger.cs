﻿using System;
using System.IO;
using System.Reflection;

namespace LikeWater.SpyClass.Util
{
    public class Logger
    {
        private static string caminhoExe = string.Empty;
        public static bool Log(string strMensagem, string strNomeArquivo = "SpyLog")
        {
            try
            {
                caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string caminhoArquivo = Path.Combine(caminhoExe, strNomeArquivo);
                if (!File.Exists(caminhoArquivo))
                {
                    FileStream arquivo = File.Create(caminhoArquivo);
                    arquivo.Close();
                }
                using (StreamWriter w = File.AppendText(caminhoArquivo))
                {
                    AppendLog(strMensagem, w);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static void AppendLog(string logMensagem, TextWriter txtWriter)
        {
            try
            {
                //txtWriter.WriteLine("\r\nLog In");
                //txtWriter.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                //txtWriter.Write("");
                txtWriter.WriteLine($"{logMensagem}");
                txtWriter.Write("|");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
