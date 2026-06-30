CREATE VIEW [dbo].[MismatchesJobTransactions] AS
SELECT BankingAccountAPP,
    BankingAccountQBO,
    [Account/Category],
    Class,
    Date,
    TxnID,
    RefNo,
    Type,
    Payee,
    Memo,
    [Division/Location],
    Amount
FROM msf_GetMismatchesJobTransactions()