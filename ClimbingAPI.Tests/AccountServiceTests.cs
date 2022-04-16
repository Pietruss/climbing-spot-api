using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using ClimbingAPI.Entities;
using ClimbingAPI.Services;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class AccountServiceTests
    {
        public static readonly object[][] UserTestData =
        {
            new object[] { new DateTime(2017,3,1), "test@gmail.com", "TestName", "TestLastName", 1},
            new object[] { new DateTime(2017, 2, 1), "", "", "", 0 }
        };

        [Theory]
        [MemberData(nameof(UserTestData))]
        public void GenerateClaims_ForGivenUser_ReturnsFilledValuesInClaimsList(DateTime? dateOfBirth, string email, string firstName, string lastName, int id)
        {
            // arrange
            AccountService accountService = new AccountService();
            User userTemplate = new User
            {
                 DateOfBirth = dateOfBirth,
                 Email = email,
                 FirstName = firstName,
                 LastName = lastName,
                 Id = id
            };
            List<Claim> initialClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userTemplate.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userTemplate.FirstName} {userTemplate.LastName}"),
                new Claim("DateOfBirth", userTemplate.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };

            var userId = initialClaimsList[0].Value;
            var userName = initialClaimsList[1].Value;
            var UserDateOfBirth = initialClaimsList[2].Value;


            // act 
            var claimsList = accountService.GenerateClaims(userTemplate);

            // assert
            Assert.Equal(userId, claimsList.Where(x => x.Type.Contains("nameidentifier")).Select(x => x.Value).First());
            Assert.Equal(userName, claimsList.Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")).Select(x => x.Value).First());
            Assert.Equal(UserDateOfBirth, claimsList.Where(x => x.Type.Contains("DateOfBirth")).Select(x => x.Value).First());
        }

    }
}
