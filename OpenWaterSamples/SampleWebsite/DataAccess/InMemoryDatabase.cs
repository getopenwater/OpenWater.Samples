using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebsite.DataAccess
{
    public class InMemoryDatabase
    {
        private static ICollection<User> Users = new List<User>();

        public InMemoryDatabase()
        {

        }


        public static void SeedData()
        {
            var user = User.CreateDemoUser();
            lock (((ICollection)Users).SyncRoot)
            {
                Users.Add(user);
            }
        }

        public static User GetUserByEmailAndPassword(string email, string password)
        {
            lock (((ICollection)Users).SyncRoot)
            {
                return Users.FirstOrDefault(u => u.EmailAddress.ToLower() == email.ToLower() && u.Password == password);
            }
        }

        public static User GetUserById(int id)
        {
            lock (((ICollection)Users).SyncRoot)
            {
                return Users.FirstOrDefault(u => u.Id == id);
            }
        }


    }
}