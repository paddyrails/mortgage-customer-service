using Microsoft.EntityFrameworkCore;
using Customer.API.Models;

namespace Customer.API.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Customer> Customers => Set<Models.Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Employment> Employments => Set<Employment>();
    public DbSet<CreditHistory> CreditHistories => Set<CreditHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Models.Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.SSN).IsUnique();

            entity.HasOne(e => e.Address)
                .WithOne(a => a.Customer)
                .HasForeignKey<Address>(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Employments)
                .WithOne(emp => emp.Customer)
                .HasForeignKey(emp => emp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreditHistory)
                .WithOne(c => c.Customer)
                .HasForeignKey<CreditHistory>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var customerId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var customerId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<Models.Customer>().HasData(
            new Models.Customer
            {
                Id = customerId1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Phone = "+1-555-0101",
                SSN = "123-45-6789",
                DateOfBirth = new DateTime(1985, 5, 15),
                CreatedAt = DateTime.UtcNow
            },
            new Models.Customer
            {
                Id = customerId2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                Phone = "+1-555-0102",
                SSN = "987-65-4321",
                DateOfBirth = new DateTime(1990, 8, 22),
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
                CustomerId = customerId1,
                Street = "123 Main Street",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            },
            new Address
            {
                Id = Guid.Parse("aaaa2222-2222-2222-2222-222222222222"),
                CustomerId = customerId2,
                Street = "456 Oak Avenue",
                Unit = "Apt 2B",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Employment>().HasData(
            new Employment
            {
                Id = Guid.Parse("bbbb1111-1111-1111-1111-111111111111"),
                CustomerId = customerId1,
                EmployerName = "Tech Corp Inc",
                JobTitle = "Software Engineer",
                EmploymentType = EmploymentType.FullTime,
                AnnualIncome = 120000,
                StartDate = new DateTime(2018, 3, 1),
                IsCurrent = true,
                CreatedAt = DateTime.UtcNow
            },
            new Employment
            {
                Id = Guid.Parse("bbbb2222-2222-2222-2222-222222222222"),
                CustomerId = customerId2,
                EmployerName = "Finance Plus LLC",
                JobTitle = "Financial Analyst",
                EmploymentType = EmploymentType.FullTime,
                AnnualIncome = 95000,
                StartDate = new DateTime(2019, 6, 15),
                IsCurrent = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<CreditHistory>().HasData(
            new CreditHistory
            {
                Id = Guid.Parse("cccc1111-1111-1111-1111-111111111111"),
                CustomerId = customerId1,
                CreditScore = 750,
                ReportDate = DateTime.UtcNow.AddDays(-30),
                CreditBureau = "Experian",
                TotalDebt = 25000,
                AvailableCredit = 50000,
                NumberOfAccounts = 5,
                LatePayments = 0,
                Bankruptcies = 0,
                Foreclosures = 0,
                CreatedAt = DateTime.UtcNow
            },
            new CreditHistory
            {
                Id = Guid.Parse("cccc2222-2222-2222-2222-222222222222"),
                CustomerId = customerId2,
                CreditScore = 680,
                ReportDate = DateTime.UtcNow.AddDays(-15),
                CreditBureau = "TransUnion",
                TotalDebt = 35000,
                AvailableCredit = 40000,
                NumberOfAccounts = 7,
                LatePayments = 1,
                Bankruptcies = 0,
                Foreclosures = 0,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
