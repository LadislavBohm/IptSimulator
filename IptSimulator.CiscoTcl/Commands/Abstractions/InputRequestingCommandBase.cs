using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.CiscoTcl.Model.EventArgs;
using IptSimulator.CiscoTcl.Model.InputData;
using NLog;

namespace IptSimulator.CiscoTcl.Commands.Abstractions
{
    public class InputRequestingCommandBase<TData> : IInputRequestingCommand<TData> where TData: BaseInputData
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public virtual TData InputData { get; protected set; }

        protected void RequestAndProcessInputData()
        {
            var args = new InputEventArgs<TData>();
            if (OnInputRequested == null)
            {
                _logger.Error($"No subscribers to {nameof(OnInputRequested)} event. Cannot request for input data.");
                throw new Exception($"No subscribers to {nameof(OnInputRequested)} event. Cannot request for input data.");
            }

            OnInputRequested.Invoke(this, args);

            _logger.Info($"Processing input data of type {typeof(TData).Name} in {GetType().Name}");

            if (args.Data == null)
            {
                _logger.Error($"Input dats is null or not set via {nameof(InputEventArgs<TData>.SetInputData)} method.");
                throw new ArgumentNullException(nameof(InputEventArgs<TData>.Data));
            }

            if (!IsInputDataValid(args.Data))
            {
                _logger.Error("Input data did not pass validation.");
                throw new ArgumentException("Input data did not pass validation.");
            }

            //data should be not null and valid at this time
            InputData = args.Data;

            ProcessInputDataInternal();

            _logger.Info("Input data successfully received and validated.");
        }

        protected virtual bool IsInputDataValid(TData inputData)
        {
            //to be overridden in concrete classes
            return true;
        }

        protected virtual void ProcessInputDataInternal()
        {
            //to be overridden in concrete classes
        }

        public event EventHandler<InputEventArgs<TData>> OnInputRequested;
        
    }
}
