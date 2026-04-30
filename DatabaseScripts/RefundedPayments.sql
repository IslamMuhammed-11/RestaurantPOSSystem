CREATE OR ALTER PROCEDURE SP_GetRefundedPaymentByID
    @RefundID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RefundID,
        PaymentID,
        Reason,
        Amount,
        RefundedAt
    FROM RefundedPayments
    WHERE RefundID = @RefundID;
END;
GO

