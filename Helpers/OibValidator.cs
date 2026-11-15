namespace Ultimate_Fight_Club_X.Helpers;

public static class OibValidator
{
    public static bool IsValid(string? oib)
    {
        if (oib is null || oib.Length != 11 || !oib.All(char.IsDigit))
            return false;

        int a = 10;
        for (int i = 0; i < 10; i++)
        {
            a = (a + (oib[i] - '0')) % 10;
            if (a == 0) a = 10;
            a = (a * 2) % 11;
        }

        int control = 11 - a;
        if (control == 10) control = 0;

        return control == oib[10] - '0';
    }
}