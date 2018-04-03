﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    internal class EventGenerator : IEventGenerator
    {
        private const string EventTimestamp = "MM/dd/yyyy hh:mm:ss.fff tt";
        private const string DefaultProductionSlotName = "production";

        public void LogFunctionTraceEvent(TraceLevel level, string subscriptionId, string appName, string functionName, string eventName, string source, string details, string summary, string exceptionType, string exceptionMessage, string functionInvocationId, string hostInstanceId, string activityId)
        {
            string eventTimestamp = DateTime.UtcNow.ToString(EventTimestamp);
            FunctionsSystemLogsEventSource.Instance.SetActivityId(activityId);
            switch (level)
            {
                case TraceLevel.Verbose:
                    FunctionsSystemLogsEventSource.Instance.RaiseFunctionsEventVerbose(subscriptionId, appName, functionName, eventName, source, details, summary, ScriptHost.Version, eventTimestamp, functionInvocationId, hostInstanceId);
                    break;
                case TraceLevel.Info:
                    FunctionsSystemLogsEventSource.Instance.RaiseFunctionsEventInfo(subscriptionId, appName, functionName, eventName, source, details, summary, ScriptHost.Version, eventTimestamp, functionInvocationId, hostInstanceId);
                    break;
                case TraceLevel.Warning:
                    FunctionsSystemLogsEventSource.Instance.RaiseFunctionsEventWarning(subscriptionId, appName, functionName, eventName, source, details, summary, ScriptHost.Version, eventTimestamp, functionInvocationId, hostInstanceId);
                    break;
                case TraceLevel.Error:
                    FunctionsSystemLogsEventSource.Instance.RaiseFunctionsEventError(subscriptionId, appName, functionName, eventName, source, details, summary, ScriptHost.Version, eventTimestamp, exceptionType, exceptionMessage, functionInvocationId, hostInstanceId);
                    break;
            }
        }

        public void LogFunctionMetricEvent(string subscriptionId, string appName, string functionName, string eventName, long average, long minimum, long maximum, long count, DateTime eventTimestamp)
        {
            FunctionsSystemLogsEventSource.Instance.LogFunctionMetricEvent(subscriptionId, appName, functionName, eventName, average, minimum, maximum, count, ScriptHost.Version, eventTimestamp.ToString(EventTimestamp));
        }
        public void LogFunctionExecutionAggregateEvent(string siteName, string functionName, long executionTimeInMs, long functionStartedCount, long functionCompletedCount, long functionFailedCount)
        {
            FunctionsEventSource.Instance.LogFunctionExecutionAggregateEvent(siteName, functionName, (ulong)executionTimeInMs, (ulong)functionStartedCount, (ulong)functionCompletedCount, (ulong)functionFailedCount);
        }

        public void LogFunctionDetailsEvent(string siteName, string functionName, string inputBindings, string outputBindings, string scriptType, bool isDisabled)
        {
            FunctionsEventSource.Instance.LogFunctionDetailsEvent(siteName, functionName, inputBindings, outputBindings, scriptType, isDisabled);
        }

        public void LogFunctionExecutionEvent(string executionId, string siteName, int concurrency, string functionName, string invocationId, string executionStage, long executionTimeSpan, bool success)
        {
            FunctionsEventSource.Instance.LogFunctionExecutionEvent(executionId, siteName, concurrency, functionName, invocationId, executionStage, (ulong)executionTimeSpan, success);
        }

        public void LogFunctionDiagnosticEvent(TraceLevel level, string resourceId, string operationName, string category, string regionName, string properties)
        {
            // Azure Monitor has no "Verbose" setting, so we map that to "Informational". We're controlling this logic here to minimize
            // the amount of logic we have deployed with our monitoring configuration.
            switch (level)
            {
                case TraceLevel.Verbose:
                    FunctionsDiagnosticLogsEventSource.Instance.RaiseFunctionsDiagnosticEventVerbose(resourceId, operationName, category, regionName, "Informational", properties);
                    break;
                case TraceLevel.Info:
                    FunctionsDiagnosticLogsEventSource.Instance.RaiseFunctionsDiagnosticEventInformational(resourceId, operationName, category, regionName, "Informational", properties);
                    break;
                case TraceLevel.Warning:
                    FunctionsDiagnosticLogsEventSource.Instance.RaiseFunctionsDiagnosticEventWarning(resourceId, operationName, category, regionName, "Warning", properties);
                    break;
                case TraceLevel.Error:
                    FunctionsDiagnosticLogsEventSource.Instance.RaiseFunctionsDiagnosticEventError(resourceId, operationName, category, regionName, "Error", properties);
                    break;
            }
        }
    }
}