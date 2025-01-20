namespace MyWay
{
    public interface ICounter
    {
        public int CurrentValue { get; }
        public void Increment(int add);
    }
}
