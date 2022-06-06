using System;
using System.IO;
using System.Numerics;
using NAudio.Wave;

namespace DSPUtilities
{
    public static class DSPUtilities
    {
        // TODO: Make an enum/consider other ways to handle this
        private static readonly WaveFormat PCM16kHz16Bit = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 16000, 1, 32000, 2, 16);
        /// <summary>
        /// Read all samples of a *.pcm/*.wav file into a complex number array 
        /// - note: for audio data, the imaginary (System.Numerics.Complex.Imag) component should be zero.
        /// </summary>
        /// <param name="path">the full path to the wave file</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns>an array of complex numbers representing sample data</returns>
        public static Complex[] ReadWavToComplexArray(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            WaveFileReader reader = new WaveFileReader(path);
            Complex[] complexSamples = new Complex[reader.SampleCount];
            for (int i = 0; i < reader.SampleCount; i++)
            {
                complexSamples[i] = new Complex(reader.ReadNextSampleFrame()[0], 0);
            }
            return complexSamples;
        }

        /// <summary>
        /// Perform element-wise multiplication between all the elements the input arrays
        /// </summary>
        /// <param name="complexArrayX">a complex array</param>
        /// <param name="complexArrayY">a complex array</param>
        /// <returns>the product of the two input arrays</returns>
        /// <exception cref="InvalidOperationException">Error: The input arrays must contain the same number of elements.</exception>
        public static Complex[] ElementWiseMultiply(Complex[] complexArrayX, Complex[] complexArrayY)
        {
            if (complexArrayX.Length != complexArrayY.Length)
            {
                Console.WriteLine("Warning: input files are not the same length. This may indicate problem with the input data.");
            }

            Complex[] result = new Complex[complexArrayX.Length];
            for (int i = 0; i < complexArrayX.Length; i++)
            {
                result[i] = complexArrayX[i] * complexArrayY[i];
            }
            return result;
        }

        /// <summary>
        /// Apply complex conjugate function across all members of the array
        /// </summary>
        /// <param name="complexArray">input array of complex numbers</param>
        /// <returns>array with complex conjugates of the input</returns>
        public static Complex[] DoElementWiseConjugate(Complex[] complexArray)
        {
            Complex[] result = new Complex[complexArray.Length];
            for (int i = 0; i < complexArray.Length; i++)
            {
                result[i] = Complex.Conjugate(complexArray[i]);
            }
            return result;
        }
        /// <summary>
        /// Returns the NAudio.Wave.WaveFormat of a file.
        /// </summary>+
        /// <param name="path">the path to the file</param>
        /// <returns>the WaveFormat of the File</returns>
        public static WaveFormat GetWaveFormat(string path)
        {
            WaveFileReader reader = new WaveFileReader(path);
            WaveFormat format = reader.WaveFormat;
            reader.Dispose();
            return format;
        }
        /// <summary>
        /// Writes an array of complex numbers representing sample data to a wave file at the given path.
        /// Only produces a sensible result with 16bit data. 
        /// - note: for audio data, the imaginary component (Complex.imag) should be zero.
        /// </summary>
        /// <param name="complexArray">the array of complex numbers representing wave data</param>
        /// <param name="path">the destination path in the file system</param>
        /// <returns>true if the file was written to disk, false otherwise</returns>
        public static bool WriteWaveToDisk(Complex[] complexArray, string path)
        {
            int floatSampleCount = complexArray.Length;
            bool fileWritten;
            try
            {
                WaveFileWriter writer = new WaveFileWriter(path, PCM16kHz16Bit);
                for (int i = 0; i < floatSampleCount; i++)
                {
                    writer.WriteSample((float)complexArray[i].Real);
                }
                fileWritten = true;
                writer.Dispose();
            } catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                fileWritten = false;
            }
            return fileWritten;
        }
    }
}