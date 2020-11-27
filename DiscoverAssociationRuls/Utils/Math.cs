namespace DiscoverAssociationRuls
{
    public class Math
    {
        private static int Fraction(int number)
        {
            if (number == 1)
                return 1;
            return number * Fraction(number - 1);
        }
    }
}