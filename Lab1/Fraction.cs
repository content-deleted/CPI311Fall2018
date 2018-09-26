using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Lab1.Lab1Util;

namespace Lab1
{
    public struct Fraction
    {
        private int numerator;
        private int denominator;

        public Fraction (int n = 0, int d = 1) 
        {
            numerator = n;
            denominator = (d != 0) ? d : 1;
        }

        public static Fraction operator +(Fraction a, Fraction b) => Simplify(new Fraction(a.numerator * b.denominator + a.denominator * b.numerator, a.denominator * b.denominator));

        public static Fraction operator -(Fraction a, Fraction b) => Simplify(new Fraction(a.numerator * b.denominator - a.denominator * b.numerator, a.denominator * b.denominator));

        public static Fraction operator *(Fraction a, Fraction b) => Simplify(new Fraction(a.numerator * b.numerator, a.denominator * b.denominator));

        public static Fraction operator /(Fraction a, Fraction b) => Simplify(new Fraction(a.numerator * b.denominator, a.denominator * b.numerator));

        public override String ToString() => $"{numerator}/{denominator}";

        private static Fraction Simplify(Fraction f)
        {
            if (f.denominator < 0)
            {
                f.denominator *= -1;
                f.numerator *= -1;
            }
            int gcd = GCD(f.numerator, f.denominator);
            f.numerator /= gcd;
            f.denominator /= gcd;

            return f;
        }
    }
}
