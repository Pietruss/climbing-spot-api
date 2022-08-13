using ClimbingAPI.Entities;
using ClimbingAPI.Utils;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class WhoColumnsTests
    {
        [Theory]
        [InlineData("CreatedByUserId", 4)]
        [InlineData("CreatedByUserId", 10)]
        public async Task SetCreationUserIdValues_ForGivenDataSet_ReturnsAppropiateValue(string createdByUserIdColumn, int? createdByUserIdValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var CreatedByUserIdProperty = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(createdByUserIdColumn));

            //act
            WhoColumns.SetCreationValues(CreatedByUserIdProperty, climbingSpot, createdByUserIdValue, DateTime.Now);

            //assert
            CreatedByUserIdProperty.GetValue(climbingSpot).Should().Be(createdByUserIdValue.ToString());
        }

        [Theory]
        [InlineData("CreationDateTime", "2008-06-12")]
        [InlineData("CreationDateTime", "2228-06-13")]
        public async Task SetCreationValues_ForGivenDataSet_ReturnsAppropiateValue(string CreationDateTimeColumn, string CreationDateTimeValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var CreationDateTimeProperty = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(CreationDateTimeColumn));

            //act
            WhoColumns.SetCreationValues(CreationDateTimeProperty, climbingSpot, 10, DateTime.Parse(CreationDateTimeValue));

            //assert
            CreationDateTimeProperty.GetValue(climbingSpot).Should().Be(DateTime.Parse(CreationDateTimeValue));
        }

        [Theory]
        [InlineData("ModifiedByUserId", 4)]
        [InlineData("ModifiedByUserId", 10)]
        public async Task SetModificationValues_ModifiedByUserId_ForGivenDataSet_ReturnsAppropiateValue(string columnName, int? expectedValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var property = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(columnName));

            //act
            WhoColumns.SetModificationValues(property, climbingSpot, expectedValue, DateTime.Now);

            //assert
            property.GetValue(climbingSpot).Should().Be(expectedValue.ToString());
        }

        [Theory]
        [InlineData("ModificationDateTime", "2008-06-12")]
        [InlineData("ModificationDateTime", "2228-06-13")]
        public async Task SetModificationValues_ModificationDateTime_ForGivenDataSet_ReturnsAppropiateValue(string ModificationDateTimeColumn, string ModificationDateTimeValue)
        {
            //arrange
            var climbingSpot = new ClimbingSpot() { };
            var property = climbingSpot.GetType().GetProperties().FirstOrDefault(x => x.Name.Equals(ModificationDateTimeColumn));

            //act
            WhoColumns.SetModificationValues(property, climbingSpot, 10, DateTime.Parse(ModificationDateTimeValue));

            //assert
            property.GetValue(climbingSpot).Should().Be(DateTime.Parse(ModificationDateTimeValue));
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
