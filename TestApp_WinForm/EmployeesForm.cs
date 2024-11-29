using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace TestApp_WinForm
{
    public partial class EmployeesForm : Form
    {
        private Employee _currentEmployee = null;

        private bool _isEditing = false;

        public BindingList<Employee> EmployeeList { get; } = new BindingList<Employee>();

        public EmployeesForm()
        {
            InitializeComponent();

            dataEmployees.AutoGenerateColumns = false;
            dataEmployees.DataSource = EmployeeList;

            EnableDisable();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                _isEditing = true;
                _currentEmployee = null;

                dataEmployees.ClearSelection();

                txtFirstName.Text = string.Empty;
                txtLastName.Text = string.Empty;
                txtUsername.Text = string.Empty;

                EnableDisable();
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnNew_Click");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                _isEditing = true;
                _currentEmployee = dataEmployees.SelectedRows[0].DataBoundItem as Employee;

                EnableDisable();
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnEdit_Click");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Employee selectedEmployee = dataEmployees.SelectedRows[0].DataBoundItem as Employee;

                if (MessageBox.Show(this, $"Do you really want to delete '{selectedEmployee.Username}'?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    EmployeeList.Remove(selectedEmployee);
                    EmployeeList.ResetBindings();

                    dataEmployees.ClearSelection();

                    EnableDisable();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnDelete_Click");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateData())
                {
                    return;
                }

                if (_currentEmployee != null)
                {
                    _currentEmployee.FirstName = txtFirstName.Text;
                    _currentEmployee.LastName = txtLastName.Text;
                    _currentEmployee.Username = txtUsername.Text;
                }
                else
                {
                    _currentEmployee = new Employee()
                    {
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Username = txtUsername.Text,
                    };

                    EmployeeList.Add(_currentEmployee);
                }

                EmployeeList.ResetBindings();

                dataEmployees.Rows[EmployeeList.IndexOf(_currentEmployee)].Selected = true;

                _isEditing = false;
                _currentEmployee = null;

                EnableDisable();
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnSave_Click");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _isEditing = false;
                _currentEmployee = null;


                dataEmployees_SelectionChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnCancel_Click");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex, "btnClose_Click");
            }
        }

        private void EnableDisable()
        {
            bool hasSelection = dataEmployees.SelectedRows.Count == 1;

            txtFirstName.Enabled = _isEditing;
            txtLastName.Enabled = _isEditing;
            txtUsername.Enabled = _isEditing;

            dataEmployees.Enabled = !_isEditing;

            btnNew.Enabled = !_isEditing;
            btnEdit.Enabled = !_isEditing && hasSelection;
            btnDelete.Enabled = !_isEditing && hasSelection;
            btnSave.Enabled = _isEditing;
            btnCancel.Enabled = _isEditing;
        }

        private void ShowError(Exception ex, string methodName)
        {
            MessageBox.Show(this, $"ERROR - {GetType().Name}.{methodName}\n{ex.Message}", Text);
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show(this, "First Name must not be empty.", Text);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(this, "Last Name must not be empty.", Text);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show(this, "Username must not be empty.", Text);
                return false;
            }

            if (_currentEmployee == null) // new
            {
                if (EmployeeList.Any(x => x.Username.Equals(txtUsername.Text, StringComparison.InvariantCultureIgnoreCase)))
                {
                    MessageBox.Show(this, $"Username '{txtUsername.Text}' already exists.", Text);
                    return false;
                }
            }
            else
            {
                if (EmployeeList.Any(x => x.Id != _currentEmployee.Id && x.Username.Equals(txtUsername.Text, StringComparison.InvariantCultureIgnoreCase)))
                {
                    MessageBox.Show(this, $"Username '{txtUsername.Text}' already exists.", Text);
                    return false;
                }
            }

            return true;
        }

        private void dataEmployees_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_isEditing)
                {
                    Employee selectedEmployee = (dataEmployees.SelectedRows.Count == 1 ? dataEmployees.SelectedRows[0].DataBoundItem : null) as Employee;

                    txtFirstName.Text = selectedEmployee?.FirstName ?? string.Empty;
                    txtLastName.Text = selectedEmployee?.LastName ?? string.Empty;
                    txtUsername.Text = selectedEmployee?.Username ?? string.Empty;

                    EnableDisable();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex, "dataEmployees_SelectionChanged");
            }
        }
    }
}
