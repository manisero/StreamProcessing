﻿namespace Manisero.StreamProcessing.Domain
{
    public class Loan
    {
        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int LoanId { get; set; }

        public decimal Value { get; set; }
    }
}
