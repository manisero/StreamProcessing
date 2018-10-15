CREATE TABLE "Dataset"
(
    "DatasetId" smallint not null,
	"Date" date not null,
    CONSTRAINT "PK_Dataset" PRIMARY KEY ("DatasetId")
);

CREATE TABLE "Client"
(
    "DatasetId" smallint not null,
    "ClientId" int not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "Client_1" PARTITION OF "Client" (CONSTRAINT "PK_Client_1" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (1);
CREATE TABLE "Client_2" PARTITION OF "Client" (CONSTRAINT "PK_Client_2" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (2);
CREATE TABLE "Client_3" PARTITION OF "Client" (CONSTRAINT "PK_Client_3" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (3);
CREATE TABLE "Client_4" PARTITION OF "Client" (CONSTRAINT "PK_Client_4" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (4);
CREATE TABLE "Client_5" PARTITION OF "Client" (CONSTRAINT "PK_Client_5" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (5);
CREATE TABLE "Client_6" PARTITION OF "Client" (CONSTRAINT "PK_Client_6" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (6);
CREATE TABLE "Client_7" PARTITION OF "Client" (CONSTRAINT "PK_Client_7" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (7);
CREATE TABLE "Client_8" PARTITION OF "Client" (CONSTRAINT "PK_Client_8" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (8);
CREATE TABLE "Client_9" PARTITION OF "Client" (CONSTRAINT "PK_Client_9" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (9);

CREATE TABLE "Loan"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "LoanId" int not null,
	"Value" numeric(19, 4) not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "Loan_1" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_1" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (1);
CREATE TABLE "Loan_2" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_2" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (2);
CREATE TABLE "Loan_3" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_3" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (3);
CREATE TABLE "Loan_4" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_4" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (4);
CREATE TABLE "Loan_5" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_5" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (5);
CREATE TABLE "Loan_6" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_6" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (6);
CREATE TABLE "Loan_7" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_7" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (7);
CREATE TABLE "Loan_8" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_8" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (8);
CREATE TABLE "Loan_9" PARTITION OF "Loan" (CONSTRAINT "PK_Loan_9" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (9);
