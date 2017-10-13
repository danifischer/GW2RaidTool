using System.Collections.Generic;

namespace EVTC_Log_Parser.Model
{
    public abstract class BoonStack
    {
	    protected readonly int _capacity;
        protected List<int> _stack = new List<int>();

	    public List<int> Stack
        {
            get { return _stack; }
            set { _stack = value; }
        }

	    public BoonStack(int capacity)
        {
            _capacity = capacity;
        }

	    public abstract int CalculateValue();
        public abstract void Update(int timePassed);

	    public void Add(int duration)
        {
            if (IsFull())
            {
                int i = _stack.Count - 1;
                if (_stack[i] < duration)
                {
                    _stack[i] = duration;
                    ReverseSort();
                }
            }
            else
            {
                _stack.Add(duration);
                ReverseSort();
            }
        }

	    protected bool IsFull()
        {
            return _stack.Count >= _capacity;
        }

        protected void ReverseSort()
        {
            _stack.Sort();
            _stack.Reverse();
        }
    }
}
