namespace rest_api.Libary.Bcrypt
{

    public static class Bcrypt
    {
        static string salt = "$2a$10$ccM8lVPDqy.yvSgyFHGv5u";
        public static string hash(string pass)
        {
            return BCrypt.Net.BCrypt.HashPassword(pass, salt);
        }
        public static bool verify(string pass)
        {
           return  BCrypt.Net.BCrypt.Verify(pass, hash(pass));
        }
    }
}