using System;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public struct DiagnosticLog
    {
        public DateTime Timestamp;
        public long ProcessWorkingSet;
        public long GcAllocatedSet;
    }
}
