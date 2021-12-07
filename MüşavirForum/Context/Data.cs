using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MüşavirForum.Context;
using MüşavirForum.Models;
using System.Web.Helpers;
using System.Data.Entity;


namespace MüşavirForum.Context
{
    public class Data : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {

        protected override void Seed(DatabaseContext context)
        {
            Random r = new Random();
            int token = r.Next();
            string HashDegeri = Crypto.SHA256("123456");

            var users = new List<User>
            {
            new User{Name="Carson",Username=Convert.ToString(token),Email=Convert.ToString("Email"),Password=Convert.ToString(HashDegeri),Token=Convert.ToString(token),Status=Convert.ToInt32(1)},

            };

            users.ForEach(s => users.Add(s));
            context.SaveChanges();

            //    students.ForEach(s => context.Students.Add(s));
            //     context.SaveChanges();

        }


    }
}