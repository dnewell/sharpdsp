using System;
using System.IO;
using System.Numerics;

namespace DSPUtilities
{
    class FFT
    {
        /// <summary>
        /// Perform a complex fast fourier transform on an array.
        /// </summary>
        /// <param name="samples">array of audio samples (imaginary part will be zero)</param>
        /// <returns>the fourier transformation of the samples</returns>
        public static Complex[] DoFFT(Complex[] samples)
        {
            Complex[] resizedSampleArray = ResizeAndZeroPad(samples, 0);
            return RecursiveFFT(resizedSampleArray);
        }

        /// <summary>
        /// Perform 1D convolution of two complex arrays.
        /// </summary>
        /// <param name="samples">array of audio samples (imaginary part will be zero)</param>
        /// <param name="signalToConvolve1"></param>
        /// <param name="signalToConvolve2"></param>
        /// <returns>the fourier transformation of the samples</returns>
        public static Complex[] Convolve(Complex[] signalToConvolve1, Complex[] signalToConvolve2)
        {
            // Zero-pad and perform a FFT on the inputs
            Complex[] dftOfInput1 = DoFFT(signalToConvolve1);
            Complex[] dftOfInput2 = DoFFT(signalToConvolve2);

            // Perform element-wise complex multiplication of the DFTs
            Complex[] complexProductOfDfts = DSPUtilities.ElementWiseMultiply(dftOfInput1, dftOfInput2);

            // Perform inverse FFT on the complex product
            Complex[] convolvedSignals = DoInverseFFT(complexProductOfDfts);

            return convolvedSignals;
        }

        /// <summary>
        /// Perform 1D convolution of two complex arrays of dissimilar cardinality.
        /// </summary>
        /// <param name="signalToConvolve1"></param>
        /// <param name="signalToConvolve2"></param>
        /// <returns>the convolution of two </returns>
        public static Complex[] ConvolveWithIR(Complex[] signalToConvolve1, Complex[] signalToConvolve2)
        {
            // find the greater number of samples between the two input signals
            int highestSampleNumber = Math.Max(signalToConvolve1.Length, signalToConvolve2.Length);

            // Zero-pad and perform a FFT on the inputs
            Complex[] dftOfInput1 = DoIRFFT(signalToConvolve1, highestSampleNumber);
            Complex[] dftOfInput2 = DoIRFFT(signalToConvolve2, highestSampleNumber);

            // Perform element-wise complex multiplication of the DFTs
            Complex[] complexProductOfDfts = DSPUtilities.ElementWiseMultiply(dftOfInput1, dftOfInput2);

            // Perform inverse FFT on the complex product
            Complex[] convolution = DoInverseFFT(complexProductOfDfts);

            return convolution;
        }

        /// <summary>
        /// Perform cross-correlation of two complex arrays.
        /// </summary>
        /// <param name="complexArrayX">array of audio samples (imaginary part will be zero)</param>
        /// <param name="complexArrayY">array of audio samples (imaginary part will be zero)</param>
        /// <returns>the cross-correlation of the samples</returns>
        public static Complex[] DoCrossCorrelation(Complex[] complexArrayX, Complex[] complexArrayY)
        {
            // Zero-pad and perform a FFT on the inputs
            Complex[] dftOfX = DoFFT(complexArrayX);
            Complex[] dftOfY = DoFFT(complexArrayY);

            // Perform element-wise complex multiplication of the DFTs
            Complex[] multipliedDfts = DSPUtilities.ElementWiseMultiply(dftOfX, dftOfY);

            // Perform inverse FFT on the complex product
            Complex[] convolvedSignals = DoInverseFFT(multipliedDfts);

            return convolvedSignals;
        }
        /// <summary>
        /// Perform a complex fast Fourier transform on an array with a specified number of samples.
        /// </summary>
        /// <param name="complexArray">array of audio samples (imaginary parts will be zero)</param>
        /// <param name="numSamples">number of samples in longer file</param>
        /// <returns>the Fourier transformation of the samples</returns>
        private static Complex[] DoIRFFT(Complex[] complexArray, int numSamples)
        {
            Complex[] resizedSampleArray = ResizeAndZeroPad(complexArray, numSamples);
            return RecursiveFFT(resizedSampleArray);
        }

        /// <summary>
        /// Recursively compute the complex fast Fourier transform of an array. 
        /// Implemented using a recursive implementation of the Cooley-Tukey radix-2 algorithm: O(n log n)
        /// </summary>
        /// <param name="complexArray">array of complexArray (size should be padded with zeros to a length of the first power of 2
        /// equal to or greater than the intial length</param>
        /// <returns>the discrete Fourier transformation of the complexArray</returns>
        private static Complex[] RecursiveFFT(Complex[] complexArray)
        {
            int halfArraySize = complexArray.Length/2;
            Complex[] evenElements = new Complex[halfArraySize];
            Complex[] oddElements = new Complex[halfArraySize];
            Complex[] result = new Complex[complexArray.Length];

            // basis
            if (complexArray.Length == 1)
            {
                return complexArray;
            }

            // input array must be radix 2
            if (complexArray.Length%2 != 0)
            {
                throw new System.ArgumentException("Length of input array is not a power of 2");
            }

            // collect even elements
            for (int i = 0; i < halfArraySize; i++)
            {
                evenElements[i] = complexArray[i*2];
            }

            // call RecursiveFFT on even elements
            Complex[] fftOfEvenElements = RecursiveFFT(evenElements);

            // collect odd elements
            for (int i = 0; i < halfArraySize; i++)
            {
                oddElements[i] = complexArray[i*2 + 1];
            }

            // recursive call performed on odd elements
            Complex[] fftOfOddElements = RecursiveFFT(oddElements);

            // combine elements
            for (int i = 0; i < halfArraySize; i++)
            {
                // applying Euler's formula
                double eulerTerm = i*-2*Math.PI/complexArray.Length;
                Complex oddPartMultiplier = new Complex(Math.Cos(eulerTerm), Math.Sin(eulerTerm));

                result[i] = oddPartMultiplier*fftOfOddElements[i] + fftOfEvenElements[i];
                result[i + halfArraySize] = fftOfEvenElements[i] - oddPartMultiplier*fftOfOddElements[i];
            }
            return result;
        }

        /// <summary>
        /// Recursively compute an inverse complex fast Fourier transform of an array. 
        /// Implemented using a recursive implementation of the Cooley-Tukey radix-2 algorithm: O(n log n)
        /// </summary>
        /// <param name="complexArray">array of complexArray (size should be padded with zeros to a length of the first power of 2
        /// equal to or greater than the intial length</param>
        /// <returns>the fourier transformation of the complexArray</returns>
        public static Complex[] DoInverseFFT(Complex[] complexArray)
        {
            Complex[] result = null;
            try
            {
                result = new Complex[complexArray.Length];

                // take conjugate
                for (int i = 0; i < complexArray.Length; i++)
                {
                    result[i] = Complex.Conjugate(complexArray[i]);
                }

                // compute forward FFT
                result = RecursiveFFT(result);

                // take conjugate again
                for (int i = 0; i < complexArray.Length; i++)
                {
                    result[i] = Complex.Conjugate(result[i]);
                }

                // divide by the number of sample points (including zero-padding)
                for (int i = 0; i < complexArray.Length; i++)
                {
                    result[i] = result[i] * (1.0 / complexArray.Length); 
                }
            }
            catch (OverflowException e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        /// TODO:
        /// <summary>
        /// Finds the relative offset in # of samples between two PCM/wav files
        /// </summary>
        /// <param name="memoryStreamX">A memory stream the emitted test signal PCM data</param>
        /// <param name="memoryStreamY">A memory stream of the PCM data of the captured result</param>
        /// <returns>The number of samples between the click in memoryStreamX and that of memoryStreamY</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static int FindSampleOffset(MemoryStream memoryStreamX, MemoryStream memoryStreamY)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return 0;
        }

        /// <summary>
        /// Checks if a number is a power of two.
        /// Handles the 0 and negative value cases.
        /// </summary>
        /// <param name="num">number to check</param>
        /// <returns>true if the parameter represents a power of two, false otherwise</returns>
        private static bool IsPowerOfTwo(long num)
        {
            return (num > 0) & ((num & (num - 1)) == 0);
        }

        /// TODO: handle 'targetSamples' or consider other approach
        /// <summary>
        /// Right-pad the array length with zeros to the first power of 2 which is either
        /// A) equal to or greater than twice the original array length (default behavior - 'targetSamples' set to 0), 
        /// or
        /// B) equal to or greater than twice the length specified by the 'targetSamples' parameter,
        ///     if the integer passed is greater than the original array length
        /// 
        /// ~~ note on second parameter ~~
        /// - the 'targetSamples' parameter specifies the minimum number of
        ///    samples from which to resize and zero-pad. This is useful when convolving a long signal
        ///    with a short impulse response.
        /// - it only has an effect if a value greater than the original array length is passed
        /// </summary>
        /// <param name="complexArray">the complex array to resize</param>
        /// <param name="targetSamples">minimum number of samples from which to size the padded array (0 == default behavior)</param>
        /// <returns>Zero padded array</returns>
        private static Complex[] ResizeAndZeroPad(Complex[] complexArray, int targetSamples)
        {
            int paddedLength = NecessarySignificantBits(complexArray.Length)*2;
            int bitsInLength;
            if (IsPowerOfTwo(complexArray.Length))
            {
                bitsInLength = NecessarySignificantBits(complexArray.Length) - 1; 
            }
            else
            {
                bitsInLength = NecessarySignificantBits(complexArray.Length);
                paddedLength = 1 << bitsInLength;
            }
            // add zero-padding
            Complex[] resizedSampleArray = new Complex[paddedLength*2];
            complexArray.CopyTo(resizedSampleArray, 0);
            return resizedSampleArray;
        }

        /// <summary>
        /// Compute the number of significant bits required to store a number
        /// (the first sufficiently large power of 2)
        /// </summary>
        /// <param name="num">a number</param>
        /// <returns>minimum number of bits necessary to store the number</returns>
        private static int NecessarySignificantBits(int num)
        {
            int bitCount = 0;
            while (num > 0)
            {
                num >>= 1;
                bitCount++;
            }
            return bitCount;
        }

    }
}