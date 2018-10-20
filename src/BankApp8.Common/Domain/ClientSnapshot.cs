﻿namespace BankApp8.Common.Domain
{
    public class ClientSnapshot
    {
        public const int DefaultReadingBatchSize = 100000;

        public short DatasetId { get; set; }

        public int ClientId { get; set; }
    }
}