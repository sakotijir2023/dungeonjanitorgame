namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>One slot as the wheel display should draw it.</summary>
    public struct WheelEntry
    {
        public string Label;
        /// <summary>Small second line (e.g. a folder's remembered pick).</summary>
        public string Detail;
        public bool Available;
        public bool Current;
        public bool IsFolder;
        public bool IsBack;
    }
}
