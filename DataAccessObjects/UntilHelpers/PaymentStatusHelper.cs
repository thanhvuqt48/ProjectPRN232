using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RentNest.Core.Enums;

namespace DataAccessObjects.UntilHelpers
{
    public static class PaymentStatusHelper
    {
        public static string ToDbValue(this PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Success => "S",
                PaymentStatus.Pending => "P",
                PaymentStatus.Failed => "F",
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unsupported payment status: {status}")
            };
        }

        public static PaymentStatus FromDbValue(string value)
        {
            return value switch
            {
                "S" => PaymentStatus.Success,
                "P" => PaymentStatus.Pending,
                "F" => PaymentStatus.Failed,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported payment status value: {value}")
            };
        }
    }
}