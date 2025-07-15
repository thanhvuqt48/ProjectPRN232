using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RentNest.Core.Enums;

namespace DataAccessObjects.UntilHelpers
{
    public static class PostStatusHelper
    {
        public static string ToDbValue(this PostStatus status)
        {
            return status switch
            {
                PostStatus.Pending => "P",
                PostStatus.Active => "A",
                PostStatus.Rejected => "R",
                PostStatus.Unpaid => "U",
                PostStatus.Expired => "E",
                PostStatus.Cancelled => "C",
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unsupported status: {status}")
            };
        }

        public static PostStatus FromDbValue(string value)
        {
            return value switch
            {
                "P" => PostStatus.Pending,
                "A" => PostStatus.Active,
                "R" => PostStatus.Rejected,
                "U" => PostStatus.Unpaid,
                "E" => PostStatus.Expired,
                "C" => PostStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported status value: {value}")
            };
        }

        public static string ToDisplay(this PostStatus status)
        {
            return status switch
            {
                PostStatus.Pending => "CHỜ DUYỆT",
                PostStatus.Active => "ĐANG HIỂN THỊ",
                PostStatus.Rejected => "BỊ TỪ CHỐI",
                PostStatus.Unpaid => "CẦN THANH TOÁN",
                PostStatus.Expired => "HẾT HẠN",
                PostStatus.Cancelled => "ĐÃ HỦY",
                _ => string.Empty
            };
        }
    }
}