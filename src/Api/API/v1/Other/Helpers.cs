using API.Infrastructure.Database;

namespace API.v1.Other
{
    public static class Helpers
    {
        public static User getUserOrCreateNew(this Context context,int id)
        {
            var user = context.Users.Find(id);
            if (user == null)
            {
                user = new User() { Id = id };
                context.Users.Add(user);
                context.SaveChanges();
            }
            return user;
        }
    }
}
