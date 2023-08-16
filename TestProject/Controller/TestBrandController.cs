using AssessmentAPI_Xunit.Controllers;
using AssessmentAPI_Xunit.model;
using AssessmentAPI_Xunit.Service.Interface;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Org.BouncyCastle.Crypto;
using System.Collections.Generic;

namespace TestProject.Controller
{
    public class TestBrandController
    {

        private readonly IFixture fixture;
        private readonly Mock<IBrandInteface> brandInterface;
        private readonly Mock<IVehicleInterface> vehicleInterface;
        private readonly BrandController brandController;

        public TestBrandController()
        {
            fixture = new Fixture();
            brandInterface = fixture.Freeze<Mock<IBrandInteface>>();
            vehicleInterface = fixture.Freeze<Mock<IVehicleInterface>>();
            brandController = new BrandController(brandInterface.Object,vehicleInterface.Object);
        }

       

        [Fact]
        public async Task AddBrand_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var brand = new Brand();
            brandInterface.Setup(repo => repo.AddBrand(It.IsAny<Brand>()))
                               .ReturnsAsync(brand);

            // Act
            var result = await brandController.AddBrand(brand);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(brand, okResult.Value);
        }
        [Fact]
        public async Task AddBrand_NullInput_ReturnsBadRequest()
        {


            // Act
            var result = await brandController.AddBrand(null) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        [Fact]

        public async Task AddBrand_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange

            brandInterface.Setup(repo => repo.AddBrand(It.IsAny<Brand>()))
                .ThrowsAsync(new Exception("Some error message"));

          
            // Act
            var result = await brandController.AddBrand(new Brand());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Some error message", badRequestResult.Value);
        }
        [Fact]
        public void GetAllBrandsOfAVehicleType_ValidId_ReturnsOkResultWithBrands()
        {
            // Arrange
            int vehicleTypeId = 1;
            var mockBrands = new List<Brand> { new Brand(), new Brand() };
            brandInterface.Setup(x => x.GetAllBrandsOfAVehicleType(vehicleTypeId)).Returns(mockBrands);
            vehicleInterface.Setup(x => x.IsExists(vehicleTypeId)).Returns(true);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBrands = Assert.IsAssignableFrom<IEnumerable<Brand>>(okResult.Value);
            Assert.Equal(mockBrands.Count, returnedBrands.Count());
        }

        [Fact]
        public void GetAllBrandsOfAVehicleType_InvalidId_ReturnsBadRequestWithMessage()
        {
            // Arrange
            int vehicleTypeId = 1;
            vehicleInterface.Setup(repo => repo.IsExists(vehicleTypeId)).Returns(false);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(vehicleTypeId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id not found", badRequestResult.Value);
        }
        [Fact]
        public void GetAllBrandsByVehicleType_ShouldReturnBadResponse_WhenVehicleIdIsNotExists()
        {
            // Arrange
            int id = 1;
            ICollection<Brand> brandByVehicleTypeList = null;
            brandInterface.Setup(x => x.GetAllBrandsOfAVehicleType(id)).Returns(brandByVehicleTypeList);
            vehicleInterface.Setup(x => x.IsExists(id)).Returns(false);

            // Act
            var result = brandController.GetAllBrandsOfAVehicleType(id);

            // Assert
            result.Should().NotBeNull();

            var getResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

            brandInterface.Verify(x => x.GetAllBrandsOfAVehicleType(id), Times.Never());
            vehicleInterface.Verify(x => x.IsExists(id), Times.Once());
        }

        [Fact]
        public void DeleteBrand_ValidId_ReturnsOkResult()
        {
            // Arrange
            int brandId = 1;
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(true);

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }
        [Fact]
        public void DeleteBrand_InvalidId_ReturnsBadRequestWithMessage()
        {
            // Arrange
            int brandId = 1;
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(false);

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something Went Wrong", badRequestResult.Value);
        }
        [Fact]
        public void DeleteBrand_Exception_ReturnsBadRequestWithExceptionMessage()
        {
            // Arrange
            int brandId = 1;
            var exceptionMessage = "An error occurred.";
            brandInterface.Setup(repo => repo.IsExists(brandId)).Returns(true);
            brandInterface.Setup(repo => repo.DeleteBrand(brandId))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = brandController.DeleteBrand(brandId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public void  UpdateBrand_ValidData_ReturnsOkResult()
        {
            int id=fixture.Create<int>();
            var brand = fixture.Create<Brand>();
            brand.BrandId = id;
            var returnData = fixture.Create<Brand>();
            brandInterface.Setup(c => c.IsExists(brand.VehicleTypeId)).Returns(true);
            brandInterface.Setup(c => c.UpdateBrand(id, brand)).ReturnsAsync(returnData);
          
            //Act
            var result = brandController.UpdateBrand(id, brand);

            //Assert
            result.Should().NotBeNull();
           result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
        }

        [Fact]
        public void  UpdateBrand_IdMismatch_ReturnsBadRequest()
        {
            int id = 1;
           
            var brand = fixture.Create<Brand>();
            brandInterface.Setup(c => c.UpdateBrand(id, brand)).Returns(Task.FromResult<Brand>(null));

            //Act
            var result = brandController.UpdateBrand(id, brand);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Should().BeAssignableTo<BadRequestResult>();
            

        }
        [Fact]
        public void EditColumn_ShouldReturnBadRequestObjectResult_WhenAnExceptionOccurred()
        {
            //Arrange
            int id = 1;
            var brandup = fixture.Create<Brand>();
            brandInterface.Setup(c => c.UpdateBrand(id, brandup)).Throws(new Exception());

            //Act
            var result = brandController.UpdateBrand(id, brandup);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Task<IActionResult>>();
            result.Result.Should().BeAssignableTo<BadRequestObjectResult>();
            
        }


    }





}
    
