## What is this?
Here is a repository with an example for importing existing ASP.Net users with their password to 0Auth. I made this repo for my team at Greenbyte/Powerfactors, but I'm on maternal leave and I have a crying 6 week old in my lap and I'm sleep deprived. So errors might be made heh :D

If something isn't working please let me know and I'll do my best to fix it. The postman collection is old, so use that as an inspiration but check their docs.

I've added some comments in the code that explains how to create the custom password hash for importing ASP.NET users (regardless of version). 

#### Quick summary:
0Auth uses the PHC string format for defining the hashing details. The algorithm ASP.NET uses is PBKDF2 with HMAC-SHA1, with utf8 encoding.

So, in other words, for ASP.NET its:
SHA1 with 1000 iterations, key length is 32 bytes (not 64 seen in many examples), and then salt and hash is added. According to the PHC spec the values are prefixed with $. 
Thus the hash value for the custom password hash should be defined as:

$pbkdf2-sha1$i=1000,l=32$salt$hash

Example JSON:

```
"custom_password_hash": {
    "algorithm": "pbkdf2",
  "hash": {
    "value": "$pbkdf2-sha1$i=1000,l=32$Il2OAfYnj8JTYyDmZEMRlQ$yXpWUx4q8stARbGBgUHb/ScM2gCyrnHx7PCbtKN6Jik",
    "encoding": "utf8"
  }
}
```

The hash and salt in the JSON should be in the b65 format. No, that is not a typo. B64 is not the same as Base64. Remove the padding and you should be good.

```
var saltB64 = Convert.ToBase64String(salt).TrimEnd('=');

var passwordB64 = Convert.ToBase64String(passwordHashBytes).TrimEnd('=');
```

ASP.NET prefixes password hashes with an identifying byte so we can know the ASP.NET version, obviously this must be removed before you break apart the hash to grab the salt and the password (hash) from the password hash.

```
var passwordHashBytes = new byte[originalPwdHash.Length - (aspIdVersionPrefixLength + saltLength)];

// Notice that we have to skip the asp id prefix and salt to get password only
Buffer.BlockCopy(originalPwdHash, aspIdVersionPrefixLength + saltLength, passwordHashBytes, 0, passwordHashBytes.Length);
```

Checkout the code for all the fun. I'm going to bed now, at 9PM. I feel old.

