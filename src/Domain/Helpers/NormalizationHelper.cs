namespace Domain.Helpers;

public static class NormalizationHelper
{
    public static string NormalizarTexto(string valor)
    {
        return string.Join(" ", valor.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public static string NormalizarNumeros(string valor)
    {
        return new string([.. valor.Where(char.IsDigit)]);
    }
}