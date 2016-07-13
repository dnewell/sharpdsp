using System.Numerics;

namespace DSPUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            // 'Emitted' and 'Captured' test files
            string inputEmitFilePath = "/Testing/Test_audio/test_16k_emit.wav";
            string inputCaptFilePath = "/Testing/Test_audio/test_16k_capt.wav";
            string outputEmitFilePath = "/Testing/Test_audio/test_16k_emit_proc.wav";
            string outputCaptFilePath = "/Testing/Test_audio/test_16k_capt_proc.wav";
            string transformed = "/Testing/Test_audio/test_16k_full_circle.wav";

            // impulse test
            string sentencesFilePath = "/Testing/Test_audio/test_16k_sentences_emit.wav";
            string impulseFilePath = "/Testing/Test_audio/ir_16kHz16bit.wav";
            string resultPath = "/Testing/Test_audio/test_convolutionResult.wav";

            Complex[] sentences = DSPUtilities.ReadWavToComplexArray(sentencesFilePath);
            Complex[] impulse = DSPUtilities.ReadWavToComplexArray(impulseFilePath);

            DSPUtilities.WriteWaveToDisk(FFT.Convolve(sentences, impulse), resultPath);
            
        }
    }
}
