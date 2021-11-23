namespace Auth0ImportAspNetUsers {
    public class UserAccountMock : IUserAccount {
        public UserAccountMock(string passwordHash) => PasswordHash = passwordHash;
        public string PasswordHash { get; private set; }
    }
    public class UserAccount : IUserAccount {
        public string PasswordHash { get; private set; }
    }
    interface IUserAccount {
        string PasswordHash { get; }
    }

}
