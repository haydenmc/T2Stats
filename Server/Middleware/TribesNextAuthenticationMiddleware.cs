using System;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace T2Stats.Middleware
{
    public class TribesNextAuthenticationMiddleware
    {
        private const string AuthorizationTypeString = "tribesnext";

        private readonly RequestDelegate next;

        private readonly TribesNextAuthenticationMiddlewareOptions options;

        private readonly BigInteger authenticationServerExponent;

        private readonly BigInteger authenticationServerModulus;

        public TribesNextAuthenticationMiddleware(RequestDelegate next, IOptions<TribesNextAuthenticationMiddlewareOptions> optionsAccessor)
        {
            this.next = next;
            this.options = optionsAccessor.Value;
            this.authenticationServerExponent = BigInteger.Parse(this.options.AuthenticationServerExponent);
            this.authenticationServerModulus = new BigInteger(
                hexStringToByteArray(this.options.AuthenticationServerModulus)
                    .Reverse()
                    .Concat(new byte[] { 0 })
                    .ToArray()
            );
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeaderContents = context.Request.Headers["Authorization"].ToString();
                if (authorizationHeaderContents.ToLower().StartsWith(AuthorizationTypeString.ToLower()))
                {
                    var authFields = authorizationHeaderContents
                        .Substring(AuthorizationTypeString.Length) // cut out starting 'tribesnext'
                        .Trim()
                        .Split('\t'); // Split auth into fields
                    var userName = authFields[0];
                    var guid = authFields[1];
                    var exponent = authFields[2];
                    var modulus = authFields[3];
                    var signedHash = authFields[4];

                    // Calculate SHA1 sum
                    var hashAlgorithm = SHA1.Create();
                    var sha1Str = userName + "\t" + guid + "\t" + exponent + "\t" + modulus;
                    var calculatedSha1 = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(sha1Str));
                    var calculatedSha1Str = string.Join("", calculatedSha1.Select(b => b.ToString("x2")).ToArray());

                    // Decrypt signed SHA1
                    var decryptedBytes = rsaOperation(
                        hexStringToByteArray(signedHash),
                        authenticationServerExponent,
                        authenticationServerModulus
                    );
                    var decryptedHashStr = byteArrayToHexString(decryptedBytes);
                    if (decryptedHashStr.Length > 40) // Strip padding from the beginning
                    {
                        decryptedHashStr = decryptedHashStr.Substring(decryptedHashStr.Length - 40);
                    }

                    // Compare signed hash to computed hash
                    if (calculatedSha1Str.ToLower() == decryptedHashStr.ToLower())
                    {
                        // Add identity claims after authenticity is verified.
                        var identity = new ClaimsIdentity(
                            new[] {
                                new Claim(ClaimTypes.Name, userName),
                                new Claim(ClaimTypes.NameIdentifier, guid)
                            },
                            "TribesNext",
                            ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType
                        );
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }

            // Call the next delegate/middleware in the pipeline
            return this.next(context);
        }

        private static byte[] hexStringToByteArray(string hex)
        {
            return Enumerable
                .Range(0, hex.Length / 2)
                .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                .ToArray();
        }

        private static string byteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private static byte[] rsaOperation(byte[] data, BigInteger exp, BigInteger mod)
        {
            BigInteger bData = new BigInteger(
                data    //our data block
                .Reverse()  //BigInteger has another byte order
                .Concat(new byte[] { 0 }) // append 0 so we are allways handling positive numbers
                .ToArray() // constructor wants an array
            );
            return 
                BigInteger.ModPow(bData, exp, mod) // the RSA operation itself
                .ToByteArray() //make bytes from BigInteger
                .Reverse() // back to "normal" byte order
                .ToArray(); // return as byte array
        }
    }
}