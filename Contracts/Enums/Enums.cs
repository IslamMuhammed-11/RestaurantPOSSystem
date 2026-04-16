namespace Contracts.Enums
{
    public class Enums
    {
        public enum ActionResult
        {
            Success,
            NotFound,
            InvalidData,
            Error,
            DBError,
            Conflict,

            //User related results
            InvalidPassword,

            AlreadyActive,
            AlreadyInactive,
            InActiveUser,
            WeakPassword
        }
    }
}