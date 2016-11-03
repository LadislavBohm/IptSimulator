using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Eagle._Commands;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IptSimulator.CiscoTcl.Commands;
using IptSimulator.CiscoTcl.Utils;
using IptSimulator.Client.Exceptions;
using IptSimulator.Client.Views;
using IptSimulator.Core.Tcl;
using NLog;


namespace IptSimulator.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string TclSyntaxFileName = "TclSyntax.xshd";
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupTclHighlighting();
        }

        private void SetupTclHighlighting()
        {
            try
            {
                var xmlFile = LoadSyntaxXmlFile();

                UpdateHighlightingFile(xmlFile);

                using (XmlTextReader reader = new XmlTextReader(new StringReader(xmlFile.ToString())))
                {
                    var result = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                    HighlightingManager.Instance.RegisterHighlighting("TCL", new[] { ".tcl" }, result);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while setting up TCL syntax highlighting");
                throw;
            }
        }

        private XDocument LoadSyntaxXmlFile()
        {
            string syntaxPath = "IptSimulator.Client.Resources." + TclSyntaxFileName;

            _logger.Debug($"Loading TCL syntax from embedded resources path {syntaxPath}.");
            using (var stream = typeof(App).Assembly.GetManifestResourceStream(syntaxPath))
            {
                var result = XDocument.Load(stream);
                _logger.Debug("Syntax file successfully loaded from embedded resources.");
                return result;
            }
        }

        private void UpdateHighlightingFile(XDocument syntaxXml)
        {
            _logger.Info("Updating syntax file for custom TCL commands and events");

            if (syntaxXml.Root == null)
            {
                throw new ArgumentException("TCL syntax file does not contain root node.");
            }
            var keywords = syntaxXml.Root.Descendants().First(e => e.Name.LocalName == "Keywords");
            var ns = syntaxXml.Root.GetDefaultNamespace();

            _logger.Debug("Adding TCL keywords.");
            foreach (var keyword in TclKeywords.All)
            {
                _logger.Trace($"Adding TCL keyword {keyword} to keywords.");
                keywords.Add(new XElement(ns + "Word", keyword));
            }

            _logger.Debug("Adding custom TCL commands.");
            foreach (var tclCommand in CustomCommandsProvider.GetCustomCommands())
            {
                _logger.Debug($"Adding {tclCommand} TCL command to keywords.");
                keywords.Add(new XElement(ns + "Word", tclCommand.Name));
            }

            _logger.Info("Syntax file successfully updated.");
        }
    }
}
