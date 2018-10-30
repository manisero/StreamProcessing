CREATE TABLE "TotalLoanCalculation"
(
    "TotalLoanCalculationId" smallint generated always as identity,
	"DatasetId" smallint not null,
	"TotalLoan" numeric null,
	CONSTRAINT "PK_TotalLoanCalculation" PRIMARY KEY ("TotalLoanCalculationId"),
	CONSTRAINT "FK_TotalLoanCalculation_Dataset" FOREIGN KEY ("DatasetId") REFERENCES "Dataset" ("DatasetId")
);

CREATE TABLE "ClientLoansCalculation"
(
    "ClientLoansCalculationId" smallint generated always as identity,
    "DatasetId" smallint not null,
    CONSTRAINT "PK_ClientLoansCalculation" PRIMARY KEY ("ClientLoansCalculationId"),
	CONSTRAINT "FK_ClientLoansCalculation_Dataset" FOREIGN KEY ("DatasetId") REFERENCES "Dataset" ("DatasetId")
);

CREATE TABLE "ClientTotalLoan"
(
    "ClientLoansCalculationId" smallint not null,
    "ClientId" int not null,
	"TotalLoan" numeric not null
) PARTITION BY LIST ("ClientLoansCalculationId");

CREATE TABLE "MaxLossCalculation"
(
    "MaxLossCalculationId" smallint generated always as identity,
	"DatasetId" smallint not null,
	"MaxLoss" numeric null,
	CONSTRAINT "PK_MaxLossCalculation" PRIMARY KEY ("MaxLossCalculationId"),
	CONSTRAINT "FK_MaxLossCalculation_Dataset" FOREIGN KEY ("DatasetId") REFERENCES "Dataset" ("DatasetId")
);
