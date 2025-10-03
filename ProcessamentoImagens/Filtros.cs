using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ProcessamentoImagens
{
    class Filtros
    {
        struct Ponto
        {
            public int x, y;
        }
        //sem acesso direto a memoria
        public static void convert_to_gray(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int r, g, b;
            Int32 gs;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    Color cor = imageBitmapSrc.GetPixel(x, y);

                    r = cor.R;
                    g = cor.G;
                    b = cor.B;
                    gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);

                    //nova cor
                    Color newcolor = Color.FromArgb(gs, gs, gs);

                    imageBitmapDest.SetPixel(x, y, newcolor);
                }
            }
        }

        //sem acesso direito a memoria
        public static void negativo(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    Color cor = imageBitmapSrc.GetPixel(x, y);

                    r = cor.R;
                    g = cor.G;
                    b = cor.B;

                    //nova cor
                    Color newcolor = Color.FromArgb(255 - r, 255 - g, 255 - b);

                    imageBitmapDest.SetPixel(x, y, newcolor);
                }
            }
        }

        //com acesso direto a memória
        public static void convert_to_grayDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;
            Int32 gs;

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++); //está armazenado dessa forma: b g r 
                        g = *(src++);
                        r = *(src++);
                        gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                    }
                    src += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        //com acesso direito a memoria
        public static void negativoDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock dados bitmap origem 
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src1 = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src1++); //está armazenado dessa forma: b g r 
                        g = *(src1++);
                        r = *(src1++);

                        *(dst++) = (byte)(255 - b);
                        *(dst++) = (byte)(255 - g);
                        *(dst++) = (byte)(255 - r);
                    }
                    src1 += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem 
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        public static void pretoeBranco(Bitmap sourceBitmap, Bitmap imageBitmapDest)
        {
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = sourceBitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDest = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int srcStride = bitmapDataSrc.Stride;
            int dstStride = bitmapDataDest.Stride;

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDest.Scan0.ToPointer();
                byte threshold = 221;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int srcIndex = (y * srcStride) + (x * pixelSize);
                        int dstIndex = (y * dstStride) + (x * pixelSize);

                        byte b = src[srcIndex];
                        byte g = src[srcIndex + 1];
                        byte r = src[srcIndex + 2];

                        int gray = (int)(0.299 * r + 0.587 * g + 0.114 * b);


                        if (gray >= threshold)
                        {
                            dst[dstIndex] = 255;
                            dst[dstIndex + 1] = 255;
                            dst[dstIndex + 2] = 255;
                        }
                        else
                        {
                            dst[dstIndex] = 0;
                            dst[dstIndex + 1] = 0;
                            dst[dstIndex + 2] = 0;
                        }
                    }
                }
            }
            sourceBitmap.UnlockBits(bitmapDataSrc);
            imageBitmapDest.UnlockBits(bitmapDataDest);
        }

        public static void ZhangSuen(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDest = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bitmapDataSrc.Stride;
            int totalBytes = stride * height;

            unsafe
            {
                byte* srcPtr = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDest.Scan0.ToPointer();

                for (int i = 0; i < totalBytes; i++)
                    dst[i] = srcPtr[i];

                bool afinando = true;
                while (afinando)
                {
                    afinando = false;

                    List<Ponto> remPoints = new List<Ponto>();
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (preto(dst, x, y, stride))
                            {
                                int[] viz = getVizinhosInts(dst, x, y, stride);
                                int somaVizinhos = somaVizinhosFromArray(viz);
                                int transicoes = ContaTransicoes(viz);
                                if (transicoes == 1 && somaVizinhos >= 3 && somaVizinhos <= 5 &&
                                    (viz[0] * viz[2] * viz[6] == 0) && (viz[0] * viz[4] * viz[6] == 0))
                                {
                                    remPoints.Add(new Ponto { x = x, y = y });
                                }
                            }
                        }
                    }

                    if (remPoints.Count > 0)
                    {
                        foreach (var pixel in remPoints)
                        {
                            int index = (pixel.y * stride) + (pixel.x * pixelSize);
                            dst[index] = 255;       // B
                            dst[index + 1] = 255;   // G
                            dst[index + 2] = 255;   // R
                        }
                        remPoints.Clear();
                        afinando = true;
                    }
                      
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (preto(dst, x, y, stride))
                            {
                                int[] viz = getVizinhosInts(dst, x, y, stride);
                                int somaVizinhos = somaVizinhosFromArray(viz);
                                int transicoes = ContaTransicoes(viz);

                                if (transicoes == 1 && somaVizinhos >= 3 && somaVizinhos <= 5 &&
                                    (viz[0] * viz[2] * viz[4] == 0) && (viz[2] * viz[4] * viz[6] == 0))
                                {
                                    remPoints.Add(new Ponto { x = x, y = y });
                                }
                            }
                        }
                    }

                    if (remPoints.Count > 0)
                    {
                        foreach (var pixel in remPoints)
                        {
                            int index = (pixel.y * stride) + (pixel.x * pixelSize);
                            dst[index] = 255;       // B
                            dst[index + 1] = 255;   // G
                            dst[index + 2] = 255;   // R
                        }
                        remPoints.Clear();
                        afinando = true;
                    }
                }

                imageBitmapSrc.UnlockBits(bitmapDataSrc);
                imageBitmapDest.UnlockBits(bitmapDataDest);
            }
        }

        private unsafe static bool preto(byte* src, int x, int y, int stride)
        {
            int index = (y * stride) + (x * 3);
            byte b = src[index];
            byte g = src[index + 1];
            byte r = src[index + 2];
            return (r + g + b) / 3 == 0;
        }

        private unsafe static bool branco(byte* src, int x, int y, int stride)
        {
            int index = (y * stride) + (x * 3);
            byte b = src[index];
            byte g = src[index + 1];
            byte r = src[index + 2];
            return (r + g + b) / 3 == 255;
        }

        private unsafe static int[] getVizinhosInts(byte* src, int x, int y, int stride)
        {
            int[] viz = new int[8];
            viz[0] = preto(src, x, y - 1, stride) ? 1 : 0;
            viz[1] = preto(src, x + 1, y - 1, stride) ? 1 : 0;
            viz[2] = preto(src, x + 1, y, stride) ? 1 : 0;
            viz[3] = preto(src, x + 1, y + 1, stride) ? 1 : 0;
            viz[4] = preto(src, x, y + 1, stride) ? 1 : 0;
            viz[5] = preto(src, x - 1, y + 1, stride) ? 1 : 0;
            viz[6] = preto(src, x - 1, y, stride) ? 1 : 0;
            viz[7] = preto(src, x - 1, y - 1, stride) ? 1 : 0;
            return viz;
        }

        private static int somaVizinhosFromArray(int[] viz)
        {
            int sum = 0;
            for (int i = 0; i < viz.Length; i++) 
                sum += viz[i];
            return sum;
        }
        private static int ContaTransicoes(int[] vizinhos)
        {
            int transicoes = 0;
            for (int i = 0; i < vizinhos.Length - 1; i++)
            {
                if (vizinhos[i] == 0 && vizinhos[i + 1] == 1)
                    transicoes++;
            }
            if (vizinhos[7] == 0 && vizinhos[0] == 1)
                transicoes++;
            return transicoes;
        }

        public static void quadrado(Bitmap imagemSrc, Bitmap imagemDst)
        {
            int width = imagemSrc.Width;
            int height = imagemSrc.Height;

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int menorX = x, maiorX = x, menorY = y, maiorY = y;
                    if (imagemSrc.GetPixel(x + 1, y).B == 0 && imagemSrc.GetPixel(x, y).B == 255)
                    {
                        imagemSrc.SetPixel(x, y, Color.Gray);
                        imagemDst.SetPixel(x, y, Color.Black);
                        bool flag;
                        do
                        {
                            flag = false;
                            if (imagemSrc.GetPixel(x + 1, y).B == 255 && imagemSrc.GetPixel(x + 1, y - 1).B == 0)
                            { // o da frente eh branco e o da diagonal eh preto
                                x++;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x + 1, y - 1).B == 255 && imagemSrc.GetPixel(x, y - 1).B == 0 && imagemSrc.GetPixel(x + 1, y).B != 0)
                            { // diagonal eh branco e o de cima eh preto e o da frente n eh preto
                                x++;
                                y--;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x, y - 1).B == 255 && imagemSrc.GetPixel(x - 1, y - 1).B == 0)
                            { //o de cima eh branco e o da diagonal eh preto 
                                y--;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x - 1, y - 1).B == 255 && imagemSrc.GetPixel(x - 1, y).B == 0 && imagemSrc.GetPixel(x, y - 1).B != 0)
                            { // diagonal eh branco e o de tras eh preto
                                x--;
                                y--;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x - 1, y).B == 255 && imagemSrc.GetPixel(x - 1, y + 1).B == 0)
                            { // o de tras eh branco e a diagonal de baixo eh preta
                                x--;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x - 1, y + 1).B == 255 && imagemSrc.GetPixel(x, y + 1).B == 0 && imagemSrc.GetPixel(x - 1, y).B != 0)
                            {// o da diagonal de baixo eh branco e o de baixo eh preto
                                x--;
                                y++;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x, y + 1).B == 255 && imagemSrc.GetPixel(x + 1, y + 1).B == 0)
                            { // o de baixo eh branco e o da diagonal da direita de baixo eh preto
                                y++;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            else if (imagemSrc.GetPixel(x + 1, y + 1).B == 255 && imagemSrc.GetPixel(x + 1, y).B == 0 && imagemSrc.GetPixel(x, y + 1).B != 0)
                            { // o da diagonal da direita de baixo eh branco e o de cima eh 
                                x++;
                                y++;
                                imagemSrc.SetPixel(x, y, Color.Gray);
                                imagemDst.SetPixel(x, y, Color.Black);
                                flag = true;
                            }
                            maiorX = Math.Max(maiorX, x);
                            menorX = Math.Min(menorX, x);
                            maiorY = Math.Max(maiorY, y);
                            menorY = Math.Min(menorY, y);

                        } while (flag);
                        DesenharRetangulo(imagemDst, menorX, maiorX, menorY, maiorY);
                    }
                }
            }

        }


        private static void DesenharRetangulo(Bitmap imagemDst, int menorX, int maiorX, int menorY, int maiorY)
        {
            Color vermelho = Color.FromArgb(255, 1, 1);
            for (int i = menorX; i <= maiorX; i++)
            {
                imagemDst.SetPixel(i, menorY, vermelho);
                imagemDst.SetPixel(i, maiorY, vermelho);
            }

            for (int i = menorY; i <= maiorY; i++)
            {
                imagemDst.SetPixel(menorX, i, vermelho);
                imagemDst.SetPixel(maiorX, i, vermelho);
            }
        }

        public static void detectarBordasSobel(Bitmap imageBitmapSrc, Bitmap imageBitmapDst)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;

            // Primeiro: converter para tons de cinza
            Bitmap gray = new Bitmap(width, height);
            convert_to_gray(imageBitmapSrc, gray);

            int[,] Gx = {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

                    int[,] Gy = {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int sumX = 0;
                    int sumY = 0;

                    // Aplica a máscara 3x3
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int pixel = gray.GetPixel(x + j, y + i).R;
                            sumX += pixel * Gx[i + 1, j + 1];
                            sumY += pixel * Gy[i + 1, j + 1];
                        }
                    }

                    int magnitude = (int)Math.Sqrt(sumX * sumX + sumY * sumY);
                    if (magnitude > 255) magnitude = 255;

                    Color newColor = Color.FromArgb(magnitude, magnitude, magnitude);
                    imageBitmapDst.SetPixel(x, y, newColor);
                }
            }
        }
    }
}
