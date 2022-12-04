using Angular_Project_Foodhub_Work_02.Models;

namespace Angular_Project_Foodhub_Work_02.HostedService
{
    public class DbSeederHostedService: IHostedService
    {
        IServiceProvider serviceProvider;
        public DbSeederHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(IServiceScope scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
                await SeedDbAsync(db);
            }
        }
        public async Task SeedDbAsync(FoodDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            if (!db.Customers.Any())
            {
                var c1 = new Customer { CustomerName = "Jobaida Tasmin", Email = "jab@gmail.com", Phone = "01xxxxx", Address = "Mirpur, Dhaka" };
                await db.Customers.AddAsync(c1);
                var c2 = new Customer { CustomerName = "Maksuda Yesmin", Email = "mak@gmail.com", Phone = "01xxxxx", Address = "Mirpur, Dhaka" };
                await db.Customers.AddAsync(c2);
                var fd1= new Food { FoodName="Chawmin", IsAvailable=true, Price = 130.00M, Picture="1.jpg" };
                await db.Foods.AddAsync(fd1);
                var fd2 = new Food { FoodName = "Burger", IsAvailable = true, Price = 125.00M, Picture ="12.jpg" };
                await db.Foods.AddAsync(fd2);
                var o1 = new Order { OrderDate= DateTime.Today.AddDays(-5), DeliveryDate= DateTime.Today.AddDays(-1), Customer= c1, Status= Status.Pending  };
                o1.OrderDetails.Add(new OrderDetail { Order = o1, Food = fd1, Quantity = 3 });
                var o2 = new Order { OrderDate = DateTime.Today.AddDays(-5), DeliveryDate = DateTime.Today.AddDays(-1), Customer = c2, Status = Status.Pending };
                o2.OrderDetails.Add(new OrderDetail { Order = o2, Food = fd1, Quantity = 1 });
                o2.OrderDetails.Add(new OrderDetail { Order = o2, Food = fd2, Quantity = 1 });
                await db.Orders.AddAsync(o1);
                await db.Orders.AddAsync(o2);
                await db.SaveChangesAsync();
            }
            
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
