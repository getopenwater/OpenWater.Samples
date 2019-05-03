using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace SampleWebsite.DataAccess
{
    public class UserTokensService
    {
        private const string USERS_TOKENS_KEY = "_UsersTokens";
        private readonly MemoryCache _cache;

        public UserTokensService()
        {
            _cache = MemoryCache.Default;
        }


        public string AddNewUserToken(User user)
        {
            var usersTokens = GetUsersTokens();
            var userToken = Guid.NewGuid().ToString();
            usersTokens.Add(userToken, user);
            var cacheItem = new CacheItem(USERS_TOKENS_KEY, usersTokens);
            _cache.Add(cacheItem, new CacheItemPolicy() { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(2) });
            return userToken;
        }

        public Dictionary<string, User> GetUsersTokens()
        {
            var usersTokens = _cache.Get(USERS_TOKENS_KEY) as Dictionary<string, User>;
            if (usersTokens == null)
            {
                usersTokens = new Dictionary<string, User>();
            }
            return usersTokens;
        }

        public User GetUserByToken(string token)
        {
            var usersTokens = GetUsersTokens();
            if (usersTokens.ContainsKey(token))
            {
                return usersTokens[token];
            }

            return null;
        }
    }
}