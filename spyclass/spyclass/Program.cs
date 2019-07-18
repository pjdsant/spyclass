using System;
using System.Diagnostics;

namespace spyclass
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                string param = args[0];

                Process[] processlist = Process.GetProcesses();

                bool hasHandler = false;

                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle) && (param == process.MainWindowTitle))
                    {
                        //Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                        //WinApi.WinApiX.GetAllFromWindowByTitle(param);
                        hasHandler = true;
                    }
                    
                   
                    //Console.WriteLine("Informe o Titulo da Tela Windows");
                    
                }

                if (hasHandler)
                {
                    WinApi.WinApiX.GetAllFromWindowByTitle(param);
                    //Logger.Log(String.Format($"{"Log criado em "} : {DateTime.Now}"), "SpyLog");
                    //Logger.Log("Registro de log de auditoria");

                }
                else
                {
                    Console.WriteLine("Tela nao encontrada, por favor informe o Titulo da Tela da sua Aplicacao Windows a se Processada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Esta faltando parametro.");
                Console.WriteLine("Execute o comando => spyclass.exe \"Windows Title\" ");
            }
            
            //Console.ReadLine();
        }
    }
}
