using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Domains;
using DataAccessObjects.DB;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAO
{
    public class PostPackageDetailDAO : BaseDAO<PostPackageDetail>
    {
        public PostPackageDetailDAO(RentNestSystemContext context) : base(context) { }
        public async Task<PostPackageDetail?> GetByPostIdAsync(int postId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }
    }
}