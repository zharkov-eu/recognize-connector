using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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

        private Matrix<double> GetHelperMatrix()
        {
            var parameterCount = _input.ColumnCount + 1;
            var helperMatrix =
                Matrix.Build.Dense(_combinedOutput.ColumnCount, _combinedOutput.RowCount * parameterCount);

            for (int i = 0; i < _combinedOutput.ColumnCount; i++)
            for (int j = 0; j < _combinedOutput.RowCount; j++)
            for (int k = 0; k < parameterCount; k++)
                if (k == _input.ColumnCount)
                    helperMatrix[i, j * parameterCount + k] = _combinedOutput[j, i];
                else
                    helperMatrix[i, j * parameterCount + k] = _combinedOutput[j, i] * _input[i, k];

            return helperMatrix;
        }

        private Matrix<double> Calculate()
        {
            var aMatrix = GetHelperMatrix();
            var bMatrix = Matrix<double>.Build.DenseOfRowVectors(_output);

            var x = Matrix<double>.Build.Dense(aMatrix.ColumnCount, aMatrix.ColumnCount);
            var s = Matrix<double>.Build.DenseIdentity(aMatrix.ColumnCount, aMatrix.ColumnCount).Multiply(LargeNumber);

            for (int i = 0; i < aMatrix.RowCount; i++)
            {
                var aT = aMatrix.SubMatrix(0, i, 0, aMatrix.ColumnCount - 1);
                var a = aT.Transpose();
                var b = bMatrix[0, i];

                s = s.Subtract(
                    s.Multiply(a).Multiply(aT).Multiply(s).Multiply(1 / (1d + aT.Multiply(s).Multiply(a)[0, 0]))
                );

                x = x.Add(s.Multiply(a).Multiply(b - aT.Multiply(x)[0, 0]));
            }

            return x;
        }
    }
}