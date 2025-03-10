﻿using Models.DTOs;

namespace Repos.Interfaces
{
    public interface IUserRepo
    {
        Task<int> CreateAsync(User user);

        Task UpdateAsync(User user);

        int UpdateLastUpdate(DateTime lastUpdate, int uid);
        //Task<int> GetUidAsync();
        Task<User?> GetUserLocalAsync();
    }
}