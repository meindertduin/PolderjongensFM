namespace Pjfm.Application.Identity
{
    public struct ApplicationIdentityConstants
    {
        public struct Policies
        {
            public const string Mod = nameof(Mod);
            public const string User = nameof(User);
        }

        public struct Claims
        {
            public const string Role = nameof(Role);
        }

        public struct Roles
        {
            public const string Mod = nameof(Mod);
        }
    }
}