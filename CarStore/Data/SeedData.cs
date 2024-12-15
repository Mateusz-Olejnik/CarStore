using CarStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarStore.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CarStoreContext(
                serviceProvider.GetRequiredService<DbContextOptions<CarStoreContext>>()))
            {
                // sprawdza cars tabela ma jakies dane w sobie 
                if (!context.Cars.Any())
                {
                    // przykladowe modele nie jest to istotne 
                    context.Cars.AddRange(
                        new Car
                        {
                            Brand = "Toyota",
                            Model = "Camry",
                            Price = 25000,
                            Description = "Niezawodny i oszczędny sedan.",
                            ImageUrl = "https://scene7.toyota.eu/is/image/toyotaeurope/CAM0001a_24_WEB:Large-Landscape?ts=1720540656193&resMode=sharp2&op_usm=1.75,0.3,2,0&fmt=png-alpha"
                        },
                        new Car
                        {
                            Brand = "Honda",
                            Model = "Civic",
                            Price = 22000,
                            Description = "Kompaktowy samochód znany ze swojej niezawodności.",
                            ImageUrl = "https://d-art.ppstatic.pl/kadry/k/r/1/24/3b/6256bab9b421b_o_original.jpg"
                        },
                        new Car
                        {
                            Brand = "Ford",
                            Model = "Mustang",
                            Price = 35000,
                            Description = "Klasyczny amerykański samochód typu muscle o potężnych osiągach.",
                            ImageUrl = "https://www.gpas-cache.ford.com/guid/d5523d74-629a-3de5-89b0-7ae61d072aad.png"
                        },
                        new Car
                        {
                            Brand = "Peugeot",
                            Model = "508",
                            Price = 32000,
                            Description = "Francuskie auto z mało awaryjnym silnikiem HDI",
                            ImageUrl = "https://www.fleetmarket.pl/wp-content/uploads/2019/11/Peugeot-508_SW-2019-1600-01.jpg"
                        },
                        new Car
                        {
                            Brand = "Nissan",
                            Model = "GT-R",
                            Price = 50000,
                            Description = "Japońska marka, której nie trzeba nikomu przedstawiać",
                            ImageUrl = "https://blog.pgd.pl/wp-content/uploads/GTR-1-scaled.jpeg"
                        }

                    );

                    context.SaveChanges();
                }

                // dodanie ADMIMNA ktory jako jedyny ma dostep do edycji/dodawania/usuwania aut, dodany do identity database
                var userManager = serviceProvider.GetRequiredService<UserManager<DefaultUser>>();
                // kolejno login, haslo
                var adminEmail = "admin@wp.pl";
                var adminPassword = "Admin123!";

                if (userManager.FindByEmailAsync(adminEmail).Result == null)
                {
                    var adminUser = new DefaultUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(adminUser, adminPassword).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception("Nie udało się utworzyć konta admina: " +
                                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}
