ALTER TABLE "ClientTotalLoan"
DROP CONSTRAINT "FK_ClientTotalLoan_ClientLoansCalculation_ClientLoansCalculati~";

ALTER TABLE "ClientTotalLoan"
DROP CONSTRAINT "PK_ClientTotalLoan";

ALTER TABLE "DepositSnapshot"
DROP CONSTRAINT "FK_DepositSnapshot_ClientSnapshot_DatasetId_ClientId";

ALTER TABLE "DepositSnapshot"
DROP CONSTRAINT "PK_DepositSnapshot";

ALTER TABLE "LoanSnapshot"
DROP CONSTRAINT "FK_LoanSnapshot_ClientSnapshot_DatasetId_ClientId";

ALTER TABLE "LoanSnapshot"
DROP CONSTRAINT "PK_LoanSnapshot";

ALTER TABLE "ClientSnapshot"
DROP CONSTRAINT "FK_ClientSnapshot_Dataset_DatasetId";

ALTER TABLE "ClientSnapshot"
DROP CONSTRAINT "PK_ClientSnapshot";

