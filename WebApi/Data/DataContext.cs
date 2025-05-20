using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
    }
}
