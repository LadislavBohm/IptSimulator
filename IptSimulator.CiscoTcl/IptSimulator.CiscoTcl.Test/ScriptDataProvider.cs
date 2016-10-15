using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Test
{
    public sealed class ScriptDataProvider
    {
        private const string ScriptBasePath = "IptSimulator.CiscoTcl.Test.Scripts";
        private static readonly Assembly ThisAssembly = typeof(ScriptDataProvider).Assembly;

        #region Script Properties

        public string CallWait => ReadScriptFromEmbeddedResources("call_wait.tcl");

        public string Pinovnice => ReadScriptFromEmbeddedResources("pinovnice.tcl");

        public string FsmStateChanging => ReadScriptFromEmbeddedResources("fsm_state_changing.tcl");

        #endregion

        #region Helper methods

        private static string ReadScriptFromEmbeddedResources(string scriptName)
        {
            if (string.IsNullOrEmpty(scriptName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(scriptName));

            var scriptPath = CreateScriptPath(scriptName);
            using (Stream stream = ThisAssembly.GetManifestResourceStream(scriptPath))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Script with path: {scriptPath} does not exist.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string CreateScriptPath(string scriptName)
        {
            return $"{ScriptBasePath}.{scriptName}";
        }


        #endregion
    }
}
