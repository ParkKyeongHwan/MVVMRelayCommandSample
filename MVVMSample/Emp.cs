namespace MVVMSample
{
    public class Emp
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public override string ToString()
        {
            return string.Concat("[", Name, ",", Job, "]");
        }
    }
}
