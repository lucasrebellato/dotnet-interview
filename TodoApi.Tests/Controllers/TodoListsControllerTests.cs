using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.Interfaces;


#nullable disable
namespace TodoApi.Tests.Controllers
{
    public class TodoListsControllerTests
    {
        private TodoListsController CreateController(Mock<ITodoListService> serviceMock)
            => new TodoListsController(serviceMock.Object);

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkWithList()
        {
            // Arrange
            var dtoList = new List<TodoListResponseDto>
            {
                new TodoListResponseDto { Id = 1, Name = "Task 1" },
                new TodoListResponseDto { Id = 2, Name = "Task 2" }
            };

            var serviceMock = new Mock<ITodoListService>();
            serviceMock.Setup(s => s.GetAll()).ReturnsAsync(dtoList);

            var controller = CreateController(serviceMock);

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IList<TodoListResponseDto>>(ok.Value);
            Assert.Equal(2, value.Count);
            serviceMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnsOkWithItem()
        {
            // Arrange
            var dto = new TodoListResponseDto { Id = 1, Name = "Task 1" };

            var serviceMock = new Mock<ITodoListService>();
            serviceMock.Setup(s => s.GetById(1)).ReturnsAsync(dto);

            var controller = CreateController(serviceMock);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<TodoListResponseDto>(ok.Value);
            Assert.Equal(1, value.Id);
            Assert.Equal("Task 1", value.Name);
            serviceMock.Verify(s => s.GetById(1), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCalled_ReturnsOkWithUpdatedItem()
        {
            // Arrange
            var updatedDto = new TodoListResponseDto { Id = 2, Name = "Changed Task 2" };

            var serviceMock = new Mock<ITodoListService>();
            serviceMock
                .Setup(s => s.Update(2, It.IsAny<UpdateTodoListDto>()))
                .ReturnsAsync(updatedDto);

            var controller = CreateController(serviceMock);

            // Act
            var result = await controller.Update(2, new UpdateTodoListDto { Name = "Changed Task 2" });

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<TodoListResponseDto>(ok.Value);
            Assert.Equal("Changed Task 2", value.Name);
            serviceMock.Verify(s => s.Update(2, It.IsAny<UpdateTodoListDto>()), Times.Once);
        }

        [Fact]
        public async Task Create_WhenCalled_ReturnsCreatedAtActionWithItem()
        {
            // Arrange
            var createdDto = new TodoListResponseDto { Id = 3, Name = "Task 3" };

            var serviceMock = new Mock<ITodoListService>();
            serviceMock
                .Setup(s => s.Create(It.IsAny<CreateTodoListDto>()))
                .ReturnsAsync(createdDto);

            var controller = CreateController(serviceMock);

            // Act
            var result = await controller.Create(new CreateTodoListDto { Name = "Task 3" });

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<TodoListResponseDto>(created.Value);
            Assert.Equal(3, value.Id);
            Assert.Equal("Task 3", value.Name);
            serviceMock.Verify(s => s.Create(It.IsAny<CreateTodoListDto>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnsNoContent()
        {
            // Arrange
            var serviceMock = new Mock<ITodoListService>();
            serviceMock.Setup(s => s.Delete(2)).Returns(Task.CompletedTask);

            var controller = CreateController(serviceMock);

            // Act
            var result = await controller.Delete(2);

            // Assert
            Assert.IsType<NoContentResult>(result);
            serviceMock.Verify(s => s.Delete(2), Times.Once);
        }
    }
}