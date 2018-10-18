CREATE TABLE "Dataset"
(
    "DatasetId" smallint generated always as identity,
	"Date" date not null,
    CONSTRAINT "PK_Dataset" PRIMARY KEY ("DatasetId")
);

CREATE TABLE "ClientSnapshot"
(
    "DatasetId" smallint not null,
    "ClientId" int not null,
	CONSTRAINT "PK_ClientSnapshot" PRIMARY KEY ("DatasetId", "ClientId"),
	CONSTRAINT "FK_ClientSnapshot_Dataset" FOREIGN KEY ("DatasetId") REFERENCES "Dataset" ("DatasetId")
);

CREATE TABLE "LoanSnapshot"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "LoanId" int not null,
	"Value" numeric(19, 4) not null,
	CONSTRAINT "PK_LoanSnapshot" PRIMARY KEY ("DatasetId", "ClientId", "LoanId"),
	CONSTRAINT "FK_LoanSnapshot_ClientSnapshot" FOREIGN KEY ("DatasetId", "ClientId") REFERENCES "ClientSnapshot" ("DatasetId", "ClientId")
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
	"TotalLoan" numeric(19, 4) not null,
	CONSTRAINT "PK_ClientTotalLoan" PRIMARY KEY ("ClientLoansCalculationId", "ClientId"),
	CONSTRAINT "FK_ClientTotalLoan_ClientLoansCalculation" FOREIGN KEY ("ClientLoansCalculationId") REFERENCES "ClientLoansCalculation" ("ClientLoansCalculationId")
);
