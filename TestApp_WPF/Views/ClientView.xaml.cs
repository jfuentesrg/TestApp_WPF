using System.Windows;
using System.Windows.Controls;
using TestApp.Entities;
using TestApp.ViewModels;

namespace TestApp.Views
{
    /// <summary>
    /// Logique d'interaction pour ClientView.xaml
    /// </summary>
    public partial class ClientView : Window, IView
    {
        public IViewModel ViewModel { get => DataContext as IViewModel; set => DataContext = value; }

        public ClientView()
        {
            InitializeComponent();

            ViewModel = new ClientViewModel(this);
        }


        /// <summary>
        /// ///Inicio de codigo 
        /// </summary>

   private void ContactsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
{
    // Verificar si la columna editada es "Email"
    if (e.Column.Header.ToString() == "Email")
    {
        var dataGrid = sender as DataGrid;

        if (dataGrid != null)
        {
            // Obtener el contacto que se está editando
            if (dataGrid.SelectedItem is Contact contact)
            {
                // Capturar el nuevo valor del correo electrónico
                var editingElement = e.EditingElement as TextBox;
                if (editingElement != null)
                {
                    string newEmail = editingElement.Text; // Valor ingresado en la celda

                    // Validar el correo electrónico ingresado
                    if (!contact.IsValidEmail(newEmail)) // Método externo para validar el email
                    {
                        // Mostrar un mensaje de advertencia
                        MessageBox.Show("The entered email is invalid. Please enter a valid email address.",
                                        "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Opcional: Cancelar la edición para evitar que se guarde el dato inválido
                        e.Cancel = true;
                        return;
                    }

                    // Si es válido, puedes procesarlo o enviarlo como parámetro a otro método
                    //ProcessEmail(newEmail); // Método que recibe el email como parámetro
                }
            }
        }
    }
}



    }
}
