using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eagle._Attributes;
using Eagle._Components.Public;
using Eagle._Components.Public.Delegates;
using Eagle._Containers.Public;
using Eagle._Interfaces.Public;

namespace IptSimulator.Console
{
    class Program
    {
        private static WebClient _webClient = new WebClient();

        static void Main(string[] args)
        {
            StartTestAsync();
            System.Console.ReadLine();
        }

        private static async void StartTestAsync()
        {
            System.Console.Clear();
            System.Console.Write("Set number of requests:");
            var numberOfRequests = int.Parse(System.Console.ReadLine() ?? "100");

            var allRequests = new Task[numberOfRequests];
            for (int i = 0; i < numberOfRequests; i++)
            {
                allRequests[i] = MakeFakeRequest(i, (100 * numberOfRequests) - (i * 100));
            }
            
            await Task.WhenAll(allRequests);
            System.Console.WriteLine("All requests has just completed.");
        }

        private static async Task MakeFakeRequest(int id, int millisecondsToComplete)
        {
            try
            {
                await _webClient.DownloadStringTaskAsync("https://www.google.cz/");
            }
            catch (Exception)
            {
                //don't give a shit
            }
            await Task.Delay(millisecondsToComplete);
            System.Console.WriteLine("Request with ID: {0} has just completed.",id);
        }





















        private static ReturnCode ExecuteCallback(Interpreter interpreter, IClientData clientData, ArgumentList arguments, ref Result result)
        {
            return ReturnCode.Ok;
        }

        private static ReturnCode InteractiveLoopCallback(Interpreter interpreter, InteractiveLoopData loopData, ref Result result)
        {
            return ReturnCode.Ok;
        }
    }

    [ObjectId("291338b7-ce35-41c7-b429-ba4139c05f30")]
    [PluginFlags(
        PluginFlags.Notify | PluginFlags.NoCommands |
        PluginFlags.NoFunctions | PluginFlags.NoPolicies |
        PluginFlags.NoTraces)]
    [NotifyTypes(NotifyType.Command)]
    [NotifyFlags(NotifyFlags.Executed)]
    internal sealed class Pause : Eagle._Plugins.Notify
    {
        #region Public Constructors
        public Pause()
            : base(new PluginData("pause", null, null, null, 
                PluginFlags.Notify | PluginFlags.NoCommands | PluginFlags.NoFunctions | PluginFlags.NoPolicies, 
                Version.Parse("1.0.0"), null, AppDomain.CurrentDomain, Assembly.GetAssembly(typeof(Pause)), null, 
                null, typeof(Pause).FullName, null, null, null, null, null, null, null, null, 0))
        {
            this.Flags |= Utility.GetPluginFlags(GetType().BaseType) |
                Utility.GetPluginFlags(this);
        }
        #endregion


        ///////////////////////////////////////////////////////////////////////

        #region INotify Members
        public override ReturnCode Notify(
            Interpreter interpreter,
            IScriptEventArgs eventArgs,
            IClientData clientData,
            ArgumentList arguments,
            ref Result result
            )
        {
            if (eventArgs == null)
                return ReturnCode.Ok;

            if (!Utility.HasFlags(
                    eventArgs.NotifyTypes, NotifyType.Command, false))
            {
                return ReturnCode.Ok;
            }

            NotifyFlags notifyFlags = eventArgs.NotifyFlags;

            if (!Utility.HasFlags(
                notifyFlags, NotifyFlags.Executed, false))
            {
                return ReturnCode.Ok;
            }

            Thread.Sleep(500); /* TODO: Fine tune? */
            return ReturnCode.Ok;
        }
        #endregion
    }

}


//Result result = null;
//var interpreter = Interpreter.Create(
//    null,
//    CreateFlags.Debug | CreateFlags.Debugger | CreateFlags.DebuggerInterpreter | CreateFlags.DebuggerUse,
//    InitializeFlags.Default,
//    ScriptFlags.Interactive | ScriptFlags.User,
//    InterpreterFlags.Default | InterpreterFlags.ScriptLocation,
//    null, null, null, null, new ExecuteCallbackDictionary(new List<ExecuteCallback>()),
//    null, null, null, null, ref result);

//interpreter.Interactive = true;
//            interpreter.Debug = true;
            
//            long token = 0;
////interpreter.AddPlugin(new Pause(), null, ref token, ref result);
//interpreter.InteractiveLoopCallback += InteractiveLoopCallback;
//            var sw = new Stopwatch();
//sw.Start();
//            var code = interpreter.EvaluateScript(
//@"
//debug enable true
//debug interactive true
//debug types +Demand
//object invoke Interpreter.GetActive Exit false

//set a 0
//debug break
//set a", ref result);
//sw.Stop();

//            System.Console.WriteLine($"Return code {code} with result: {result}, elapsed time: {sw.Elapsed.TotalSeconds}");

//            System.Console.ReadLine();