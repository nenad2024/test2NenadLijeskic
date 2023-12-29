using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace test2NenadLijeskic
{
    
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            string key = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            string api = $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={key}";
            HttpResponseMessage response = await client.GetAsync(api);
            
            string json = await response.Content.ReadAsStringAsync();
                

                
             var employeeData = JsonConvert.DeserializeObject<List<EmployeeData>>(json);
             var groupedResults = employeeData.GroupBy(
             p => p.EmployeeName,
             p => p.TotalWorkedHours,
             (name, total) => new KeyValuePair<string, double>(name, total.Sum()));

            var EmployeeNames = groupedResults.Select(x => x.Key).ToArray();
            var EmployeeHours = groupedResults.Select(x => x.Value).ToArray();

            string[] labels = EmployeeNames;
            double[] data = EmployeeHours;

            SKColor[] colors = new SKColor[data.Length];
            Random random = new Random();
            for (int j = 0; j < data.Length; j++)
            {
                colors[j] = new SKColor((byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256));
            }

            

            int width = 800;
            int height = 600;

            using (SKBitmap bitmap = new SKBitmap(width, height))
            {
                
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(SKColors.White); 

                    using (SKPaint paint = new SKPaint())
                    {
                        SKRect rect = new SKRect(0, 0, width, height);
                        double startAngle = 0;

                        for (int i = 0; i < data.Length-1; i++)
                        {
                            paint.Color = colors[i]; 
                            double sweepAngle = (360 * data[i]) / data.Sum();
                            if (string.IsNullOrEmpty(labels[i]))
                            {
                                continue;
                            }
                                
                            
                            //canvas.DrawArc(rect, (float)startAngle, (float)sweepAngle, true, paint);
                            //startAngle += sweepAngle;

                            float midAngle = (float)(startAngle + (sweepAngle / 2));
                            
                            SKPaint textPaint = new SKPaint {
                                TextSize = 13,
                                Color = SKColors.Black
                            };
                            float x = (float)(width / 2 + (width / 2 * Math.Cos(midAngle * Math.PI / 180)));
                            float y = (float)(height / 2 + (height / 2 * Math.Sin(midAngle * Math.PI / 180))) - textPaint.TextSize;
                            string nameText = labels[i];
                            string percentageText = $"{(data[i] / data.Sum()) * 100:F2}%";
                            SKRect textBounds = new SKRect();
                            textPaint.MeasureText(nameText, ref textBounds);


                            float textX = x - textBounds.MidX/2;
                            float textY = y + textBounds.Height/4;

                            
                            canvas.DrawArc(rect, (float)startAngle, (float)sweepAngle, true, paint);
                            canvas.DrawText(nameText, textX, textY, textPaint);
                            canvas.DrawText(percentageText,textX+10, textY+10 ,textPaint);
                            startAngle += sweepAngle;
                        }
                    }

                    Random rnd = new  Random();
                    int r = rnd.Next();
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, $"PieChart{r}.png");



                    using (FileStream fileStream = File.Create(filePath))
                    {
                        bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
                        Console.WriteLine($"Pie chart download location: {filePath}");
                    }
                }
            }
        }
    
    }
}
