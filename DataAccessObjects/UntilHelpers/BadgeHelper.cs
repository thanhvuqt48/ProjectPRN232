using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RentNest.Core.Enums;

namespace DataAccessObjects.UntilHelpers
{
    public static class BadgeHelper
    {
        public static string GetBadgeClass(string packageTypeName)
        {
            return packageTypeName switch
            {
                "VIP Kim Cương" => "bg-danger",
                "VIP Vàng" => "bg-warning",
                "VIP Bạc" => "bg-info",
                _ => ""
            };
        }
        public static PackageTypeEnum ParsePackageType(string? typeName)
        {
            return typeName switch
            {
                "VIP Kim Cương" => PackageTypeEnum.Diamond,
                "VIP Vàng" => PackageTypeEnum.Gold,
                "VIP Bạc" => PackageTypeEnum.Silver,
                _ => PackageTypeEnum.Normal
            };
        }
        public static bool IsVip(PackageTypeEnum type) => type != PackageTypeEnum.Normal;
    }
}