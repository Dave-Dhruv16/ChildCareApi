namespace ChildCareApi.Models
{
    public class AuthModel
    {
        public class UserRegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }
        }

        public class UserRegisterResponse
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int RoleId { get; set; }
            public string Message { get; set; }
        }

        public class UserLoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserLoginResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }
            public int RoleId { get; set; }
            public string FullName { get; set; }
        }
    }
}

