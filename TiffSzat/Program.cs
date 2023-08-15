using BitMiracle.LibTiff.Classic;

namespace TiffSzat
{
    class Program
    {
        static void Main()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string fileSearchPattern = "*.tif";

            try
            {
                string[] files = Directory.GetFiles(currentDirectory, fileSearchPattern);

                foreach (string inputFile in files)
                {
                    CompressTiff(inputFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }

            Console.WriteLine("Сжатие завершено. Нажмите ENTER.");
            Console.ReadLine();
        }

        static void CompressTiff(string inputFilePath)
        {
            string outputFilePath = Path.Combine(
                Path.GetDirectoryName(inputFilePath) ?? throw new InvalidOperationException(),
                Path.GetFileNameWithoutExtension(inputFilePath) + "_сжат.tif");

            using (Tiff inputTiff = Tiff.Open(inputFilePath, "r"))
            using (Tiff outputTiff = Tiff.Open(outputFilePath, "w"))
            {
                if (inputTiff == null || outputTiff == null)
                {
                    Console.WriteLine($"Не удалось обработать файл: {inputFilePath}");
                    return;
                }

                int width = inputTiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = inputTiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                int bitsPerPixel = inputTiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                int samplesPerPixel = inputTiff.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();

                outputTiff.SetField(TiffTag.IMAGEWIDTH, width);
                outputTiff.SetField(TiffTag.IMAGELENGTH, height);
                outputTiff.SetField(TiffTag.BITSPERSAMPLE, bitsPerPixel);
                outputTiff.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel);
                outputTiff.SetField(TiffTag.COMPRESSION, Compression.LZW); // Задайте желаемый метод сжатия
                outputTiff.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                outputTiff.SetField(TiffTag.COMPRESSION, Compression.LZW);
                outputTiff.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);

                byte[] buffer = new byte[width * samplesPerPixel];
                for (int i = 0; i < height; i++)
                {
                    inputTiff.ReadScanline(buffer, i);
                    outputTiff.WriteScanline(buffer, i);
                }

                Console.WriteLine($"Файл сжат и сохранен: {outputFilePath}");
            }
        }
    }
}