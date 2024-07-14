namespace SharpCraft.World;

// kindly used code from https://github.com/thecodeofnotch/rd-161348/blob/master/src/main/java/com/mojang/rubydung/level/PerlinNoiseFilter.java
public sealed class PerlinNoiseFilter(int octave)
{
    private const int Fuzz = 16;

    public int[] GetMap(int width, int height)
    {
        var random = new Random();
        var table = new int[width * height];

        for (int step = width >> octave, y = 0; y < height; y += step)
        {
            for (var x = 0; x < width; x += step)
            {
                table[x + y * width] = (random.Next(256) - 128) * Fuzz;
            }
        }

        for (var step = width >> octave; step > 1; step /= 2)
        {
            var max = 256 * (step << octave);
            var halfStep = step / 2;

            for (var y = 0; y < height; y += step)
            {
                for (var x = 0; x < width; x += step)
                {
                    var value = table[x % width + y % height * width];
                    
                    var stepX = table[(x + step) % width + y % height * width];
                    var stepY = table[x % width + (y + step) % height * width];
                    var stepXy = table[(x + step) % width + (y + step) % height * width];
                    
                    var mutatedValue = (value + stepY + stepX + stepXy) / 4 + random.Next(max * 2) - max;
                    table[x + halfStep + (y + halfStep) * width] = mutatedValue;
                }
            }
            
            for (var y = 0; y < height; y += step)
            {
                for (var x = 0; x < width; x += step)
                {
                    var value = table[x % width + y % height * width];
                    
                    var stepX = table[(x + step) % width + y % height * width];
                    var stepY = table[x % width + (y + step) % height * width];

                    var halfStepXPos = table[(x + halfStep & width - 1) + (y + halfStep - step & height - 1) * width];
                    var halfStepYPos = table[(x + halfStep - step & width - 1) + (y + halfStep & height - 1) * width];

                    var halfStepValue = table[(x + halfStep) % width + (y + halfStep) % height * width];
                    
                    var mutatedValueX = (value + stepX + halfStepValue + halfStepXPos) / 4 + random.Next(max * 2) - max;
                    var mutatedValueY = (value + stepY + halfStepValue + halfStepYPos) / 4 + random.Next(max * 2) - max;

                    table[x + halfStep + y * width] = mutatedValueX;
                    table[x + (y + halfStep) * width] = mutatedValueY;
                }
            }
        }
        
        var result = new int[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[x + y * width] = table[x % width + y % height * width] / 512 + 128;
            }
        }

        return result;
    }
}