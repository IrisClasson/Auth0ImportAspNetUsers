namespace Auth0ImportAspNetUsers {
    // I haven't bothered fixing the class names and properties
    // Use [JsonProperty("description")] etc :)
    public class ImportUser {
        public string Email { get; set; }
        public bool Email_verified { get; set; }
        public string User_id { get; set; }
        public string User_name { get; set; }
        public App_Metadata App_metadata { get; set; }
        public Custom_Password_Hash Custom_password_hash { get; set; }
    }

    public class App_Metadata {
        public string[] Roles { get; set; }
    }

    public class Custom_Password_Hash {
        public string Algorithm { get; set; }
        public Hash Hash { get; set; }
    }

    public class Hash {
        public string Value { get; set; }
        public string Encoding { get; set; }
    }
}
