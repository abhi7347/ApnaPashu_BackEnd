using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

class Program {
    static void Main() {
        var handler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        };
        var tokenDict = handler.CreateJwtSecurityToken(claims: new ClaimsIdentity(claims)).Payload;
        
        foreach(var kv in tokenDict) {
            Console.WriteLine(kv.Key + "=" + kv.Value);
        }
    }
}
