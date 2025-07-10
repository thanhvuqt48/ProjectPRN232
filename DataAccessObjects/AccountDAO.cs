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
    public class AccountDAO : BaseDAO<Account>
    {
        public AccountDAO(RentNestSystemContext context) : base(context) { }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _dbSet.Include(u => u.UserProfile).FirstOrDefaultAsync(a => a.Email == email);
        }
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(a => a.Email == email);
        }
        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(a => a.Username == username);
        }
        public async Task<Account?> GetAccountByEmailOrUsernameAsync(string input)
        {
            return await _context.Accounts
                 .Include(a => a.UserProfile)
                .FirstOrDefaultAsync(a => a.Email.ToLower() == input.ToLower() || a.Username.ToLower() == input.ToLower());
        }

    }
}
