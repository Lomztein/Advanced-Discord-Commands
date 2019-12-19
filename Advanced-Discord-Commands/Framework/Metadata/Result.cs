using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class Result {

        public object Value { get; private set; }
        public string Message { get; private set; }
        public bool Failed { get; private set; }
        public Exception Exception { get; private set; }
        public bool HiddenExecution { get; private set; }
        public bool Succesful { get => Value != null || !string.IsNullOrEmpty(Message) || Exception != null; }

        public Result(object value, string message) {
            Value = value;
            Message = message;
        }

        public Result(object value, string message, bool failed) : this(value, message)
        {
            Failed = failed;
        }

        public Result (Exception _exception) {
            Exception = _exception;
            Failed = true;
        }

        public string GetMessage () {

            if (Exception != null)
                return "Error - " + Exception.Message;

            if (string.IsNullOrWhiteSpace (Message))
                return "";

            return Message;
        }

        public object GetValue () {
            return Value;
        }

        public void SetHidden(bool value) => HiddenExecution = value;

    }


}
