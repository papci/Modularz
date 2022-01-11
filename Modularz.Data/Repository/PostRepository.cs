using MessagePack;
using Microsoft.EntityFrameworkCore;
using Modularz.Data.EF;
using UnitSense.CacheManagement;
using UnitSense.Repositories.Abstractions;
using UnitSense.Repositories.Abstractions.Filters;

namespace Modularz.Data.Repository;

public class PostRepository : DataRepository<BlogDbContext, BlogPost, int, string>
{
    public PostRepository(RedisCacheManager redisCacheManager, LocalCacheManager localCacheManager, BlogDbContext dbCtx,
        RedisBusHandler busHandler, RepositorySetup setup) : base(redisCacheManager, localCacheManager, dbCtx,
        busHandler, typeof(BlogPost), setup)
    {
    }

    public override Task<BlogPost> GetByIdAsync(int key)
    {
        return FindDataAsync(GetPrimKeyValue(key), () => dbContext.Posts.FirstOrDefaultAsync(x => x.Id == key));
    }

    public override Task<BlogPost> GetBySecondaryAsync(string key)
    {
        return FindDataAsync(GetSecondaryKeyValue(key),
            () => dbContext.Posts.FirstOrDefaultAsync(x => x.SeoUrl == key));
    }

    public override async Task PutAsync(BlogPost data)
    {
        dbContext.Posts.Add(data);
        await dbContext.SaveChangesAsync();
        await WriteAllToCache(data);
    }

    public override async Task RefreshAsync(int key)
    {
        var dbArticle = await dbContext.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == key);
        await WriteAllToCache(dbArticle);
    }

    public override async Task DeleteAsync(int key)
    {
        var keyData = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == key);
        dbContext.Posts.Remove(keyData);
        await dbContext.SaveChangesAsync();
        await DeleteAllFromCache(keyData);
    }

    public override async Task UpdateAsync(BlogPost data)
    {
        var dbItem = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == data.Id);
        if (dbItem == null)
            return;

        dbItem.CopyPropertiesFrom(data,
            new string[]
            {
            });

        await dbContext.SaveChangesAsync();
        SetPubData(dbItem);
        await dbContext.SaveChangesAsync();
        await RefreshAsync(dbItem.Id);
    }

    private void SetPubData(BlogPost dbItem)
    {
        if (dbItem.State == BlogPost.BlogState.Published && string.IsNullOrWhiteSpace(dbItem.SeoUrl))
        {
            dbItem.SeoUrl = dbItem.CreateSeoUrl();
        }

        if (dbItem.State == BlogPost.BlogState.Published && dbItem.DatePublished == DateTime.MinValue)
        {
            dbItem.DatePublished = DateTime.UtcNow;
            
        }
        
        dbItem.DateUpdated = DateTime.UtcNow;
    }

    protected override async Task WriteAllToCache(BlogPost data)
    {
        await WriteCacheDataAsync(GetPrimKeyValue(data.Id), data);
        await WriteCacheDataAsync(GetSecondaryKeyValue(data.SeoUrl), data);
    }

    protected override async Task DeleteAllFromCache(BlogPost data)
    {
        await DeleteCacheDataAsync(GetPrimKeyValue(data.Id), data);
        await DeleteCacheDataAsync(GetSecondaryKeyValue(data.SeoUrl), data);
    }
}
[MessagePackObject()]
public class PostQueryFilter : RawFilter, IQueryFilter<BlogDbContext, BlogPost>
{
    [Key(0)]
    public BlogPost.BlogState? State { get; set; }
    [Key(1)]
    public OrderWay Way { get; set; }
    [Key(2)]
    public OrderField? OrderField { get; set; }
    public async Task<FilteredDataSetResult<BlogPost>> CreateGenTask(BlogDbContext dbContext)
    {
        var query = dbContext.Posts.AsQueryable();
        if (State.HasValue)
        {
            query = query.Where(x => x.State == State);
        }
        
        var skipValue = (Page - 1) * Nb;
        var results = new FilteredDataSetResult<BlogPost>
        {
            NbPerPage = Nb,
            CurrentPage = Page,
            TotalItems = await query.CountAsync()
        };
        switch (OrderField)
        {
         
            case Repository.OrderField.DatePublished:
                query = Way == OrderWay.Asc
                    ? query.OrderBy(x => x.DatePublished)
                    : query.OrderByDescending(x => x.DatePublished);
                break;
       
            default:
                query = Way == OrderWay.Asc ? query.OrderBy(x => x.DateCreated)
                    : query.OrderByDescending(x => x.DateCreated);
                break;
        }
        results.MaxPage = Convert.ToInt32(Math.Ceiling(results.TotalItems / (double)Nb));
        results.Results = await query.Skip(skipValue).Take(Nb).OrderByDescending(x=> x.DateCreated).ToListAsync();

        return results;
    }
    
   public  string GetUniqueKey() => "filtered:" + BitConverter.ToString(MessagePackSerializer.Serialize(this));
   


}
public enum OrderField
{
    DateCreated,
    DatePublished,
    DateReleased
}