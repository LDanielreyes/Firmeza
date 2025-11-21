namespace Firmeza.Models;

public static class ColumnHeuristics
{
    public static readonly Dictionary<string, string[]> ProductKeywords = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        { "Name", new[] { "Nombre", "Product Name", "Item", "Artículo" } },
        { "Price", new[] { "Precio", "P. Unitario", "Unit Price", "Coste" } },
        { "Stock", new[] { "Inventario", "Stock", "Cantidad Existente", "Qty" } }
    };

    public static readonly Dictionary<string, string[]> ClientKeywords = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        { "FullName", new[] { "Cliente", "Nombre Cliente", "Customer Name", "Razon Social" } },
        { "Email", new[] { "Correo", "Email", "E-Mail" } },
        { "PhoneNumber", new[] { "Telefono", "Phone", "Celular" } }
    };
}