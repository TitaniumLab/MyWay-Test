namespace MyWay
{
    public class Counter : ICounter
    {
        private int _currentValue;
        public int CurrentValue { get { return _currentValue; } }


        public Counter(int startValue)
        {
            _currentValue = startValue;
        }


        public void Increment(int add)
        {
            _currentValue += add;
        }
    }
}
