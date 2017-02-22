namespace NuClear.ValidationRules.WebApp.Model
{
    public class Message
    {
        public int Rule { get; set; }
        public string Text { get; set; }
        public string MainReference { get; set; }
        public Level Level { get; set; }
        public string Period { get; set; }
        public string PlainText { get; set; }
        public string Class { get; set; }
    }
}