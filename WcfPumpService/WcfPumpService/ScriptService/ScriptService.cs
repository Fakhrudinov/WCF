using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WcfPumpService.SettingsService;
using WcfPumpService.StatisticService;

namespace WcfPumpService.ScriptService
{
    public class ScriptService : IScriptService
    {
        // компилировать сюда
        private CompilerResults сompilerResults = null;

        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _settingsService;
        private readonly IPumpServiceCallback _pumpServiceCallback;

        public ScriptService(
            IPumpServiceCallback callback,
            ISettingsService serviceSettings,
            IStatisticsService statisticsService)
        {
            _settingsService = serviceSettings;
            _statisticsService = statisticsService;
            _pumpServiceCallback = callback;
        }

        public bool Compile()
        {
            try
            {
                CompilerParameters compilerParameters = new CompilerParameters();
                compilerParameters.GenerateInMemory = true; // скрипт компилировать в памяти

                // добавить библиотеки, нужные для работы скомпилированной сборки
                compilerParameters.ReferencedAssemblies.Add("System.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
                compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

                // добавить в компилирующуся сборку текущую сборку
                compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                // в buffer [] считать текст скрипта
                FileStream fileStream = new FileStream(_settingsService.FileName, FileMode.Open);
                byte[] buffer;
                try
                {
                    int length = (int)fileStream.Length;
                    buffer = new byte[length];
                    int count;
                    int sum = 0;
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;
                }
                finally
                {
                    fileStream.Close();
                }

                CSharpCodeProvider provider = new CSharpCodeProvider();// инициализация объекта CSharpCodeProvider

                // компиляция
                сompilerResults = provider.CompileAssemblyFromSource(
                    compilerParameters, // параметры, опции компиляции
                    System.Text.Encoding.UTF8.GetString(buffer)); // текст скрипта в виде текста
                
                //проверка успешности компиляции
                if (сompilerResults.Errors != null && сompilerResults.Errors.Count != 0)
                {
                    string compileErrors = string.Empty;
                    for (int i = 0; i < сompilerResults.Errors.Count; i++)
                    {
                        if (compileErrors != string.Empty)
                        {
                            compileErrors += "\r\n";
                            Console.WriteLine("сompilerResults.Errors = " + compileErrors);
                        }
                        compileErrors += сompilerResults.Errors[i];
                    }

                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when compile! " + e.Message);
                return false;
            }
        }

        public void Run(int count)
        {
            // проверка успешности компиляции перед запуском
            if (сompilerResults == null || 
                (сompilerResults != null && 
                сompilerResults.Errors != null && 
                сompilerResults.Errors.Count > 0))
            {
                return;
                //// еще раз компиляция, если проверки провалены. Убрать? Здесь то уже не проверяются ошибки...
                //if (Compile() == false)
                //{
                //    return;
                //}
            }

            // проверка - есть ли ожидаемый класс SampleScript в скомпилированном коде
            // t=описание класса SampleScript
            Type t = сompilerResults.CompiledAssembly.GetType("Sample.SampleScript");
            if (t == null)
            {
                return;
            }

            // проверка - есть ли ожидаемый метод в скомпилированном коде
            MethodInfo entryPointMethod = t.GetMethod("EntryPoint");
            if (entryPointMethod == null)
            {
                return;
            }

            // запуск скомпилированного: наш метод, полученный entryPointMethod - с помощью Invoke
            //результаты запуска пихаем в статистику
            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bool)entryPointMethod //приведение результатов выполнения entryPointMethod в виде obj к типу bool
                        .Invoke(//Activator - создает объект на базе описания типа
                            Activator.CreateInstance(t), // obj, объект, на котором будем вызывать метод из entryPointMethod(если он НЕ статик)
                            null)) // массив параметров для метода, если они нужны
                    {
                        _statisticsService.SuccessTacts++;
                    }
                    else
                    {
                        _statisticsService.ErrorTacts++;
                    }
                    _statisticsService.AllTacts++;

                    // вызов обновления статистики
                    _pumpServiceCallback.UpdateStatistics((StatisticsService)_statisticsService); // (StatisticsService)_statisticsService = приведение к классу StatisticsService

                    Thread.Sleep(1000);
                }
            });

        }
    }
}