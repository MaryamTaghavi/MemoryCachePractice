using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace PracticeMemoryCache;

public class HomeController : Controller
{
    //In-Memory-Caching / LocalCaching
    private readonly IMemoryCache  _cache;
    public HomeController(IMemoryCache  cache)
    {
        _cache = cache;
    }
    
    public IActionResult Set1()
    {
        _cache.Set("CacheKey", "Hello World" , TimeSpan.FromDays(1));
        return View();
    }
    
    public IActionResult Get()
    {
        object data = _cache.Get("CacheKey");
        return View(data);
    }
    
    public IActionResult Set2()
    {
        DateTime data;
        // Look for cache key.
        if (!_cache.TryGetValue( "CacheKey" , out data))
        {
            // Key not in cache, so get data.
            data= DateTime.Now;

            // Save data in cache and set the relative expiration time to one day
            _cache.Set( "CacheKey" , data, TimeSpan.FromDays(1));
        }
        return View(data);
    }
    
    public IActionResult GetOrCreate()
    {
        var data = _cache.GetOrCreate( "CacheKey" , entry =>
            entry.SlidingExpiration = TimeSpan.FromSeconds(3));
        return View(data);
    }

    public IActionResult SetOptions()
    {
        //دور روش برای expiretime شدن کش وجود دارد:
        //1- absolute expiration به این معنی است که یک زمان قطعی را برای منقضی شدن کش ها مشخص میکند به عبارتی میگوییم کش با کلید فلان در تاریخ و ساعت فلان حذف شود.
        //2- sliding expiration یک بازه زمانی برای منقضی شدن کش ها مشخص میکنیم یعنی میگوییم بعد از گذشت فلان دقیقه از ایجاد کش ، آن را حذف کن.
        
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
        options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
        options.SlidingExpiration = TimeSpan.FromMinutes(1);
        _cache.Set("CacheKey", "HelloWorld!", options );

        return View();
    }

    public IActionResult LoadLevels()
    {
        //یکی از روش های مدیریت حافظه Ram در کش ها این است که برای حذف شدن کش ها از حافظه اولویت بندی هایی تعریف کنید. اولویت ها در چهار سطح قابل دسترسی است: Low = 0 / Normal = 1 / High = 2/ NeverRemove = 3
        // این الویت بندی ها زمانی کاربرد خواهد داشت که حافظه اختصاصی Ram برای کش ها پر شده باشد و در این حالت سیستم کشینگ بصورت خودمختار کش های با الویت پایین را از حافظه حذف میکند و کش ها با الویت ببیشتر در حافظه باقی میماند.
        var data = "Hello Worls" ;
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
        
        options.Priority = CacheItemPriority.High;
        _cache.Set("CacheKey", data, options);

        return View();
    }
}