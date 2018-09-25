CREATE TABLE "Dataset"
(
    "DatasetId" smallint not null,
    CONSTRAINT "PK_Dataset" PRIMARY KEY ("DatasetId")
);

CREATE TABLE "Client"
(
    "DatasetId" smallint not null,
    "ClientId" int not null,
    CONSTRAINT "PK_Client" PRIMARY KEY ("DatasetId", "ClientId")
);

CREATE TABLE "Loan"
(
    "DatasetId" smallint not null,
	"ClientId" int not null,
    "LoanId" int not null,
	"Value" numeric(19, 4) not null,
    CONSTRAINT "PK_Loan" PRIMARY KEY ("DatasetId", "ClientId", "LoanId")
);
