﻿using BusinessObjects.Domains;
using DataAccessObjects.DB;
using Microsoft.EntityFrameworkCore;
using RentNest.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class UserProfileDAO : BaseDAO<UserProfile>
    {
        public UserProfileDAO(RentNestSystemContext context) : base(context) { }

        public async Task<UserProfile?> GetProfileByAccountIdAsync(int accountId)
        {

            return await _dbSet
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task UpdateProfileAsync(UserProfile updatedProfile)
        {
            var existingProfile = await _dbSet
                .FirstOrDefaultAsync(p => p.AccountId == updatedProfile.AccountId);

            if (existingProfile == null)
            {
                throw new Exception("Profile not found.");
            }

            existingProfile.FirstName = updatedProfile.FirstName;
            existingProfile.LastName = updatedProfile.LastName;
            existingProfile.Gender = updatedProfile.Gender;
            existingProfile.DateOfBirth = updatedProfile.DateOfBirth;
            existingProfile.Address = updatedProfile.Address;
            existingProfile.Occupation = updatedProfile.Occupation;
            existingProfile.PhoneNumber = updatedProfile.PhoneNumber;
            existingProfile.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(updatedProfile.AvatarUrl))
            {
                existingProfile.AvatarUrl = updatedProfile.AvatarUrl;
            }

            existingProfile.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}
