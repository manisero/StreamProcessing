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
	"TotalLoan" numeric(19, 4) not null
) PARTITION BY LIST ("ClientLoansCalculationId");
