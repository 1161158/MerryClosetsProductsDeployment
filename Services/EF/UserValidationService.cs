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

        private static ValidationResult _validationResult;

        private class Token {
            public string token { get; set; }
        }

        private class ValidationResult {
            public string userRef;
            public string[] roles;
        }

        static HttpClient client = new HttpClient();

        public async Task<bool> ValidateContentManager(string tokenString) {
            Token token = new Token();
            token.token = tokenString;

            
            if(await Validate(tokenString)) {
                if(IsContentManager(_validationResult.roles)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public async Task<bool> Validate(string tokenString)
        {
            Token token = new Token();
            token.token = tokenString;

            HttpResponseMessage response = await client.PostAsJsonAsync(validationUrl, token);

            var statusCode = response.StatusCode;

            if (statusCode == HttpStatusCode.OK)
            {
                _validationResult = await response.Content.ReadAsAsync<ValidationResult>();
                return true;
            }

            return false;
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

        public bool CheckAuthorizationToken(string authorization) {
            try {
                string token = authorization.Split(" ")[1];
                return true;
            } catch (Exception e){
                return false;
            }
        }
        
        private bool IsContentManager(string[] roles) {
            for(int i = 0; i < roles.Length; i++) {
                if(roles[i] == "Content Manager") {
                    return true;
                }
            }
            return false;
        }
    }
}