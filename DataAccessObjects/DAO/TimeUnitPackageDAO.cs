using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class TimeUnitPackageDAO
    {
        public TimeUnitPackageDAO(RentNestSystemContext context) : base(context) { }
        public async Task<TimeUnitPackage> GetTimeUnitById(int timeUnitId)
        {
            return await _context.TimeUnitPackages.FirstOrDefaultAsync(t => t.TimeUnitId == timeUnitId);
        }
    }
}