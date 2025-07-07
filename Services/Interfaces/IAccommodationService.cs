using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Domains;

namespace Service.Interfaces
{
    public interface IAccommodationService
    {
        Task<List<Post>> GetAccommodationsBySearchDto(
            string provinceName,
            string districtName,
            string wardName,
            double? area,
            decimal? minMoney,
            decimal? maxMoney);
        Task<string> GetAccommodationImage(int accommodationId);
        Task<string> GetAccommodationType(int accommodationId);

    }

}