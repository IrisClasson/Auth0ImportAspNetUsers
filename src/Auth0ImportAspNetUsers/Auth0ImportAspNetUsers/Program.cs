using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Auth0ImportAspNetUsers {
    class Program {
        static void Main(string[] args) {

            var userAccount = GenerateExampleUserAccount("mysecretpassword");
            var originalPwdHashBase64 = userAccount.PasswordHash;
            var originalPwdHash = Convert.FromBase64String(originalPwdHashBase64);

            // Regardless of version the prefix is always just one byte
            var aspIdVersionPrefixLength = 1;
            var saltLength = 16;

            // Copy hash salt part of the passwordhash and trim padding at the end for B64 compatability
            // Read more here: https://github.com/P-H-C/phc-string-format/blob/master/phc-sf-spec.md#b64
            var salt = new byte[saltLength];
            // Notice that we have to skip the asp id prefix
            Buffer.BlockCopy(originalPwdHash, aspIdVersionPrefixLength, salt, 0, saltLength);
            //b64 does not have padding like Base64 does
            var saltB64 = Convert.ToBase64String(salt).TrimEnd('=');

            // Copy password part of the passwordhash  and trim padding at the end for B64 compatability
            var passwordHashBytes = new byte[originalPwdHash.Length - (aspIdVersionPrefixLength + saltLength)];
            // Notice that we have to skip the asp id prefix and salt to get password only
            Buffer.BlockCopy(originalPwdHash, aspIdVersionPrefixLength + saltLength, passwordHashBytes, 0, passwordHashBytes.Length);
            var passwordB64 = Convert.ToBase64String(passwordHashBytes).TrimEnd('=');

            var hashValue = $"$pbkdf2-sha1$i=1000,l=32${saltB64}${passwordB64}";

            var importUser = new ImportUser {
                User_name = "admin",
                User_id = "1",
                Email = "admin@admin.se",
                Email_verified = true,
                App_metadata = new App_Metadata { Roles = new string[] { "admin" } },
                Custom_password_hash = new Custom_Password_Hash {
                    Algorithm = "pbkdf2",
                    Hash = new Hash {
                        // pbkdf2 should always use utf8 
                        Encoding = "utf8",
                        // Use the PHC string format
                        // https://github.com/P-H-C/phc-string-format/blob/master/phc-sf-spec.md
                        Value = hashValue
                    }
                }
            };

            Console.WriteLine(CreateJson(importUser));
        }

        // Replace this with real user accounts
        static IUserAccount GenerateExampleUserAccount(string examplePassword) =>
            new UserAccountMock(Crypto.HashPassword(examplePassword));

        static string CreateJson(ImportUser user) => JsonConvert.SerializeObject(new List<ImportUser> { user }, new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        });
    }
}
