using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Domains;
using BusinessObjects.Dtos;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _accountRepository.GetSystemAccountByEmailAsync(email);
        }
        public async Task<bool> UpdateAccountAsync(int id, Account updatedAccount)
        {
            var acc = await _accountRepository.GetByIdAsync(id);
            if (acc == null) return false;
            acc.Email = updatedAccount.Email;
            acc.Username = updatedAccount.Username;
            // ... các trường khác
            await _accountRepository.UpdateAsync(acc);
            await _accountRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var acc = await _accountRepository.GetByIdAsync(id);
            if (acc == null) return false;
            _accountRepository.Delete(acc);
            await _accountRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        => await _accountRepository.GetAllAsync();

        public async Task<Account> CreateExternalAccountAsync(ExternalAccountRegisterDto dto)
        {
            return await _accountRepository.CreateExternalAccountAsync(dto);
        }
    }
}