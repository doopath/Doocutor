namespace Tests
{
    internal static class Checkbox
    {
        public static bool State { get; private set; }

        public static void TurnOn() => State = true;
        
        public static void TurnOff() => State = false;
    }
}