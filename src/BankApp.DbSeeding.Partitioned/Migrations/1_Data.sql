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

CREATE TABLE "DepositSnapshot"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "DepositId" int not null,
	"Value" numeric not null
) PARTITION BY LIST ("DatasetId");

CREATE TABLE "LoanSnapshot"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "LoanId" int not null,
	"Value" numeric not null
) PARTITION BY LIST ("DatasetId");
