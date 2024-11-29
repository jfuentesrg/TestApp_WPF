using System.Text.RegularExpressions;

namespace TestApp.Entities
{
    public class Contact : EntityBase<Contact>
    {

        private int _ClientId;
        /// <summary>ClientId</summary>
        public int ClientId { get => _ClientId; set => SetField(ref _ClientId, value, nameof(ClientId)); }

        private string _FirstName;
        /// <summary>First Name</summary>
        public string FirstName { get => _FirstName; set => SetField(ref _FirstName, value?.Trim(), nameof(FirstName)); }

        private string _LastName;
        /// <summary>LastName</summary>
        public string LastName { get => _LastName; set => SetField(ref _LastName, value?.Trim(), nameof(LastName)); }

        private string _Email;
        /// <summary>Email</summary>
        public string Email { get => _Email; set => SetField(ref _Email, value?.Trim(), nameof(Email)); }

        public Contact()
        {

        }

        protected override void MatchPropertiesInternal(Contact fromEntity)
        {
            if (fromEntity == null)
            {
                return;
            }

            FirstName = fromEntity.FirstName;
            LastName = fromEntity.LastName;
            Email = fromEntity.Email;
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                return false;
            }
            // Check if the email is null, empty, or consists only of whitespace.
            // Also, validate the email format using the IsValidEmail method.
            // If either condition is true, return false to indicate an invalid email.
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }

        
        /// <summary>
        /// Checks if an email address is valid.
        /// Combines a validation using a regular expression and deeper verification using the MailAddress class.
        /// </summary>
        /// <param name="Email">The email address to validate.</param>
        /// <returns>
        /// Returns true if the email address is valid; 
        /// otherwise, returns false.
        /// </returns>
        public bool IsValidEmail(string Email)
        {
            // Check if the email is null or empty.
            // This ensures that no null or whitespace email is processed.
            if (string.IsNullOrWhiteSpace(Email))
                return false;

            // Use a regular expression to validate a basic email format.
            // The pattern includes:
            // 1. Characters before the '@' symbol.
            // 2. A domain after the '@' with a '.' to indicate the TLD (Top-Level Domain).
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email pattern.

            // If the email does not match the pattern, return false.
            if (!System.Text.RegularExpressions.Regex.IsMatch(Email, emailPattern))
                return false;

            try
            {
                // Use the MailAddress class for more detailed validation.
                // This includes additional checks such as special characters in the domain.
                var mail = new System.Net.Mail.MailAddress(Email);

                // Confirm that the provided address matches exactly with the generated one.
                // Prevents issues with emails containing unwanted spaces or additional characters.
                return mail.Address == Email;
            }
            catch
            {
                // If an exception occurs, the email is invalid.
                return false;
            }
        }
    }
}
