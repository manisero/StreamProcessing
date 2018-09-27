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

CREATE TABLE "LoansProcessClientResult_1" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_1" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (1);
CREATE TABLE "LoansProcessClientResult_2" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_2" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (2);
CREATE TABLE "LoansProcessClientResult_3" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_3" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (3);
CREATE TABLE "LoansProcessClientResult_4" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_4" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (4);
CREATE TABLE "LoansProcessClientResult_5" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_5" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (5);
CREATE TABLE "LoansProcessClientResult_6" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_6" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (6);
CREATE TABLE "LoansProcessClientResult_7" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_7" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (7);
CREATE TABLE "LoansProcessClientResult_8" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_8" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (8);
CREATE TABLE "LoansProcessClientResult_9" PARTITION OF "LoansProcessClientResult" (CONSTRAINT "PK_LoansProcessClientResult_9" PRIMARY KEY ("LoansProcessId", "ClientId")) FOR VALUES IN (9);
