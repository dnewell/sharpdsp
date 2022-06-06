using System.Numerics;

// The Main method is currently for testing DSPUtilities functions
// TODO: Refactor and integrate properly segregated unit testing framework, and use Program class to handle console-based file I/O and args
namespace DSPUtilities
{
    class Program
    {
        static void Main(string[] args)  // TODO: use Main for file I/O and processing command line args, not for testing 
        {
            // 'Emitted' and 'Captured' test files
            //string inputEmitFilePath = "/Testing/Test_audio/test_16k_emit.wav";
            //string inputCaptFilePath = "/Testing/Test_audio/test_16k_capt.wav";
            //string outputEmitFilePath = "/Testing/Test_audio/test_16k_emit_out.wav";
            //string outputCaptFilePath = "/Testing/Test_audio/test_16k_capt_out.wav";
            //string transformed = "/Testing/Test_audio/test_16k_full_circle.wav";

            // impulse test
            string inputWavPath = "/Testing/Test_audio/test_16k_sentences_emit.wav";
            string IRFilePath = "/Testing/Test_audio/ir_16kHz16bit.wav";
            string outputWavPath = "/Testing/Test_audio/test_convolutionResult.wav";

            Complex[] inputWav = DSPUtilities.ReadWavToComplexArray(inputWavPath);
            Complex[] inputIR = DSPUtilities.ReadWavToComplexArray(IRFilePath);

            DSPUtilities.WriteWaveToDisk(FFT.Convolve(inputWav, inputIR), outputWavPath);
            
        }
    }
}
