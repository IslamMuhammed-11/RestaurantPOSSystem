namespace Contracts.Exceptions
{
    public class BusinessException : Exception
    {
        public int ErrorNumber { get; }
        public Enums.ActionResultEnum.ActionResult ErrorType { get; }

        public BusinessException(string message, int errorNumber, Enums.ActionResultEnum.ActionResult errorType) : base(message)
        {
            ErrorNumber = errorNumber;
            ErrorType = errorType;
        }
    }
}