using BitMiracle.LibTiff.Classic;

namespace TiffSzat
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Перенесите файл в папку и нажмите ENTER");
            Console.ReadLine();
            Console.WriteLine("Введите имя файла:");
            var readFile = Console.ReadLine();
            string inputFilePath = readFile + ".tif";
            var outputFile = readFile + "_сжат";
            string outputFilePath = outputFile + ".tif";

            using (Tiff inputTiff = Tiff.Open(inputFilePath, "r"))
            {
                if (inputTiff == null)
                {
                    Console.WriteLine("Failed to open input TIFF file.");
                    return;
                }

                using (Tiff outputTiff = Tiff.Open(outputFilePath, "w"))
                {
                    if (outputTiff == null)
                    {
                        Console.WriteLine("Failed to create output TIFF file.");
                        return;
                    }

                    // Копирование заголовков изображения
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

                    // Чтение и запись данных изображения
                    byte[] buffer = new byte[width * samplesPerPixel];
                    for (int i = 0; i < height; i++)
                    {
                        inputTiff.ReadScanline(buffer, i);
                        outputTiff.WriteScanline(buffer, i);
                    }
                }
            }

            Console.WriteLine("Сжатие завержено. Нажмите ENTER.");
            Console.ReadLine();
        }
    }
}