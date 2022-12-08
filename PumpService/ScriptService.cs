using Microsoft.CSharp;
using PumpService.Interfaces;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PumpService
{
    public class ScriptService : IScriptService
    {
        private CompilerResults results = null;
        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _settingsService;
        private readonly IPumpServiceCallback _pumpServiceCallback;

        public ScriptService(IStatisticsService statisticsService,
                             ISettingsService settingsService,
                             IPumpServiceCallback pumpServiceCallback)
        {
            _statisticsService = statisticsService;
            _settingsService = settingsService;
            _pumpServiceCallback = pumpServiceCallback;
        }

        public bool Compile()
        {
            try
            {
                var compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true
                };
                compilerParameters.ReferencedAssemblies.Add("System.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
                compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                var fileStream = new FileStream(_settingsService.FileName, FileMode.Open);
                byte[] buffer;
                try
                {
                    int length = (int)fileStream.Length;
                    buffer = new byte[length];
                    int count, sum = 0;
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    {
                        sum += count;
                    }
                }
                finally
                {
                    fileStream.Close();
                }

                CSharpCodeProvider provider = new CSharpCodeProvider();
                results = provider.CompileAssemblyFromSource(compilerParameters, Encoding.UTF8.GetString(buffer));

                if (results.Errors != null && results.Errors.Count >= 0)
                {
                    var errorsList = new StringBuilder();
                    for (int i = 0; i < results.Errors.Count; i++)
                    {
                        errorsList.AppendLine(results.Errors[i].ToString());
                    }
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Run(int count)
        {
            if (results == null || (results != null && results.Errors != null && results.Errors.Count > 0))
            {
                if (Compile() == false)
                {
                    return;
                } 
            }

            Type type = results.CompiledAssembly.GetType("Sample.SampleScript");
            if(type == null)
            {
                return;
            }

            MethodInfo methodInfo = type.GetMethod("EntryPoint");
            if(methodInfo == null)
            {
                return;
            }

            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bool)methodInfo.Invoke(Activator.CreateInstance(type), null))
                    {
                        _statisticsService.SuccessTacts++;
                    }
                    else
                    {
                        _statisticsService.ErrorTacts++;
                    }
                    _statisticsService.AllTacts++;

                    _pumpServiceCallback.UpdateStatistics((StatisticsService)_statisticsService);
                    Thread.Sleep(1000);
                }
            });
        }
    }
}