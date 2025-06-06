using Movil2.Data;
using Movil2.Models;

namespace Movil2.Pages;

public partial class ProductosPage : ContentPage
{
    ProductoDatabase _database;

    public ProductosPage()
    {
        InitializeComponent();
        // Inicializamos la base de datos aquí con la ruta de archivo
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "productos.db3");
        _database = new ProductoDatabase(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarProductos();
    }

    async Task CargarProductos()
    {
        var lista = await _database.GetProductosAsync();
        ProductosListView.ItemsSource = lista;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) || string.IsNullOrWhiteSpace(PrecioEntry.Text))
        {
            await DisplayAlert("Error", "Debes ingresar nombre y precio", "OK");
            return;
        }

        if (!decimal.TryParse(PrecioEntry.Text, out decimal precio))
        {
            await DisplayAlert("Error", "Precio inválido", "OK");
            return;
        }

        var nuevoProducto = new Producto
        {
            Nombre = NombreEntry.Text,
            Precio = precio
        };

        await _database.SaveProductoAsync(nuevoProducto);
        NombreEntry.Text = string.Empty;
        PrecioEntry.Text = string.Empty;
        await CargarProductos();

        await DisplayAlert("Éxito", "Producto agregado correctamente", "OK");
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as Producto;

        if (producto == null)
            return;

        // Solicitar el nuevo precio al usuario
        string nuevoPrecioTexto = await DisplayPromptAsync(
            "Editar Precio",
            $"Producto: {producto.Nombre}\nPrecio actual: {producto.Precio:C}\n\nIngresa el nuevo precio:",
            "Guardar",
            "Cancelar",
            "0.00",
            keyboard: Keyboard.Numeric);

        // Si el usuario canceló o no ingresó nada
        if (string.IsNullOrWhiteSpace(nuevoPrecioTexto))
            return;

        // Validar que el precio sea válido
        if (!decimal.TryParse(nuevoPrecioTexto, out decimal nuevoPrecio))
        {
            await DisplayAlert("Error", "El precio ingresado no es válido", "OK");
            return;
        }

        // Validar que el precio sea mayor a 0
        if (nuevoPrecio <= 0)
        {
            await DisplayAlert("Error", "El precio debe ser mayor a cero", "OK");
            return;
        }

        try
        {
            // Actualizar el precio del producto
            producto.Precio = nuevoPrecio;
            await _database.SaveProductoAsync(producto);
            await CargarProductos(); // Recargar la lista
            await DisplayAlert("Éxito", $"Precio actualizado correctamente\nNuevo precio: {nuevoPrecio:C}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar el precio: {ex.Message}", "OK");
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as Producto;

        if (producto == null)
            return;

        // Mostrar confirmación antes de eliminar
        bool confirmacion = await DisplayAlert(
            "Confirmar eliminación",
            $"¿Estás seguro de que quieres eliminar el producto '{producto.Nombre}'?",
            "Sí",
            "No");

        if (confirmacion)
        {
            try
            {
                await _database.DeleteProductoAsync(producto);
                await CargarProductos(); // Recargar la lista
                await DisplayAlert("Éxito", "Producto eliminado correctamente", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar el producto: {ex.Message}", "OK");
            }
        }
    }
}