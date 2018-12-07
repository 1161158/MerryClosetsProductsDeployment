using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MerryClosets.Services.Interfaces;

namespace MerryClosets.Services.EF {

    public class UserValidationService : IUserValidationService {

        static string validationUrl = "https://merryclosetsusers.herokuapp.com/users/validate";
        static string userRefUrl = "https://merryclosetsusers.herokuapp.com/users/user-ref";

        private class Token {
            public string token { get; set; }
        }

        private class ValidationResult {
            public string userRef;
            public string[] roles;
        }

        static HttpClient client = new HttpClient();

        public async Task<bool> validateContentManager(string tokenString) {
            Token token = new Token();
            token.token = tokenString;

            HttpResponseMessage response = await client.PostAsJsonAsync(validationUrl, token);

            var statusCode = response.StatusCode;

            if(statusCode == HttpStatusCode.OK) {
                if(isContentManager(await response.Content.ReadAsAsync<ValidationResult>())) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public async Task<string> GetUserRef(string tokenString)
        {
            Token token = new Token();
            token.token = tokenString;

            HttpResponseMessage response = await client.PostAsJsonAsync(userRefUrl, token);

            var statusCode = response.StatusCode;

            if (statusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsAsync<ValidationResult>();
                return result.userRef;
            }

            return "";
        }
        
        private bool isContentManager(ValidationResult result) {
            for(int i = 0; i < result.roles.Length; i++) {
                if(result.roles[i] == "Content Manager") {
                    return true;
                }
            }
            return false;
        }
    }
}