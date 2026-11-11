namespace TaskBoard.Api.Domain;

public static class TaskValidator
{
    public const int MaxTitleLength = 120;

    public static IReadOnlyList<string> Validate(string? title)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(title))
            errors.Add("El título es obligatorio.");
        else if (title.Trim().Length > MaxTitleLength)
            errors.Add($"El título no puede superar {MaxTitleLength} caracteres.");

        return errors;
    }
}
