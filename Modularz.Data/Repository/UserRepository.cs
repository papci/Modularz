using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Modularz.Data.EF;
using UnitSense.CacheManagement;
using UnitSense.Extensions.Encryption;
using UnitSense.Extensions.Extensions;
using UnitSense.Repositories.Abstractions;

namespace Modularz.Data.Repository;

public class UserRepository : DataRepository<BlogDbContext, BlogUser, int, string>
{
    public UserRepository(RedisCacheManager redisCacheManager, LocalCacheManager localCacheManager, BlogDbContext dbCtx,
        RedisBusHandler busHandler, RepositorySetup setup) : base(redisCacheManager, localCacheManager, dbCtx,
        busHandler, typeof(BlogUser), setup)
    {
    }

    public override Task<BlogUser> GetByIdAsync(int key)
    {
        return FindDataAsync(GetPrimKeyValue(key), () => dbContext.BlogUsers.FirstOrDefaultAsync(x => x.Id == key));
    }

    public override Task<BlogUser> GetBySecondaryAsync(string key)
    {
        return FindDataAsync(GetSecondaryKeyValue(key),
            () => dbContext.BlogUsers.FirstOrDefaultAsync(x => x.AccountName == key));
    }

    public override Task PutAsync(BlogUser data)
    {
        throw new NotSupportedException("Cannot use this method since users should be created with a lot of controls");
    }

    /// <summary>
    /// Create a pending user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public async Task<UserCreationResult> CreateAsync(string userName, string email, string password)
    {
        if (UserNameExists(userName))
        {
            return new UserCreationResult()
            {
                Message = "UserName not available.",
                Success = false,
                User = null
            };
        }

        if (EmailExists(email))
        {
            return new UserCreationResult()
            {
                Message = "Email not available.",
                Success = false,
                User = null
            };
        }
        BlogUser user = new BlogUser()
        {
            Email = email,
            AccountName = userName,
            Salt = StringExt.Randomize(6),
            DateCreated = DateTime.UtcNow,
            State = BlogUser.UserState.Pending,
            Rank = BlogUser.UserRank.User,
            TempToken = HashEncryption.SHA256Hash($"{DateTime.UtcNow}")
        };

        user.HashedPassword = HashEncryption.SHA256Hash($"{user.Salt}{password}");
        dbContext.BlogUsers.Add(user);
        await dbContext.SaveChangesAsync();
        await WriteAllToCache(user);
        return new UserCreationResult()
        {
            Message = string.Empty,
            Success = true,
            User = user
        };
    }

    public async Task SetAsAdmin(int id)
    {
        var admin = await dbContext.BlogUsers.FirstOrDefaultAsync(x => x.Id == id);
        if (admin != null)
        {
            admin.Rank = BlogUser.UserRank.Admin;
            await dbContext.SaveChangesAsync();
            await WriteAllToCache(admin);
        }
        
    }

    private bool EmailExists(string email)
    {
        return dbContext.BlogUsers.Any(x => x.Email == email);
    }

    private bool UserNameExists(string userName)
    {
        return dbContext.BlogUsers.Any(x => x.AccountName == userName);
    }

    public override async Task RefreshAsync(int key)
    {
        var dbItem = await dbContext.BlogUsers.FirstOrDefaultAsync(x => x.Id == key);
        await WriteAllToCache(dbItem);
    }

    public override async Task DeleteAsync(int key)
    {
        var dbitem = await dbContext.BlogUsers.FirstOrDefaultAsync(x => x.Id == key);
        var cacheTask = DeleteAllFromCache(dbitem);
        dbContext.BlogUsers.Remove(dbitem);
        var dbTask = dbContext.SaveChangesAsync();
        await Task.WhenAll(cacheTask, dbTask);
    }

    public override async Task UpdateAsync(BlogUser data)
    {
        var dbItem = await dbContext.BlogUsers.FirstOrDefaultAsync(x => x.Id == data.Id);
        dbItem.CopyPropertiesFrom(data);
        var dbTask = dbContext.SaveChangesAsync();
        var cacheTask = WriteAllToCache(data);
        await Task.WhenAll(dbTask, cacheTask);
    }

    protected override async Task WriteAllToCache(BlogUser data)
    {
        var t1 = WriteCacheDataAsync(GetPrimKeyValue(data.Id), data);
        var t2 = WriteCacheDataAsync(GetSecondaryKeyValue(data.AccountName), data);
        await Task.WhenAll(t1, t2);
    }

    protected override Task DeleteAllFromCache(BlogUser data)
    {
        var t1 = DeleteCacheDataAsync(GetPrimKeyValue(data.Id), data);
        var t2 = DeleteCacheDataAsync(GetSecondaryKeyValue(data.AccountName), data);
        return Task.WhenAll(t1, t2);
    }
    
}

public record UserCreationResult
{
    public bool Success { get; set; }
    public BlogUser? User { get; set; }
    public string Message { get; set; }
}