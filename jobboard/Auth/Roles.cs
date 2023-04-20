namespace jobboard.Auth
{
    public class Roles
    {
        public const string Administratorius = nameof(Administratorius);
        public const string Darbuotojas = nameof(Darbuotojas);
        public const string Darbdavys = nameof(Darbdavys);

        public static readonly IReadOnlyCollection<string> All = new[] { Administratorius, Darbdavys, Darbuotojas };

    }
}
