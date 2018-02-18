namespace Code11.Model
{
    internal static class ReminderStatus
    {
        internal static string Received()
        {
            return "Received";
        }
        internal static string Sent()
        {
            return "Sent";
        }
        internal static string TimeoutError()
        {
            return "TimeoutError";
        }
        internal static string Error()
        {
            return "Error";
        }
    }
}