using MathNet.Numerics.LinearAlgebra;

namespace ExpertSystem.Aggregator.Services
{
    public class LeastSquaresService
    {
        private static readonly double LargeNumber = 1d;
        private Matrix<double> _input;
        private Vector<double> _output;
        private Matrix<double> _combinedOutput;
        private Vector<double> _consequence;

        public LeastSquaresService(double[,] input, double[] output, int consequenceNeuronsSize)
        {
            _input = Matrix<double>.Build.DenseOfArray(input);
            _output = Vector<double>.Build.DenseOfArray(output);
            _consequence = Vector<double>.Build.Dense(consequenceNeuronsSize);
            _combinedOutput = Matrix<double>.Build.Dense(consequenceNeuronsSize, 3);
        }

        private Matrix<double> Calculate()
        {
            var aMatrix = Matrix<double>.Build.Dense(0, 0);
            var bMatrix = Matrix<double>.Build.DenseOfRowVectors(_output);

            var xMatrix = Matrix<double>.Build.Dense(aMatrix.ColumnCount, aMatrix.ColumnCount);
            var sMatrix = Matrix<double>.Build.DenseIdentity(aMatrix.ColumnCount, aMatrix.ColumnCount)
                .Multiply(LargeNumber);

            return xMatrix;
        }
    }
}