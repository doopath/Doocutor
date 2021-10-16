namespace Tests
{
    internal static class Checkbox
    {
        public static bool State { get; private set; }
        private static readonly object Locker = new();

        public static void TurnOn()
        {
            lock (Locker)
                State = true;
        }

        public static void TurnOff()
        {
            lock (Locker)
                State = false;
        }
    }
}