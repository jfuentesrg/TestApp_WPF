using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TestApp.Entities;
using TestApp.Views;

namespace TestApp.ViewModels
{
    public class ClientViewModel : ViewModelBase<ClientView>
    {
        private Client _originalClient;

        private Client _EditClient;
        /// <summary>EditClient</summary>
        public Client EditClient { get => _EditClient; private set => SetNotifyableProperty(ref _EditClient, value, nameof(EditClient)); }

        public BindingList<Client> ClientList { get; private set; }

        private bool _IsEditing;
        /// <summary>IsEditing</summary>
        public bool IsEditing
        {
            get => _IsEditing;

            set
            {
                if (SetNotifyableProperty(ref _IsEditing, value, nameof(IsEditing)))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        private Client _SelectedClient;
        /// <summary>SelectedClient</summary>
        public Client SelectedClient
        {
            get => _SelectedClient;

            set
            {
                if (SetNotifyableProperty(ref _SelectedClient, value, nameof(SelectedClient)))
                {
                    EditClient = SelectedClient;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public List<Province> ProvinceList { get; protected set; }

        public ClientViewModel(ClientView view)
            : base(view)
        {
            CancelCommand = new DelegateCommand(Cancel, CanCancel);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            NewCommand = new DelegateCommand(New, CanNew);
            SaveCommand = new DelegateCommand(Save, CanSave);

            // Initialize the list of clients. This list will likely be bound to the UI for display.
            ClientList = new BindingList<Client>();

            // Populate the list of provinces from the static Provinces property.
            // Used to populate dropdowns or selection fields for provinces in the UI.
            ProvinceList = Province.Provinces;
        }

        #region Cancel
        public DelegateCommand CancelCommand { get; }

        private bool CanCancel() => IsEditing;

        private void Cancel()
        {
            try
            {
                // Check if the user confirms the action to cancel the current changes.
                // If the user does not confirm (returns false), exit the method early.
                if (!ConfirmAction("Cancel the current changes")) return;


                if (EditClient.IsDirty && !EditClient.IsNew)
                {
                    SelectedClient.MatchProperties(_originalClient);
                }

                _originalClient = null;
                EditClient = SelectedClient;

                IsEditing = false;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Cancel");
            }
        }
        #endregion

        #region Delete
        public DelegateCommand DeleteCommand { get; }

        private bool CanDelete() => !IsEditing && SelectedClient != null;

        private void Delete()
        {
            try
            {
                // Prompt the user to confirm the deletion of the client.
                // If the user does not confirm (returns false), exit the method without proceeding.
                if (!ConfirmAction("delete this client")) return;


                ClientList.Remove(SelectedClient);

                _originalClient = null;
                SelectedClient = null;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Delete");
            }
        }
        #endregion

        #region Edit
        public DelegateCommand EditCommand { get; }

        private bool CanEdit() => !IsEditing && SelectedClient != null;

        private void Edit()
        {
            try
            {
                _originalClient = SelectedClient.Clone();
                EditClient = SelectedClient.Clone();
                EditClient.AcceptChanges();

                IsEditing = true;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Edit");
            }
        }
        #endregion

        #region New
        public DelegateCommand NewCommand { get; }

        private bool CanNew() => !IsEditing;

        private void New()
        {
            try
            {
                _originalClient = null;
                SelectedClient = null;
                EditClient = new Client();

                IsEditing = true;
            }
            catch (Exception ex)
            {
                ShowError(ex, "New");
            }
        }
        #endregion

        #region Save
        public DelegateCommand SaveCommand { get; }

        private bool CanSave() => IsEditing;

        private void Save()
        {
            try
            {
                if (EditClient.IsDirty && Validate())
                {
                    bool isNew = EditClient.IsNew;
                    Client client;

                    if (isNew)
                    {
                        client = EditClient.Clone();
                    }
                    else
                    {
                        SelectedClient.MatchProperties(EditClient);
                        client = SelectedClient;
                    }

                    foreach (Contact contact in client.Contacts)
                    {
                        contact.ClientId = client.Id;
                        contact.AcceptChanges();
                    }

                    client.AcceptChanges();

                    _originalClient = null;
                    SelectedClient = null;

                    if (isNew)
                    {
                        ClientList.Add(client);
                    }

                    ClientList = new BindingList<Client>(ClientList.OrderBy(x => x.Name).ToList());
                    RaisePropertyChanged(nameof(ClientList));

                    SelectedClient = client;
                    EditClient = SelectedClient;

                    IsEditing = false;
                }
                // Check if the company's name is a duplicate.
                // If the name already exists (based on the IsNameDuplicate method):
                // Show a warning message to the user indicating the duplicate name.
                // Prevent further execution by exiting the method with 'return'.
                else
                {
                    if (IsNameDuplicate(EditClient.Name, EditClient.IsNew ? (int?)null : SelectedClient.Id))
                    {
                        MessageBox.Show($"Company Name '{EditClient.Name}' already exists. Please choose a different name.",
                                           "Duplicate Company Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }


            }
            catch (Exception ex)
            {
                ShowError(ex, "Save");
            }
        }
        #endregion

        // Checks if a given client name is already in use.
        // Ignores case sensitivity and optionally excludes a client by ID (useful for edits).
        private bool IsNameDuplicate(string name, int? currentClientId = null)
        {
            // Search the client list for any client with the same name.
            // Exclude the client with the given ID (if provided) to avoid false positives during updates.
            return ClientList.Any(client =>
                client.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                (!currentClientId.HasValue || client.Id != currentClientId.Value));
        }

        // Displays a confirmation dialog for user actions like deletion or cancellation.
        // Returns true if the user confirms the action; false otherwise.
        private bool ConfirmAction(string actionDescription)
        {
            return MessageBox.Show($"Are you sure you want to {actionDescription}?",
                "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }


        private void RaiseCanExecuteChanged()
        {
            CancelCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            NewCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private bool Validate()
        {
            if (!EditClient.Validate())
            {
                return false;
            }

            // Validate that the client has at least one contact before saving.
            // If the 'Contacts' collection is null or empty:
            // Display an error message to inform the user. 
            if (EditClient.Contacts == null || !EditClient.Contacts.Any())
            {
                MessageBox.Show("Please add at least one contact before saving the client.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            if (EditClient.IsNew)
            {
                if (ClientList.Any(x => x.Name.Equals(EditClient.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return false;
                }
            }
            else
            {
                if (ClientList.Any(x => x.Id != SelectedClient.Id && x.Name.Equals(EditClient.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
