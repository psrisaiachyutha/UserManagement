namespace Common.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException() : base() { }

        public RecordNotFoundException(string message) : base(message) { }

        public RecordNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
