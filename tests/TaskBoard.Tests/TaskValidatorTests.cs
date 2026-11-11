using TaskBoard.Api.Domain;

namespace TaskBoard.Tests;

public class TaskValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_TituloVacio_RetornaError(string? title)
    {
        var errors = TaskValidator.Validate(title);

        Assert.Single(errors);
    }

    [Fact]
    public void Validate_TituloMuyLargo_RetornaError()
    {
        var title = new string('a', TaskValidator.MaxTitleLength + 1);

        var errors = TaskValidator.Validate(title);

        Assert.Single(errors);
    }

    [Fact]
    public void Validate_TituloValido_NoRetornaErrores()
    {
        var errors = TaskValidator.Validate("Configurar Docker Compose");

        Assert.Empty(errors);
    }
}
