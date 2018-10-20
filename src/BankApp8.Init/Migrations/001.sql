CREATE TABLE "Dataset"
(
    "DatasetId" smallint generated always as identity,
	"Date" date not null,
    CONSTRAINT "PK_Dataset" PRIMARY KEY ("DatasetId")
);

CREATE TABLE "ClientSnapshot"
(
    "DatasetId" smallint not null,
    "ClientId" int not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "ClientSnapshot_1" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_1" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (1);
CREATE TABLE "ClientSnapshot_2" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_2" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (2);
CREATE TABLE "ClientSnapshot_3" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_3" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (3);
CREATE TABLE "ClientSnapshot_4" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_4" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (4);
CREATE TABLE "ClientSnapshot_5" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_5" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (5);
CREATE TABLE "ClientSnapshot_6" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_6" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (6);
CREATE TABLE "ClientSnapshot_7" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_7" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (7);
CREATE TABLE "ClientSnapshot_8" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_8" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (8);
CREATE TABLE "ClientSnapshot_9" PARTITION OF "ClientSnapshot" (CONSTRAINT "PK_ClientSnapshot_9" PRIMARY KEY ("DatasetId", "ClientId")) FOR VALUES IN (9);

CREATE TABLE "DepositSnapshot"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "DepositId" int not null,
	"Value" numeric(19, 4) not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "DepositSnapshot_1" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_1" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (1);
CREATE TABLE "DepositSnapshot_2" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_2" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (2);
CREATE TABLE "DepositSnapshot_3" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_3" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (3);
CREATE TABLE "DepositSnapshot_4" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_4" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (4);
CREATE TABLE "DepositSnapshot_5" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_5" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (5);
CREATE TABLE "DepositSnapshot_6" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_6" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (6);
CREATE TABLE "DepositSnapshot_7" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_7" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (7);
CREATE TABLE "DepositSnapshot_8" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_8" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (8);
CREATE TABLE "DepositSnapshot_9" PARTITION OF "DepositSnapshot" (CONSTRAINT "PK_DepositSnapshot_9" PRIMARY KEY ("DatasetId", "ClientId", "DepositId")) FOR VALUES IN (9);

CREATE TABLE "LoanSnapshot"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "LoanId" int not null,
	"Value" numeric(19, 4) not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "LoanSnapshot_1" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_1" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (1);
CREATE TABLE "LoanSnapshot_2" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_2" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (2);
CREATE TABLE "LoanSnapshot_3" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_3" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (3);
CREATE TABLE "LoanSnapshot_4" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_4" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (4);
CREATE TABLE "LoanSnapshot_5" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_5" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (5);
CREATE TABLE "LoanSnapshot_6" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_6" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (6);
CREATE TABLE "LoanSnapshot_7" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_7" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (7);
CREATE TABLE "LoanSnapshot_8" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_8" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (8);
CREATE TABLE "LoanSnapshot_9" PARTITION OF "LoanSnapshot" (CONSTRAINT "PK_LoanSnapshot_9" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")) FOR VALUES IN (9);
