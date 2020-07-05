using System.Collections.Generic;

namespace Project2048
{
    public class Weights : List<Weight>
    {
        public Weights()
        {
            // 0empty
            Add(new Weight(270, 270, 270));

            // 1sum
            Add(new Weight(11, 2, 100));

            // 2mono
            Add(new Weight(47, 2, 100));

            // 3merge
            Add(new Weight(700, 200, 2000));

            // 4smooth
            Add(new Weight(100, 100, 100));

            // 5sumpow
            Add(new Weight(3.5, 2, 10));

            // 6monopow
            Add(new Weight(4, 2, 10));

            // 7smoothpow
            Add(new Weight(4, 0.1, 10));
        }
        public double EmptyWeight
        {
            get { return this[0]; }
        }
        public double SumWeight
        {
            get { return this[1]; }
        }
        public double MonoWeight
        {
            get { return this[2]; }
        }
        public double MergeWeight
        {
            get { return this[3]; }
        }
        public double SmoothWeight
        {
            get { return this[4]; }
        }
        public double SumPower
        {
            get { return this[5]; }
        }
        public double MoveMonoPower
        {
            get { return this[6]; }
        }
        public double SmoothPower
        {
            get { return this[7]; }
        }
        public void SetInterpolationToWeight(int weightIndex, double interp)
        {
            this[weightIndex] = this[weightIndex].SetInterpolation(interp);
        }
        public void SetDeltaChangeToWeight(int weightIndex, double deltaChange)
        {
            this[weightIndex] = this[weightIndex].SetDeltaChange(deltaChange);
        }
        public override string ToString()
        {
            return
                $"\tEmptyWeight:\t{EmptyWeight}\n" +
                $"\tSumWeight:\t{SumWeight}\n" +
                $"\tMoveMonoWeight:\t{MonoWeight}\n" +
                $"\tMergeWeight:\t{MergeWeight}\n" +
                $"\tSmoothWeight:\t{SmoothWeight}\n" +
                $"\tSumPower:\t{SumPower}\n" +
                $"\tMoveMonoPower:\t{MoveMonoPower}\n" +
                $"\tSmoothPower:\t{SmoothPower}\n";
        }


    }
}
