CREATE TABLE "LoansProcess"
(
    "LoansProcessId" smallint generated always as identity,
    "DatasetId" smallint not null,
    CONSTRAINT "PK_LoansProcess" PRIMARY KEY ("LoansProcessId")
);

CREATE TABLE "LoansProcessClientResult"
(
    "LoansProcessId" smallint not null,
    "ClientId" int not null,
	"TotalLoan" numeric(19, 4) not null
) PARTITION BY LIST ("LoansProcessId");
