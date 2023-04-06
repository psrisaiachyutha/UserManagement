namespace Common.Exceptions
{
    public class RecordAlreadyExistsException : Exception
    {
        public RecordAlreadyExistsException() : base() { }

        public RecordAlreadyExistsException(string message) : base(message) { }

        public RecordAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
    }
}
