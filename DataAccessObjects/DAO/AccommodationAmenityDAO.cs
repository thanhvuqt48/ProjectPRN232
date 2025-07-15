using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Domains;
using DataAccessObjects.DB;

namespace DataAccessObjects.DAO
{
    public class AccommodationAmenityDAO : BaseDAO<AccommodationAmenity>
    {
        public AccommodationAmenityDAO(RentNestSystemContext context) : base(context) { }
    }
}