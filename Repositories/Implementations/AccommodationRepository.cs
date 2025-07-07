﻿using BusinessObjects.Domains;
using RentNest.Infrastructure.DataAccess;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class AccommodationRepository : IAccommodationRepository
    {
        private readonly AccommodationDAO _accommodationDAO;

        public AccommodationRepository(AccommodationDAO accommodationDAO)
        {
            _accommodationDAO = accommodationDAO;
        }

        public Task<string> GetAccommodationImage(int accommodationId)
        {
            return _accommodationDAO.GetAccommodationImage(accommodationId);
        }

        public Task<string> GetAccommodationType(int accommodationId)
        {
            return _accommodationDAO.GetAccommodationType((int)accommodationId);
        }
        public Task<List<Post>> GetAccommodationsBySearchDto(string provinceName, string districtName, string wardName, double? area, decimal? minMoney, decimal? maxMoney)
        {
            return _accommodationDAO.GetAccommodationsBySearchDto(provinceName, districtName, wardName, area, minMoney, maxMoney);
        }
    }
}
