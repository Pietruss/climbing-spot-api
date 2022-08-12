using ClimbingAPI.Entities;
using ClimbingAPI.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class WhoColumnsTests
    {
        [Theory]
        [InlineData("CreatedByUserId", 4)]
        [InlineData("CreatedByUserId", 10)]
        public async Task SetCreationValues_ForGivenDataSet_ReturnsAppropiateValue(string columnName, int? expectedValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var property = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(columnName));

            //act
            WhoColumns.SetCreationValues(property, climbingSpot, expectedValue);

            //assert
            property.GetValue(climbingSpot).Should().Be(expectedValue.ToString());
        }

        [Theory]
        [InlineData("ModifiedByUserId", 4)]
        [InlineData("ModifiedByUserId", 10)]
        public async Task SetModificationValues_ForGivenDataSet_ReturnsAppropiateValue(string columnName, int? expectedValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var property = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(columnName));

            //act
            WhoColumns.SetModificationValues(property, climbingSpot, expectedValue);

            //assert
            property.GetValue(climbingSpot).Should().Be(expectedValue.ToString());
        }

        [Fact]
        public async Task GetObjectProperties_ForGivenDataSet_ReturnsAppropiateValue()
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            
            //act
            var properties = WhoColumns.GetObjectProperties(climbingSpot);

            //assert
            properties.Length.Should().Be(14);
        }
    }
}
