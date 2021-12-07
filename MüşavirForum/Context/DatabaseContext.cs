using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MüþavirForum.Models;
using System.Web.Helpers;

namespace MüþavirForum.Context
{
    public class DatabaseContext : DbContext
    {


        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new SampleDatabaseContextInitializer());

        }
    }



    public class SampleDatabaseContextInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            Random r = new Random();
            int token = r.Next();
            string HashDegeri = Crypto.SHA256("123456");

            /*   var users = new List<User>
               {
               new User{Name="Carson",Username=Convert.ToString(token),Email=Convert.ToString("Email"),Password=Convert.ToString(HashDegeri),Token=Convert.ToString(token),Status=Convert.ToInt32(1)}

               };
               */

            DatabaseContext db = new DatabaseContext();

          

            db.Users.Add(new User
            {
                Name = Convert.ToString("Carson"),
                Username = Convert.ToString(token),
                Email = Convert.ToString("Email"),
                Password = Convert.ToString(HashDegeri),
                Token = Convert.ToString(token),
                Status = Convert.ToInt32(1)
            });

            db.Categories.Add(new Category
            {
                Name = Convert.ToString("Kategori1"),
                Description = Convert.ToString("Kategori1"),
                Seo = Convert.ToString("Kategori1"),
                ParentId = Convert.ToInt32(0),
                Status = Convert.ToInt32(1)
            }           
            );


            db.Categories.Add(new Category
            {
                Name = Convert.ToString("Kategori2"),
                Description = Convert.ToString("Kategori2"),
                Seo = Convert.ToString("Kategori2"),
                ParentId = Convert.ToInt32(0),
                Status = Convert.ToInt32(1)
            }
           );

            db.Categories.Add(new Category
            {
                Name = Convert.ToString("Kategori3"),
                Description = Convert.ToString("Kategori3"),
                Seo = Convert.ToString("Kategori3"),
                ParentId = Convert.ToInt32(0),
                Status = Convert.ToInt32(1)
            }
           );

      /*      db.Topics.Add(
  new Topic
  {
      CategoryId = Convert.ToInt32(1),
      UserId = Convert.ToInt32(1),
      Content = Convert.ToString("Konu KOnu Konu Konu KOnu KOnuKOnuKOnuKOnuKOnuKOnuKOnuKOnu KOnu"),
      Date = Convert.ToString("aaa"),
      Status = Convert.ToInt32(1),
      Seo = Convert.ToString("konu-konu")
  }
  );

            db.Topics.Add(new Topic
            {
                CategoryId = Convert.ToInt32(1),
                UserId = Convert.ToInt32(1),
                Content = Convert.ToString("Konu2 Konu2 Konu2 Konu2 Konu2Konu2Konu2Konu2Konu2Konu2Konu2Konu2Konu2Konu2"),
                Date = Convert.ToString("00000000"),
                Status = Convert.ToInt32(1),
                Seo = Convert.ToString("konu2-konu2")

            }
               );


            db.Topics.Add(new Topic
            {
                CategoryId = Convert.ToInt32(1),
                UserId = Convert.ToInt32(1),
                Content = Convert.ToString("Konu3 Konu3 Konu3 Konu3 Konu3Konu3Konu3Konu3Konu3Konu3Konu3Konu3"),
                Date = Convert.ToString("00000000"),
                Status = Convert.ToInt32(1),
                Seo = Convert.ToString("konu3-konu3")

            });

            */

            db.SaveChanges();





        }
    }
}
