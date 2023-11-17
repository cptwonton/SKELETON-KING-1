namespace ZORGATH;

public class RegistrationDataHelper
{
    // The official HoN authentication server uses these in SRP. I took them from existing projects,
    // so not sure how they were reverse engineered.
    // Courtesy of https://github.com/theli-ua/pyHoNBot/blob/master/hon/masterserver.py#L37
    private const string magic = "[!~esTo0}";
    private const string magic2 = "taquzaph_?98phab&junaj=z=kuChusu";

    public static string HashAccountPasswordMD5(string md5Pass, string passwordSalt)
    {
        string saltedMagic = md5Pass + passwordSalt + magic;
        string md5Magic = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(saltedMagic))).ToLower();
        string magicTwo = md5Magic + magic2;
        return Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(magicTwo))).ToLower();
    }
}
