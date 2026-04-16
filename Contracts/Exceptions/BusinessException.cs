namespace Contracts.Exceptions
{
    public class BusinessException : Exception
    {
        public int ErrorNumber { get; }
        public Enums.Enums.ActionResult ErrorType { get; }

        public BusinessException(string message, int errorNumber, Enums.Enums.ActionResult errorType) : base(message)
        {
            ErrorNumber = errorNumber;
            ErrorType = errorType;
        }
    }
}