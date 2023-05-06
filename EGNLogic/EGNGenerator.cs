using System.Globalization;
using System.Runtime.CompilerServices;

namespace EGNLogic
{
    public class EGNGenerator
    {
        private static int[] weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Region Region { get; set; }
        public EGNGenerator(DateOnly date = default, Gender gender = Gender.Неизвестен, Region region = Region.Неизвестен)
        {
            DateOfBirth = date;
            Gender = gender;
            Region = region;
        }
        public static bool CheckValidity(string egn)
        {
            if (string.IsNullOrEmpty(egn) || string.IsNullOrWhiteSpace(egn) || egn.Length != 10) { return false; }

            if(!EGNGenerator.TryGetBirthDate(egn, out var date)) { return false; }

            int.TryParse(egn.Substring(9, 1), out var checksum);
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(egn[i].ToString()) * weights[i];
            }

            sum %= 11;
            
            if (sum == 10)
            {
                sum = 0;
            }

            if (checksum != sum)
            {
                return false;
            }

            return true;
        }
        public static bool TryParse(string egn, out EGNGenerator output)
        {
            output = default;

            if (!EGNGenerator.CheckValidity(egn)) { return false; }

            if (!EGNGenerator.TryGetBirthDate(egn, out var birthday)) { return false; }
            if (!EGNGenerator.TryGetRegion(egn, out var region)) { return false; }
            if (!EGNGenerator.TryGetGender(egn, out var gender)) { return false; }

            output = new EGNGenerator(date:birthday, gender:gender, region:region);

            return true;
        }
        public string GenerateEGN()
        {
            if (DateOfBirth.Equals(default) || Gender.Equals(Gender.Неизвестен) || Region.Equals(Region.Неизвестен))
            {
                throw new ArgumentException("ЕГН не можа да се генерира поради некоректно зададени параметри.");
            }

            var egn = "";
            var year = DateOfBirth.Year.ToString().Substring(2).PadLeft(2, '0');
            string mounth;
            var day = DateOfBirth.Day.ToString().PadLeft(2, '0');

            if (DateOfBirth < DateOnly.Parse("January 1, 1900"))
            {
                mounth = (DateOfBirth.Month + 20).ToString().PadLeft(2, '0');
            }
            else if (DateOfBirth > DateOnly.Parse("December 31, 1999"))
            {
                mounth = (DateOfBirth.Month + 40).ToString().PadLeft(2, '0');
            }
            else
            {
                mounth = (DateOfBirth.Month).ToString().PadLeft(2, '0');
            }

            var previousValue = Enum.GetValues(typeof(Region)).Cast<Region>().LastOrDefault(e => (int)e < (int)Region);
            var region = Random.Shared.Next(previousValue == 0 ? 0 : (int)previousValue + 1, (int)Region + 1);


            if (Gender == Gender.Мъж && (region % 2 != 0))
            {
                region--;
            }
            if (Gender == Gender.Жена && (region % 2 == 0))
            {
                region++;
            }

            var strReg = region.ToString().PadLeft(3, '0');

            egn = $"{year}{mounth}{day}{strReg}";

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(egn[i].ToString()) * weights[i];
            }

            var checkSum = sum % 11;

            if (checkSum == 10) { checkSum = 0; }

            egn = $"{egn}{checkSum}";

            return egn;
        }
        private static bool TryGetBirthDate(string egn, out DateOnly output)
        {
            output = default;

            if (string.IsNullOrEmpty(egn) || string.IsNullOrWhiteSpace(egn) || egn.Length != 10) { return false; }

            if (!int.TryParse(egn.Substring(0, 2), out int year)) { return false; }
            if(!int.TryParse(egn.Substring(2, 2), out int mounth)) { return false; }
            if(!int.TryParse(egn.Substring(4, 2), out int day)) { return false; }

            if (mounth > 40)
            {
                if (!DateOnly.TryParse($"{day}/{mounth - 40}/{year + 2000}"
                    , new CultureInfo("en-US")
                    , DateTimeStyles.None
                    , out output))
                {
                    return false;
                }
            }
            else if (mounth > 20)
            {
                if (!DateOnly.TryParse($"{day}/{mounth - 20}/{year + 1800}"
                    , new CultureInfo("en-US")
                    , DateTimeStyles.None
                    , out output))
                {
                    return false;
                }
            }
            else
            {
                if (!DateOnly.TryParse($"{day}/{mounth}/{year + 1900}"
                    , new CultureInfo("en-US")
                    , DateTimeStyles.None
                    , out output))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool TryGetRegion(string egn, out Region output)
        {
            output = Region.Неизвестен;

            if (string.IsNullOrEmpty(egn) || string.IsNullOrWhiteSpace(egn) || egn.Length != 10) { return false; }

            if (!int.TryParse(egn.Substring(6, 3), out var regNum)) { return false; }

            var regions = Enum.GetValues(typeof(Region)).Cast<int>().ToList();
            int left = 0, right = 0;

            int i = 0;
            while (regNum >= regions[i])
            {
                left = regions[i];
                if (i + 1 < regions.Count)
                {
                    right = regions[i + 1];
                }
                else
                {
                    break;
                }
                i++;
            }

            output = (Region)right;

            return true;
        }
        private static bool TryGetGender(string egn, out Gender output) 
        {
            output = Gender.Неизвестен;

            if (string.IsNullOrEmpty(egn) || string.IsNullOrWhiteSpace(egn) || egn.Length != 10) { return false; }

            if (!int.TryParse(egn[8].ToString(), out var genderNum)) { return false; }

            output = genderNum % 2 == 0 ? Gender.Мъж : Gender.Жена;

            return true;
        }
        public override string ToString()
        {
            return $"birthday:{this.DateOfBirth} gender:{this.Gender} region:{this.Region}";
        }
    }
}
