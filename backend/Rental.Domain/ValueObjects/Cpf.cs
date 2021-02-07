using System.Text.RegularExpressions;

namespace Rental.Domain.ValueObjects
{
    public class Cpf
    {
        public string Value { get; set; }
        public bool IsValid { get; }

        public Cpf(string input) 
        {
            var cpf = GetCpfFromInput(input);
            if (!string.IsNullOrEmpty(cpf) && IsValidCpf(cpf))
            {
                IsValid = true;
                Value = cpf;
            }
            else 
            {
                IsValid = false;
            }
        }
        private string GetCpfFromInput(string input) 
        {
            var regex = new Regex(@"[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2}");
            return regex.IsMatch(input) ? regex.Match(input).Value : string.Empty;
        }
        private bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            int[] firstMultiplier = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] secondMultiplier = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string temporaryCpf, digits;
            int sum, rest;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11) return false;

            temporaryCpf = cpf.Substring(0, 9);
            sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(temporaryCpf[i].ToString()) * firstMultiplier[i];
            }

            rest = sum % 11;

            rest = rest < 2 ? 0 : 11 - rest;

            digits = rest.ToString();
            temporaryCpf += digits;
            sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(temporaryCpf[i].ToString()) * secondMultiplier[i];
            }

            rest = sum % 11;

            rest = rest < 2 ? 0 : 11 - rest;

            digits += rest.ToString();

            return cpf.EndsWith(digits);
        }
    }
}
